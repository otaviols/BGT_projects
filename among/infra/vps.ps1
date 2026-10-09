# vps.ps1
# O acesso à VPS da Hostinger, onde moram o servidor do jogo e o site (desde 2026-10-09; antes era o
# cluster AKS do Azure). Incluído com `. (Join-Path $PSScriptRoot "vps.ps1")` pelo deploy.ps1, pelo
# admin.ps1 e pelo release.ps1 - os três precisam do token, do banco e do commit no ar, e cada um
# escrevendo o seu ssh divergiria na primeira mudança. Ver notes/infra-e-deploy.md, "VPS na Hostinger".

# O NOME, e não o IP: é por ele que o known_hosts conhece a máquina (ver a armadilha do "scp:
# Connection closed" nas notas). O servidor do jogo e o site são a mesma VPS.
$script:VpsHost = "amongus.blindtabern.com"
$script:VpsKey = Join-Path $env:USERPROFILE ".ssh\amongus_vps"
$script:VpsData = "/opt/amongus/data"
$script:VpsContainer = "amongus-server"

function Vps-SshArgs { return @("-i", $script:VpsKey, "-o", "IdentitiesOnly=yes", "-o", "BatchMode=yes") }

# Um comando na VPS; devolve a saída. Falhou, para tudo: um passo de servidor que falha calado é o que
# este projeto mais paga.
function Vps-Run([string]$comando) {
	$a = Vps-SshArgs
	$saida = ssh @a "root@$script:VpsHost" $comando
	if ($LASTEXITCODE -ne 0) { throw "Falhou na VPS: $comando" }
	return $saida
}

# O token de administração, lido do arquivo com que o contêiner sobe (/opt/amongus/admin.env, modo 600).
# Ele só passa pela memória deste processo - nunca vai para arquivo daqui.
function Vps-AdminToken {
	$linha = Vps-Run "cat /opt/amongus/admin.env" | Where-Object { $_ -match '^AMONGUS_ADMIN_TOKEN=' } | Select-Object -First 1
	if (-not $linha) { throw "Não achei AMONGUS_ADMIN_TOKEN em /opt/amongus/admin.env na VPS." }
	return ($linha -replace '^AMONGUS_ADMIN_TOKEN=', '').Trim()
}

# Copia o banco de contas da VPS para `destino`, e confere que chegou um SQLite inteiro.
function Vps-CopyDb([string]$destino) {
	$a = Vps-SshArgs
	scp -q @a "root@${script:VpsHost}:$script:VpsData/among_users.db" $destino
	if ($LASTEXITCODE -ne 0 -or -not (Test-Path $destino)) { throw "Não consegui copiar o banco da VPS." }
	$cabeca = [System.Text.Encoding]::ASCII.GetString([System.IO.File]::ReadAllBytes($destino)[0..14])
	if ($cabeca -ne "SQLite format 3") { Remove-Item $destino -ErrorAction SilentlyContinue; throw "O banco copiado da VPS não é um SQLite inteiro." }
}

# O commit do servidor que está no ar: a etiqueta da imagem do contêiner ("amongus-server:5761992").
function Vps-ServerCommit {
	$imagem = Vps-Run "docker inspect $script:VpsContainer --format '{{.Config.Image}}'"
	if ("$imagem" -match ':([0-9a-f]{7,})\s*$') { return $Matches[1] }
	return ""
}
