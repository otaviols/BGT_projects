# Servidor de TESTE na VPS: um segundo contêiner, ao lado do oficial, para testar uma versão nova com
# amigos antes de publicá-la. Nada aqui toca o servidor oficial (amongus-server, porta 8934).
#
#   infra\test_server.ps1 subir      # compila a imagem do commit atual, confere que sobe, e troca o de teste
#   infra\test_server.ps1 status     # o que está no ar no de teste
#   infra\test_server.ps1 desligar   # para e remove o contêiner de teste (o banco dele fica)
#
# O de teste:
#   - escuta na porta UDP 8935 (o cliente aponta para ele com um server.txt "amongus.blindtabern.com:8935");
#   - tem BANCO PRÓPRIO, em /opt/amongus/test-data - as contas do teste não são as do jogo;
#   - aceita os mapas em construção (o arquivo mapas_em_teste.txt na pasta dele);
#   - manda e-mail se houver um email.json lá, copiado do servidor de teste local (com "required": false).
#
# Rode de uma árvore COMMITADA: a imagem leva o nome do commit, como no deploy oficial.

param(
	[Parameter(Mandatory = $true, Position = 0)][ValidateSet("subir", "status", "desligar")][string]$Acao,
	# De onde vem o email.json (a senha da caixa de e-mail nunca entra no repositório).
	[string]$EmailJson = "C:\apto10\acer\documents\Among Us - servidor de teste\email.json"
)

$ErrorActionPreference = "Stop"
. (Join-Path $PSScriptRoot "vps.ps1")
$root = Split-Path -Parent $PSScriptRoot
$TestContainer = "amongus-test"
$TestData = "/opt/amongus/test-data"
$TestPort = 8935

function Status {
	Vps-Run "docker ps -a --filter name=^/$TestContainer`$ --format '{{.Names}} {{.Image}} {{.Status}} {{.Ports}}'"
}

if ($Acao -eq "status") { Status; return }

if ($Acao -eq "desligar") {
	Vps-Run "docker rm -f $TestContainer >/dev/null 2>&1; echo desligado" | Out-Host
	return
}

# --- subir ---
$dirty = git -C $root status --porcelain --untracked-files=no
if (-not [string]::IsNullOrWhiteSpace($dirty)) { throw "Há mudanças por commitar; a imagem leva o nome do commit." }
$tag = (git -C $root rev-parse --short HEAD).Trim()
$fullImage = "amongus-server:$tag"

Write-Host "Compilando o servidor (Linux)..."
Push-Location $root
try {
	Remove-Item server_main.zip -ErrorAction SilentlyContinue
	nvgt -c -plinux server_main.nvgt | Out-Host
	if (-not (Test-Path "server_main.tar.gz")) { throw "O NVGT não gerou o server_main.tar.gz." }
	$pkgDir = Join-Path $root "server_pkg"
	if (Test-Path $pkgDir) { Remove-Item $pkgDir -Recurse -Force }
	New-Item -ItemType Directory -Path $pkgDir | Out-Null
	tar -xf server_main.tar.gz -C $pkgDir
	if (-not (Test-Path (Join-Path $pkgDir "server_main"))) { throw "Não consegui extrair o servidor." }
	docker build -f (Join-Path $PSScriptRoot "Dockerfile") -t $fullImage $root | Out-Host
	if ($LASTEXITCODE -ne 0) { throw "docker build falhou." }
}
finally { Pop-Location }

# O mesmo teste do deploy: uma build do NVGT pode sair defeituosa e morrer ao abrir.
Write-Host "Conferindo se o servidor sobe nesta imagem..."
docker rm -f amongus-smoketest 2>&1 | Out-Null
docker run -d --name amongus-smoketest $fullImage | Out-Null
Start-Sleep -Seconds 8
$running = docker inspect amongus-smoketest --format "{{.State.Running}}"
docker rm -f amongus-smoketest 2>&1 | Out-Null
if ($running -ne "true") { throw "O servidor NÃO fica de pé nesta imagem. Recompile e tente de novo." }

# A imagem pode já estar na VPS (o oficial do mesmo commit): só manda se faltar.
$tem = Vps-Run "docker image inspect $fullImage >/dev/null 2>&1 && echo sim || echo nao"
if ("$tem".Trim() -ne "sim") {
	Write-Host "Enviando a imagem para a VPS..."
	$tar = Join-Path $env:TEMP "amongus-test-$tag.tar"
	docker save -o $tar $fullImage
	try {
		$a = Vps-SshArgs
		scp -q @a $tar "root@${script:VpsHost}:/opt/amongus/imagem-teste.tar"
		if ($LASTEXITCODE -ne 0) { throw "Falhou o envio da imagem." }
	}
	finally { Remove-Item $tar -ErrorAction SilentlyContinue }
	Vps-Run "docker load -i /opt/amongus/imagem-teste.tar >/dev/null && rm -f /opt/amongus/imagem-teste.tar" | Out-Null
}

# A pasta do de teste: o arquivo dos mapas em teste e, se houver, o e-mail (sempre opcional aqui).
Vps-Run "mkdir -p $TestData && echo 'servidor de teste: mapas em construcao aceitos' > $TestData/mapas_em_teste.txt" | Out-Null
if (Test-Path $EmailJson) {
	$cfg = Get-Content $EmailJson -Raw | ConvertFrom-Json
	$cfg.required = $false
	$tmp = Join-Path $env:TEMP "email-teste.json"
	$cfg | ConvertTo-Json | Set-Content -Path $tmp -Encoding utf8
	try {
		$a = Vps-SshArgs
		scp -q @a $tmp "root@${script:VpsHost}:$TestData/email.json"
		if ($LASTEXITCODE -ne 0) { throw "Falhou o envio do email.json." }
	}
	finally { Remove-Item $tmp -ErrorAction SilentlyContinue }
	Vps-Run "chmod 600 $TestData/email.json" | Out-Null
}
else { Write-Warning "Sem ${EmailJson}: o de teste sobe sem e-mail." }
Vps-Run "chown -R 10001:10001 $TestData" | Out-Null

Write-Host "Trocando o contêiner de teste..."
Vps-Run ("docker rm -f $TestContainer >/dev/null 2>&1; docker run -d --name $TestContainer --restart unless-stopped " +
	"-p ${TestPort}:8934/udp -v ${TestData}:/data $fullImage >/dev/null") | Out-Null
Start-Sleep -Seconds 5
Status | Out-Host
Vps-Run "docker logs $TestContainer 2>&1 | tail -6" | Out-Host
Write-Host ""
Write-Host "No ar. O jogo de teste aponta para ele com um server.txt contendo:  ${script:VpsHost}:$TestPort"
