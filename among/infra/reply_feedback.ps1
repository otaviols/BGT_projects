# reply_feedback.ps1
# Responde a um recado de jogador, de dentro do jogo.
#
#   infra\reply_feedback.ps1 -Id 31 -Text "Obrigado! Manda o email certo por aqui mesmo."
#
# O jogador dono do recado ouve a resposta na hora, se estiver conectado, ou na próxima vez que
# abrir o jogo: "você tem uma resposta a um recado seu". Nenhum email, nenhum canal fora do jogo.
#
# O token que autoriza responder mora num segredo do cluster (amongus-admin, ver infra/k8s). Este
# script o busca de lá e o passa à ferramenta por variável de ambiente - ele nunca fica em arquivo
# nem no histórico de comandos. Para criar o segredo, uma vez:
#
#   kubectl create secret generic amongus-admin -n amongus --from-literal=token=<senha longa>
#
# e reiniciar o servidor (kubectl rollout restart deploy/amongus-server -n amongus) para ele ler.
#
# Para responder num servidor local de teste, defina AMONGUS_ADMIN_TOKEN antes de subir o servidor
# e passe -Local: o script usa esse valor em vez de ir ao cluster.

param(
	[Parameter(Mandatory = $true)][int]$Id,
	[Parameter(Mandatory = $true)][string]$Text,
	[switch]$Local,
	[string]$Namespace = "amongus"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

if (-not $Local) {
	$encoded = kubectl get secret amongus-admin -n $Namespace -o jsonpath="{.data.token}" 2>$null
	if ([string]::IsNullOrWhiteSpace($encoded)) {
		throw "Não achei o segredo amongus-admin no namespace $Namespace. Crie-o como descrito no topo deste script."
	}
	$env:AMONGUS_ADMIN_TOKEN = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($encoded))
} elseif ([string]::IsNullOrWhiteSpace($env:AMONGUS_ADMIN_TOKEN)) {
	throw "Com -Local, defina AMONGUS_ADMIN_TOKEN no ambiente (o mesmo valor com que o servidor local subiu)."
} else {
	$env:AMONGUS_SERVER_HOST = "127.0.0.1" # a ferramenta fala com o servidor local, não com o oficial
}

# Acentos: a ferramenta imprime UTF-8, e o console do Windows precisa ser avisado disso.
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Push-Location $root
try {
	nvgt tools/reply_feedback.nvgt $Id $Text
}
finally {
	Pop-Location
	$env:AMONGUS_ADMIN_TOKEN = ""
	$env:AMONGUS_SERVER_HOST = ""
}
