# deploy.ps1
# Publica uma versão: manda o servidor para a VPS e o cliente para os sites de download.
#
# Rode da raiz do projeto (a pasta among), depois de ter gerado os pacotes:
#   nvgt tools/build_pack.nvgt
#   nvgt -c -plinux server_main.nvgt
#   nvgt -c AmongUs.nvgt
#   infra\deploy.ps1 -StorageAccount <nome>
#
# O nome do storage sai do `terraform output` (é o site antigo, no Azure, que as versões até a 0.51.0
# consultam). O servidor e o site novo moram na VPS da Hostinger desde 2026-10-09 - antes o servidor era
# um contêiner no cluster AKS do Azure. O acesso à VPS está em infra/vps.ps1.

param(
	[Parameter(Mandatory = $true)][string]$StorageAccount,
	# O nome da imagem do servidor; a etiqueta é o commit.
	[string]$Image = "amongus-server",
	# Pular uma das partes é útil quando só o cliente mudou (ou só o servidor).
	[switch]$SkipServer,
	[switch]$SkipSite,
	# Reinício anunciado: quanto tempo avisar os jogadores e esperar as partidas em andamento
	# acabarem antes de trocar o servidor. 0 = trocar na hora (derruba quem estiver jogando).
	[int]$DrainSeconds = 300,
	# Pula a conferência de traduções. Só para emergência (um conserto de servidor que não pode
	# esperar tradutor) - em release normal, não use.
	[switch]$SkipTranslations,
	# O site novo, na VPS da Hostinger (ver notes/infra-e-deploy.md e infra/vps.ps1).
	[string]$SiteHost = "amongus.blindtabern.com"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
. (Join-Path $PSScriptRoot "vps.ps1")

# --- Traduções, ANTES de tudo ---
#
# Primeiro de propósito: recusar aqui custa zero, enquanto recusar depois de construir a imagem e
# avisar os jogadores do reinício custa o tempo de todo mundo.
#
# Isto existe porque a 0.32.0 subiu com um espanhol COMPLETO parado na caixa de entrada: o
# build_clients já conferia, mas tem `-SkipInbox`, e quem pula uma vez pula sempre. O portão de
# verdade é aqui, no que vai ao ar.
#
# Recolher do servidor faz parte da conferência: uma tradução que o jogador mandou ontem e ninguém
# baixou está tão atrasada quanto uma ignorada. `admin.ps1 traducoes` traz e APAGA do servidor,
# então o que vier fica em translations_inbox/ e o conferidor abaixo recusa até alguém tratar.
if (-not $SkipTranslations) {
	Write-Host "Conferindo traducoes..."
	& (Join-Path $PSScriptRoot "admin.ps1") traducoes | Out-Host
	if ($LASTEXITCODE -ne 0) { throw "Nao consegui recolher as traducoes do servidor. Resolva, ou rode com -SkipTranslations." }
	python (Join-Path $root "tools\check_translations_all.py") | Out-Host
	if ($LASTEXITCODE -ne 0) {
		throw "As traducoes nao estao em dia (veja acima). Trate o que falta - CLAUDE.md, secao Traducoes - ou rode com -SkipTranslations."
	}
}

if (-not $SkipServer) {
	# A imagem é etiquetada com o commit atual, e a etiqueta só diz a verdade se o que está sendo
	# compilado É o commit. Com mudanças por commitar, o servidor diria "6b40082" rodando código que o
	# 6b40082 não tem - e quem fosse investigar um problema no servidor olharia o código errado.
	# Já aconteceu. Commite antes de publicar o servidor.
	$dirty = git -C $root status --porcelain --untracked-files=no
	if (-not [string]::IsNullOrWhiteSpace($dirty)) {
		throw "Há mudanças por commitar. A imagem leva o nome do commit, então commite antes de publicar o servidor:`n$dirty"
	}

	# O NVGT grava o servidor como server_main.zip (builds antigas) ou server_main.tar.gz (recentes).
	# O mais NOVO dos dois é o que vale: um zip velho ao lado de um tar.gz recém-gerado já quase foi
	# publicado como se fosse o servidor atual. O pacote é extraído aqui para server_pkg/, que é o
	# que o Dockerfile copia - o tar do Windows abre os dois formatos.
	$packages = @("server_main.tar.gz", "server_main.zip") | ForEach-Object { Join-Path $root $_ } | Where-Object { Test-Path $_ }
	if (-not $packages) { throw "Não achei server_main.tar.gz nem server_main.zip. Gere com: nvgt -c -plinux server_main.nvgt" }
	$serverPkg = $packages | Sort-Object { (Get-Item $_).LastWriteTime } -Descending | Select-Object -First 1
	$stale = $packages | Where-Object { $_ -ne $serverPkg }
	if ($stale) { Write-Warning "Ignorando pacote(s) mais antigo(s): $($stale -join ', ') - apague para não confundir." }
	$pkgDir = Join-Path $root "server_pkg"
	if (Test-Path $pkgDir) { Remove-Item $pkgDir -Recurse -Force }
	New-Item -ItemType Directory -Path $pkgDir | Out-Null
	tar -xf $serverPkg -C $pkgDir
	if ($LASTEXITCODE -ne 0 -or -not (Test-Path (Join-Path $pkgDir "server_main"))) { throw "Não consegui extrair $serverPkg." }
	Write-Host "Servidor: $(Split-Path $serverPkg -Leaf)"

	# A etiqueta é o commit atual, e não `latest`: dá para saber exatamente qual código está no ar
	# olhando o contêiner (`docker inspect amongus-server`), e voltar atrás é subir a etiqueta anterior.
	$tag = (git -C $root rev-parse --short HEAD).Trim()
	if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($tag)) {
		throw "Não consegui descobrir o commit atual para etiquetar a imagem."
	}
	$fullImage = "${Image}:${tag}"

	Write-Host "Construindo $fullImage..."
	# O contexto é a RAIZ do projeto porque o Dockerfile precisa da server_pkg/, que fica lá.
	docker build -f (Join-Path $PSScriptRoot "Dockerfile") -t $fullImage $root
	if ($LASTEXITCODE -ne 0) { throw "docker build falhou." }

	# Sobe a imagem UMA VEZ aqui antes de publicar, e confere que o servidor realmente inicia.
	#
	# Isto existe por causa de um incidente real: uma compilação do NVGT produziu um binário
	# defeituoso - mesmo código-fonte, build seguinte já saiu boa - que morria com segfault ao
	# iniciar. Ele foi publicado, o rollout "deu certo" (a imagem baixa e o contêiner sobe), e o
	# servidor entrou em ciclo de reinício com o jogo fora do ar. Nada no caminho tinha como perceber:
	# compilar com sucesso não é a mesma coisa que o binário funcionar.
	#
	# O teste é o mais barato possível e pega exatamente essa classe de falha: rodar e ver se o
	# processo continua vivo depois de alguns segundos.
	Write-Host "Conferindo se o servidor sobe nesta imagem..."
	docker rm -f amongus-smoketest 2>&1 | Out-Null
	docker volume rm -f amongus-smoketest 2>&1 | Out-Null
	docker run -d --name amongus-smoketest -v amongus-smoketest:/data $fullImage | Out-Null
	Start-Sleep -Seconds 8
	$running = docker inspect amongus-smoketest --format "{{.State.Running}}"
	$smokeLog = docker logs amongus-smoketest 2>&1
	docker rm -f amongus-smoketest 2>&1 | Out-Null
	docker volume rm -f amongus-smoketest 2>&1 | Out-Null
	if ($running -ne "true") {
		Write-Host "--- o que o servidor disse ---"
		Write-Host $smokeLog
		throw "O servidor NÃO fica de pé nesta imagem - nada foi publicado. Recompile (nvgt -c -plinux server_main.nvgt) e tente de novo: uma build do NVGT pode sair defeituosa e a seguinte já sair boa."
	}
	Write-Host "Servidor sobe normalmente."

	# A imagem vai direto para a VPS, sem registro: `docker save` aqui, `scp`, `docker load` lá (~55 MB
	# comprimida). Era o ghcr.io, para o cluster baixar - e o segredo de leitura dele vencia.
	$tar = Join-Path $env:TEMP "amongus-server-$tag.tar"
	Write-Host "Enviando a imagem para a VPS..."
	docker save -o $tar $fullImage
	if ($LASTEXITCODE -ne 0) { throw "docker save falhou." }
	try {
		$a = Vps-SshArgs
		scp -q @a $tar "root@${VpsHost}:/opt/amongus/imagem.tar"
		if ($LASTEXITCODE -ne 0) { throw "Falhou o envio da imagem para a VPS." }
	}
	finally { Remove-Item $tar -ErrorAction SilentlyContinue }
	Vps-Run "docker load -i /opt/amongus/imagem.tar && rm -f /opt/amongus/imagem.tar" | Out-Null

	# O servidor guarda as partidas na memória: trocar o contêiner no meio de uma derruba todo mundo que
	# está jogando, sem aviso - aconteceu, e foi assim que se descobriu. Então, com a imagem nova já
	# pronta e conferida, o servidor ATUAL é avisado: todo jogador conectado ouve "o servidor vai
	# reiniciar em N minutos", nenhuma partida nova pode começar, e o deploy espera as que estão
	# rolando acabarem - ou o prazo. Quem está na sala de espera cai e volta; quem está no meio de
	# uma partida teve N minutos para terminá-la.
	#
	# Se o servidor atual não aceitar o aviso, o deploy segue sem esperar - e diz isso.
	if ($DrainSeconds -gt 0) {
		$env:AMONGUS_ADMIN_TOKEN = Vps-AdminToken
		Push-Location $root
		try {
			$drain = nvgt tools/server_admin.nvgt drain $DrainSeconds 2>&1
			if ("$drain" -notmatch "^ok=") {
				Write-Warning "O servidor atual não aceitou o aviso de reinício ($drain); trocando direto."
			} else {
				Write-Host "Jogadores avisados: reinício em $DrainSeconds s. Esperando as partidas em andamento acabarem..."
				$deadline = (Get-Date).AddSeconds($DrainSeconds)
				while ((Get-Date) -lt $deadline) {
					$st = nvgt tools/server_admin.nvgt status 2>&1
					$emAndamento = ($st | Select-String -Pattern "^in_progress=(\d+)").Matches
					# Leitura que falhou NÃO é "nenhuma partida": continua esperando até o prazo, que os
					# jogadores já ouviram. Antes isto saía do laço e trocava na hora - na 0.46.0 uma única
					# leitura perdida derrubou partidas que tinham 5 minutos prometidos para acabar.
					if ($emAndamento.Count -eq 0) {
						Write-Warning "Não consegui ler o estado do servidor ($("$st".Trim())); esperando mesmo assim."
						Start-Sleep -Seconds 15
						continue
					}
					$n = [int]$emAndamento[0].Groups[1].Value
					if ($n -eq 0) { Write-Host "Nenhuma partida em andamento. Trocando o servidor."; break }
					Write-Host ("  {0} partida(s) em andamento; faltam {1:N0} s do prazo." -f $n, ($deadline - (Get-Date)).TotalSeconds)
					Start-Sleep -Seconds 15
				}
			}
		}
		finally {
			Pop-Location
			$env:AMONGUS_ADMIN_TOKEN = ""
		}
	}

	# Troca o contêiner: o banco fica no volume /opt/amongus/data (dono 10001, o usuário da imagem), e o
	# token no admin.env. As imagens antigas ficam na VPS de propósito: voltar atrás é um `docker run`
	# com a etiqueta anterior (`docker images amongus-server` lista).
	Write-Host "Trocando o servidor na VPS..."
	Vps-Run ("docker rm -f $VpsContainer >/dev/null 2>&1; docker run -d --name $VpsContainer --restart unless-stopped " +
		"-p 8934:8934/udp -v ${VpsData}:/data --env-file /opt/amongus/admin.env $fullImage") | Out-Null
	# A mesma conferência da imagem local, agora no lugar de verdade: vivo depois de alguns segundos.
	Start-Sleep -Seconds 8
	$vivo = Vps-Run "docker inspect $VpsContainer --format '{{.State.Running}}'"
	if ("$vivo".Trim() -ne "true") {
		Write-Warning "O servidor não ficou de pé na VPS. Últimas linhas do log:"
		Vps-Run "docker logs --tail 50 $VpsContainer 2>&1" | Out-Host
		throw "O servidor não subiu na VPS."
	}
	Write-Host "Servidor no ar na VPS ($fullImage), porta 8934/udp."
}

if (-not $SkipSite) {
	$clientZip = Join-Path $root "AmongUs.zip"
	if (-not (Test-Path $clientZip)) {
		throw "Não achei $clientZip. Gere com: nvgt -c AmongUs.nvgt"
	}

	Write-Host "Publicando o site e o cliente..."
	# O site estático mora no container `$web` - é o nome que o Azure exige, não é escolha nossa.
	az storage blob upload --account-name $StorageAccount --auth-mode login `
		--container-name '$web' --name "AmongUs.zip" --file $clientZip --overwrite | Out-Null
	# Os pacotes de Linux e Mac só sobem se existirem: são gerados à parte (ver CLAUDE.md, "Compilar"),
	# e o de Mac depende de um stub que nem toda máquina tem. Um deploy sem eles publica só o Windows
	# e deixa os links antigos de pé.
	foreach ($extra in @("AmongUs-linux.tar.gz", "AmongUs-linux.zip", "AmongUs-mac.iso", "AmongUs-android.apk")) {
		$path = Join-Path $root $extra
		if (Test-Path $path) {
			az storage blob upload --account-name $StorageAccount --auth-mode login `
				--container-name '$web' --name $extra --file $path --overwrite | Out-Null
			Write-Host "Publicado $extra"
		}
	}
	az storage blob upload-batch --account-name $StorageAccount --auth-mode login `
		--destination '$web' --source (Join-Path $PSScriptRoot "site") --overwrite | Out-Null

	$url = az storage account show --name $StorageAccount --query "primaryEndpoints.web" -o tsv
	Write-Host "Site publicado: $url"

	# E o site novo, na VPS. O do Azure continua de pé por quem está numa versão que procura a
	# atualização lá (até a 0.51.0); a 0.51.1 em diante procura aqui. Cada arquivo sobe com nome
	# temporário e é renomeado no fim - nunca se baixa um pacote pela metade - e o version.json é o
	# ÚLTIMO a trocar, como no Azure: quem o lê já acha os pacotes que ele anuncia.
	$ssh = Vps-SshArgs
	$arquivos = @($clientZip)
	foreach ($extra in @("AmongUs-linux.tar.gz", "AmongUs-linux.zip", "AmongUs-mac.iso", "AmongUs-android.apk")) {
		$path = Join-Path $root $extra
		if (Test-Path $path) { $arquivos += $path }
	}
	$arquivos += Get-ChildItem (Join-Path $PSScriptRoot "site") -File | Where-Object { $_.Name -ne "version.json" } | ForEach-Object { $_.FullName }
	$arquivos += Join-Path $PSScriptRoot "site\version.json"
	$renomes = @()
	foreach ($f in $arquivos) {
		$nome = Split-Path $f -Leaf
		scp -q @ssh $f "root@${SiteHost}:/var/www/amongus/.$nome.novo"
		if ($LASTEXITCODE -ne 0) { throw "Falhou o envio de $nome para $SiteHost." }
		$renomes += "mv -f /var/www/amongus/.$nome.novo /var/www/amongus/$nome"
	}
	ssh @ssh "root@$SiteHost" ($renomes -join " && ")
	if ($LASTEXITCODE -ne 0) { throw "Falhou a troca dos arquivos em $SiteHost." }
	Write-Host "Site publicado: https://$SiteHost/"
}
