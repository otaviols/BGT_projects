# resolve_feedback.ps1
# Arquiva recados já tratados (implementados, respondidos, ou os dois).
#
#   infra\resolve_feedback.ps1 -Id 42            # arquiva o #42
#   infra\resolve_feedback.ps1 -Until 96         # arquiva TUDO até o #96
#   infra\resolve_feedback.ps1 -Id 42 -Undo      # desarquiva o #42
#
# ARQUIVA, não apaga: o recado some da leitura padrão (infra\read_feedback.ps1) e continua no banco -
# `-All` mostra tudo de novo. O contexto de um recado (versão, papel, sala, crash.log) é justamente o
# que falta quando o mesmo problema volta meses depois.
#
# Quem mexe no banco é o SERVIDOR, e não este script: ele mantém o SQLite aberto o tempo todo, e duas
# mãos no mesmo arquivo é pedir corrupção. Por isso vai por comando de rede, como o reply_feedback.
#
# O token que autoriza mora no segredo amongus-admin do cluster; este script o busca de lá e o passa
# por variável de ambiente - ele nunca fica em arquivo nem no histórico de comandos. Para um servidor
# local de teste, defina AMONGUS_ADMIN_TOKEN antes de subir o servidor e passe -Local.

param(
	[int]$Id = 0,
	[int]$Until = 0,
	[switch]$Undo,
	[switch]$Local,
	[string]$Namespace = "amongus"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

if ($Id -le 0 -and $Until -le 0) {
	throw "Diga o que arquivar: -Id <n> para um recado, ou -Until <n> para todos até ele."
}
if ($Id -gt 0 -and $Until -gt 0) {
	throw "Use -Id OU -Until, não os dois: um arquiva um recado, o outro arquiva tudo até ele."
}
if ($Undo -and $Until -gt 0) {
	throw "-Undo desarquiva UM recado; use -Id."
}

if (-not $Local) {
	$encoded = kubectl get secret amongus-admin -n $Namespace -o jsonpath="{.data.token}" 2>$null
	if ([string]::IsNullOrWhiteSpace($encoded)) {
		throw "Não achei o segredo amongus-admin no namespace $Namespace. Veja o topo de infra\reply_feedback.ps1."
	}
	$env:AMONGUS_ADMIN_TOKEN = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($encoded))
} elseif ([string]::IsNullOrWhiteSpace($env:AMONGUS_ADMIN_TOKEN)) {
	throw "Com -Local, defina AMONGUS_ADMIN_TOKEN no ambiente (o mesmo valor com que o servidor local subiu)."
} else {
	$env:AMONGUS_SERVER_HOST = "127.0.0.1"
}

$alvo = if ($Until -gt 0) { $Until } else { $Id }
$modo = if ($Undo) { "undo" } elseif ($Until -gt 0) { "until" } else { "" }

[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Push-Location $root
try {
	if ($modo -eq "") { nvgt tools/resolve_feedback.nvgt $alvo }
	else { nvgt tools/resolve_feedback.nvgt $alvo $modo }
}
finally {
	Pop-Location
	$env:AMONGUS_ADMIN_TOKEN = ""
	$env:AMONGUS_SERVER_HOST = ""
}
