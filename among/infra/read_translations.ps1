# read_translations.ps1
# Recolhe as traduções que os jogadores enviaram pelo jogo (opção "Enviar uma tradução" na lista de
# partidas). Cada envio vira um arquivo em translations_inbox\<código>__<usuário>.json e é apagado do
# servidor ao ser trazido - o banco do servidor é caixa de entrada, não arquivo.
#
#   infra\read_translations.ps1          # recolhe e apaga do servidor
#   infra\read_translations.ps1 -Keep    # recolhe e deixa lá
#   infra\read_translations.ps1 -Local   # contra um servidor local (AMONGUS_ADMIN_TOKEN no ambiente)
#
# Depois de recolher: revisar o arquivo (tools\check_translation.py diz o que falta e o que sobra),
# e commitar no repositório de traduções (ver CLAUDE.md, "Traduções"). O build seguinte traz de lá.
#
# O token vem do segredo amongus-admin do cluster, como no reply_feedback.ps1.

param(
	[switch]$Keep,
	[switch]$Local,
	[string]$Namespace = "amongus",
	[string]$Out = "translations_inbox"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

if (-not $Local) {
	$encoded = kubectl get secret amongus-admin -n $Namespace -o jsonpath="{.data.token}" 2>$null
	if ([string]::IsNullOrWhiteSpace($encoded)) { throw "Não achei o segredo amongus-admin no namespace $Namespace." }
	$env:AMONGUS_ADMIN_TOKEN = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($encoded))
} elseif ([string]::IsNullOrWhiteSpace($env:AMONGUS_ADMIN_TOKEN)) {
	throw "Com -Local, defina AMONGUS_ADMIN_TOKEN no ambiente (o mesmo valor com que o servidor local subiu)."
} else {
	$env:AMONGUS_SERVER_HOST = "127.0.0.1"
}

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$destino = if ([System.IO.Path]::IsPathRooted($Out)) { $Out } else { Join-Path $root $Out }
Push-Location $root
try {
	if ($Keep) { nvgt tools/read_translations.nvgt $destino keep } else { nvgt tools/read_translations.nvgt $destino }
}
finally {
	Pop-Location
	$env:AMONGUS_ADMIN_TOKEN = ""
	$env:AMONGUS_SERVER_HOST = ""
}
