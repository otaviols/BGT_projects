# deploy.ps1
# Publica uma versão: manda o servidor pro cluster e o cliente pro site de download.
#
# Rode da raiz do projeto (a pasta among), depois de ter gerado os pacotes:
#   nvgt tools/build_pack.nvgt
#   nvgt -c -plinux server_main.nvgt
#   nvgt -c AmongUs.nvgt
#   infra\deploy.ps1 -StorageAccount <nome>
#
# O nome do storage sai do `terraform output`. O servidor não precisa mais de -ServerIp: ele deixou
# de ser uma VM alcançada por SSH e virou um contêiner no cluster, então quem sabe onde ele fica é o
# kubectl.

param(
	[Parameter(Mandatory = $true)][string]$StorageAccount,
	# Onde a imagem do servidor é publicada. O padrão é o mesmo registro que o outro jogo já usa.
	[string]$Image = "ghcr.io/otaviols/amongus-server",
	# Pular uma das partes é útil quando só o cliente mudou (ou só o servidor).
	[switch]$SkipServer,
	[switch]$SkipSite,
	# Reinício anunciado: quanto tempo avisar os jogadores e esperar as partidas em andamento
	# acabarem antes de trocar o servidor. 0 = trocar na hora (derruba quem estiver jogando).
	[int]$DrainSeconds = 300
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

if (-not $SkipServer) {
	# A imagem é etiquetada com o commit atual, e a etiqueta só diz a verdade se o que está sendo
	# compilado É o commit. Com mudanças por commitar, o pod diria "6b40082" rodando código que o
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

	# A tag é o commit atual, e não `latest`, por dois motivos: dá para saber exatamente qual código
	# está no ar olhando o pod, e o Kubernetes só reinicia o servidor quando a tag MUDA - com
	# `latest` fixo, `kubectl apply` não veria diferença nenhuma e o deploy não faria nada.
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

	Write-Host "Enviando a imagem..."
	docker push $fullImage
	if ($LASTEXITCODE -ne 0) {
		throw "docker push falhou. Se for erro de autenticação: docker login ghcr.io -u <usuario>"
	}

	# O servidor guarda as partidas na memória: trocar o pod no meio de uma derruba todo mundo que
	# está jogando, sem aviso - aconteceu, e foi assim que se descobriu. Então, com a imagem nova já
	# pronta e conferida, o servidor ATUAL é avisado: todo jogador conectado ouve "o servidor vai
	# reiniciar em N minutos", nenhuma partida nova pode começar, e o deploy espera as que estão
	# rolando acabarem - ou o prazo. Quem está na sala de espera cai e volta; quem está no meio de
	# uma partida teve N minutos para terminá-la.
	#
	# Se o servidor atual não conhece o aviso (versão anterior a esta) ou o token não bater, o
	# deploy segue sem esperar - e diz isso.
	if ($DrainSeconds -gt 0) {
		$encoded = kubectl get secret amongus-admin -n amongus -o jsonpath="{.data.token}" 2>$null
		if ([string]::IsNullOrWhiteSpace($encoded)) {
			Write-Warning "Sem o segredo amongus-admin: não dá para avisar os jogadores; trocando o servidor direto."
		} else {
			$env:AMONGUS_ADMIN_TOKEN = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($encoded))
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
						if ($emAndamento.Count -eq 0) { Write-Warning "Não consegui ler o estado do servidor; seguindo."; break }
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
	}

	Write-Host "Atualizando o servidor no cluster..."
	kubectl apply -f (Join-Path $PSScriptRoot "k8s/amongus.yaml")
	if ($LASTEXITCODE -ne 0) { throw "kubectl apply falhou." }

	# O YAML tem um PLACEHOLDER na tag: quem resolve qual versão sobe é este comando, não o arquivo.
	# Assim o manifesto continua igual entre versões e o histórico não vira uma sequência de commits
	# que só trocam um número.
	kubectl set image -n amongus deployment/amongus-server server=$fullImage
	if ($LASTEXITCODE -ne 0) { throw "kubectl set image falhou." }

	# Sem --timeout o comando espera para sempre se o pod não subir, e o deploy trava sem dizer por
	# quê. Com ele, a falha aparece e os logs abaixo mostram a causa.
	kubectl rollout status -n amongus deployment/amongus-server --timeout=180s
	if ($LASTEXITCODE -ne 0) {
		Write-Warning "O servidor não subiu. Últimas linhas do log:"
		kubectl logs -n amongus deployment/amongus-server --tail=50
		throw "rollout falhou."
	}

	$serverIp = kubectl get svc -n amongus amongus-server -o jsonpath="{.status.loadBalancer.ingress[0].ip}"
	Write-Host "Servidor no ar em ${serverIp}:8934/udp"
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
	foreach ($extra in @("AmongUs-linux.tar.gz", "AmongUs-linux.zip", "AmongUs-mac.iso")) {
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
}
