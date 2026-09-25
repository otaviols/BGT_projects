# set_notice.ps1
# Põe (ou tira) o recado fixo do saguão - o aviso que todo mundo ouve ao chegar ao servidor.
#
#   infra\set_notice.ps1 -Text "A versão para Android já está no site."
#   infra\set_notice.ps1 -Clear
#
# Para que serve: avisar os jogadores de algo SEM publicar uma versão do jogo. Uma versão nova no
# site, uma manutenção marcada, um problema conhecido - tudo isso mudava de semana em semana e não
# cabia num changelog, que só chega a quem atualiza.
#
# O recado é falado uma vez, quando chega, para quem está no saguão naquele momento e para quem
# entrar depois; e fica FIXO no topo da lista de partidas, para reler. Some quando você o apagar.
# Ele sobrevive ao reinício do servidor (fica no banco), então lembre de apagá-lo quando deixar de
# valer - um aviso que ficou para trás é pior do que aviso nenhum.
#
# Ele viaja como TEXTO, e não como chave de tradução, e é a única coisa do servidor que faz isso:
# não existe chave para uma frase que ainda não foi escrita. Quem lê recebe o texto como você o
# escreveu, precedido de "Recado do servidor:" no idioma dele. Se a sua base joga em dois idiomas,
# escreva as duas versões na mesma frase.
#
# O token vem do segredo amongus-admin do cluster, como no reply_feedback.ps1; com -Local ele usa o
# AMONGUS_ADMIN_TOKEN do ambiente e fala com o servidor da sua máquina.

param(
	[string]$Text = "",
	[switch]$Clear,
	[switch]$Local,
	[string]$Namespace = "amongus"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

if (-not $Clear -and [string]::IsNullOrWhiteSpace($Text)) {
	throw "Passe -Text ""...."" para pôr um recado, ou -Clear para apagar o que está no ar."
}
if ($Clear) { $Text = "" }

if (-not $Local) {
	$encoded = kubectl get secret amongus-admin -n $Namespace -o jsonpath="{.data.token}" 2>$null
	if ([string]::IsNullOrWhiteSpace($encoded)) {
		throw "Não achei o segredo amongus-admin no namespace $Namespace. Veja infra\reply_feedback.ps1 para criá-lo."
	}
	$env:AMONGUS_ADMIN_TOKEN = [System.Text.Encoding]::UTF8.GetString([Convert]::FromBase64String($encoded))
} elseif ([string]::IsNullOrWhiteSpace($env:AMONGUS_ADMIN_TOKEN)) {
	throw "Com -Local, defina AMONGUS_ADMIN_TOKEN no ambiente (o mesmo valor com que o servidor local subiu)."
} else {
	$env:AMONGUS_SERVER_HOST = "127.0.0.1"
}

# Acentos: a ferramenta imprime UTF-8, e o console do Windows precisa ser avisado disso.
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
Push-Location $root
try {
	nvgt tools/server_admin.nvgt notice $Text
}
finally {
	Pop-Location
	$env:AMONGUS_ADMIN_TOKEN = ""
	$env:AMONGUS_SERVER_HOST = ""
}
