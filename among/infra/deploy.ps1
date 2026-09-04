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
	[switch]$SkipSite
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

if (-not $SkipServer) {
	$serverZip = Join-Path $root "server_main.zip"
	if (-not (Test-Path $serverZip)) {
		throw "Não achei $serverZip. Gere com: nvgt -c -plinux server_main.nvgt"
	}

	# A tag é o commit atual, e não `latest`, por dois motivos: dá para saber exatamente qual código
	# está no ar olhando o pod, e o Kubernetes só reinicia o servidor quando a tag MUDA - com
	# `latest` fixo, `kubectl apply` não veria diferença nenhuma e o deploy não faria nada.
	$tag = (git -C $root rev-parse --short HEAD).Trim()
	if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($tag)) {
		throw "Não consegui descobrir o commit atual para etiquetar a imagem."
	}
	$fullImage = "${Image}:${tag}"

	Write-Host "Construindo $fullImage..."
	# O contexto é a RAIZ do projeto porque o Dockerfile precisa do server_main.zip, que fica lá.
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
	az storage blob upload-batch --account-name $StorageAccount --auth-mode login `
		--destination '$web' --source (Join-Path $PSScriptRoot "site") --overwrite | Out-Null

	$url = az storage account show --name $StorageAccount --query "primaryEndpoints.web" -o tsv
	Write-Host "Site publicado: $url"
}
