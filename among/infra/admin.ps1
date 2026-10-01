# admin.ps1
# A administração do jogo, toda num lugar só.
#
#   infra\admin.ps1 recados                    # recados pendentes dos jogadores
#   infra\admin.ps1 recados -Depois 40         # só os que chegaram depois do #40
#   infra\admin.ps1 recados -Limite 20 -Crash  # os 20 mais recentes, com o crash.log de cada um
#   infra\admin.ps1 recados -Todos             # inclui os arquivados
#   infra\admin.ps1 recados -Saida r.txt       # grava em arquivo (UTF-8) em vez de mostrar
#   infra\admin.ps1 responder 31 "Obrigado! Já está na 0.46.0."
#   infra\admin.ps1 arquivar 42                # arquiva o #42
#   infra\admin.ps1 arquivar 96 -Ate           # arquiva TUDO até o #96
#   infra\admin.ps1 desarquivar 42
#   infra\admin.ps1 aviso "A versão para Android está no site."
#   infra\admin.ps1 aviso -Apagar
#   infra\admin.ps1 traducoes                  # traz as enviadas pelo jogo para translations_inbox\ e APAGA do servidor
#   infra\admin.ps1 traducoes -Manter          # traz e deixa lá
#   infra\admin.ps1 status                     # conexões, salas, partidas, quem está online por plataforma
#   infra\admin.ps1 drenar 120                 # avisa todo mundo e trava partidas novas por 2 minutos
#   infra\admin.ps1 plataformas 30             # jogadores distintos por plataforma e versão nos últimos 30 dias
#
# -Local em qualquer comando que passa pelo servidor: fala com o servidor desta máquina, usando o
# AMONGUS_ADMIN_TOKEN do ambiente (o mesmo com que ele subiu).
#
# Por que um script só: eram cinco (read_feedback, reply_feedback, resolve_feedback, set_notice,
# read_translations), cada um repetindo a busca do token, o acerto de acentos e a limpeza do ambiente
# - e o read_feedback quebrava nesta máquina por um detalhe que os outros não tinham. Agora o
# conserto é feito uma vez.
#
# O token mora no segredo amongus-admin do cluster. Ele vai por variável de ambiente só durante o
# comando e é apagado em seguida - nunca fica em arquivo nem no histórico. Para criar, uma vez:
#   kubectl create secret generic amongus-admin -n amongus --from-literal=token=<senha longa>
# e reiniciar o servidor (kubectl rollout restart deploy/amongus-server -n amongus).

param(
	[Parameter(Position = 0)][string]$Comando = "",
	[Parameter(Position = 1, ValueFromRemainingArguments = $true)][string[]]$Resto = @(),
	[switch]$Local,
	[string]$Namespace = "amongus",
	# recados
	[int]$Depois = 0,
	[int]$Limite = 0,
	[switch]$Crash,
	[switch]$Todos,
	[string]$Saida = "",
	# arquivar
	[switch]$Ate,
	# aviso
	[switch]$Apagar,
	# traducoes
	[switch]$Manter,
	[string]$Pasta = "translations_inbox"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$origem = Get-Location # caminhos relativos (-Saida, -Pasta) são relativos a de onde se chamou

function Uso {
	Get-Content $PSCommandPath | Select-Object -Skip 3 -First 17 | ForEach-Object { $_ -replace '^# ?', '' }
}

# Acentos: Python e NVGT imprimem UTF-8, e o console do Windows mostra "vers�o" se não for avisado.
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
$env:PYTHONIOENCODING = "utf-8"

function Resolve-Caminho([string]$p) {
	if ([System.IO.Path]::IsPathRooted($p)) { return $p }
	return Join-Path $origem $p
}

# Roda um comando de tools/server_admin.nvgt com o token no ambiente, e limpa tudo ao sair.
function Servidor([string[]]$argumentos) {
	if (-not $Local) {
		$encoded = kubectl get secret amongus-admin -n $Namespace -o jsonpath="{.data.token}" 2>$null
		if ([string]::IsNullOrWhiteSpace($encoded)) {
			throw "Não achei o segredo amongus-admin no namespace $Namespace (ver o topo deste script)."
		}
		$env:AMONGUS_ADMIN_TOKEN = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($encoded))
	} elseif ([string]::IsNullOrWhiteSpace($env:AMONGUS_ADMIN_TOKEN)) {
		throw "Com -Local, defina AMONGUS_ADMIN_TOKEN no ambiente (o mesmo valor com que o servidor local subiu)."
	} else {
		$env:AMONGUS_SERVER_HOST = "127.0.0.1"
	}
	Push-Location $root
	try {
		nvgt tools/server_admin.nvgt @argumentos
	}
	finally {
		Pop-Location
		# Só apaga o que ESTE script pôs: com -Local o token é de quem chamou, e apagá-lo deixava o
		# comando seguinte sem token.
		if ($Local) { $env:AMONGUS_SERVER_HOST = "" } else { $env:AMONGUS_ADMIN_TOKEN = "" }
	}
}

# Recados: lidos de uma CÓPIA do banco, e não por comando de rede - é leitura, e o crash.log de cada
# recado não cabe num pacote. Quem escreve no banco (responder, arquivar) é sempre o servidor.
function Recados {
	$pod = kubectl get pods -n $Namespace -o jsonpath="{.items[0].metadata.name}"
	if ([string]::IsNullOrWhiteSpace($pod)) { throw "Não achei o pod do servidor no namespace $Namespace." }
	$work = Join-Path $env:TEMP "amongus_feedback"
	New-Item -ItemType Directory -Force $work | Out-Null
	Push-Location $work
	try {
		Remove-Item "feedback.db" -ErrorAction SilentlyContinue
		# Caminho RELATIVO: com "C:\..." o kubectl lê o "C:" como o separador de "pod:caminho".
		# E com o ErrorAction em Continue só aqui: o kubectl cp escreve um aviso do tar ("Removing
		# leading '/'") na saída de erro, e com "Stop" o PowerShell 5.1 trata isso como falha - era por
		# isso que o read_feedback.ps1 morria nesta máquina. A falha de verdade é o arquivo não chegar.
		$ErrorActionPreference = "Continue"
		kubectl cp -n $Namespace "${pod}:/data/among_users.db" "feedback.db" 2>&1 | Out-Null
		$ErrorActionPreference = "Stop"
		if (-not (Test-Path "feedback.db")) { throw "Não consegui copiar o banco do servidor." }
		$leitor = Join-Path $root "tools\read_feedback.py"
		$flagCrash = if ($Crash) { "1" } else { "0" }
		$flagTodos = if ($Todos) { "1" } else { "0" }
		if ($Saida -ne "") {
			$destino = Resolve-Caminho $Saida
			python $leitor "feedback.db" $Limite $Depois $flagCrash $flagTodos | Out-File -FilePath $destino -Encoding utf8
			Write-Host "Gravado em $destino"
		} else {
			python $leitor "feedback.db" $Limite $Depois $flagCrash $flagTodos
		}
	}
	finally {
		# O banco tem as contas (com hash de senha): não fica largado no disco depois de lido.
		Remove-Item "feedback.db" -ErrorAction SilentlyContinue
		Pop-Location
	}
}

function Numero([string]$nome) {
	if ($Resto.Count -lt 1 -or -not ($Resto[0] -match '^\d+$')) { throw "Falta o número do recado: infra\admin.ps1 $nome <id>" }
	return $Resto[0]
}

switch ($Comando) {
	"recados" { Recados }
	"responder" {
		$id = Numero "responder"
		$texto = ($Resto | Select-Object -Skip 1) -join " "
		if ([string]::IsNullOrWhiteSpace($texto)) { throw 'Falta o texto: infra\admin.ps1 responder <id> "texto"' }
		Servidor @("reply", $id, $texto)
	}
	"arquivar" {
		$id = Numero "arquivar"
		if ($Ate) { Servidor @("resolve", $id, "until") } else { Servidor @("resolve", $id) }
	}
	"desarquivar" { Servidor @("resolve", (Numero "desarquivar"), "undo") }
	"aviso" {
		$texto = $Resto -join " "
		if ($Apagar) { $texto = "" }
		elseif ([string]::IsNullOrWhiteSpace($texto)) {
			# Sem texto e sem -Apagar é quase sempre engano: apagar o aviso de todo mundo tem que ser pedido.
			throw 'Falta o texto: infra\admin.ps1 aviso "texto"  (ou -Apagar para tirar o aviso)'
		}
		Servidor @("notice", $texto)
	}
	"traducoes" {
		$args2 = @("translations", (Resolve-Caminho $Pasta))
		if ($Manter) { $args2 += "keep" }
		Servidor $args2
	}
	"status" { Servidor @("status") }
	"drenar" {
		$s = if ($Resto.Count -gt 0) { $Resto[0] } else { "120" }
		Servidor @("drain", $s)
	}
	"plataformas" {
		$d = if ($Resto.Count -gt 0) { $Resto[0] } else { "30" }
		Servidor @("platforms", $d)
	}
	default { Uso; if ($Comando -ne "") { throw "Comando desconhecido: $Comando" } }
}
