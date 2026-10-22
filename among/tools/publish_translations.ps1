# publish_translations.ps1
# Publica os idiomas da COMUNIDADE no site, para os jogadores receberem sem esperar uma versão nova do jogo.
#
#   tools\publish_translations.ps1           # sincroniza com o repositório de traduções e publica
#   tools\publish_translations.ps1 -SemSync  # publica o lang/ como está (o deploy já sincronizou)
#
# Depois de promover uma tradução no repositório de traduções (notes/traducoes.md), é isto que a leva aos
# jogadores: na próxima vez que abrirem o jogo, ele lê idiomas/revisions.list no site e baixa o idioma
# cuja revisão for maior que a dele (src/core/lang_download.nvgt). Não compila nada, não mexe no
# servidor do jogo - quem está jogando nem percebe.
#
# Sobe para /var/www/amongus/idiomas.novo e troca a pasta inteira de uma vez no fim: quem baixar no meio
# acha a pasta velha inteira ou a nova inteira, nunca um índice novo apontando para um arquivo velho.
# Português e inglês ficam de fora: nascem junto com o código e só vêm dentro do jogo.

param(
	[switch]$SemSync,
	[string]$SiteHost = "amongus.blindtabern.com"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
. (Join-Path $root "infra\vps.ps1")

if (-not $SemSync) { & (Join-Path $PSScriptRoot "sync_translations.ps1") }

$lang = Join-Path $root "lang"
$revisoes = Join-Path $lang "revisions.list"
if (-not (Test-Path $revisoes)) { throw "Falta lang\revisions.list - rode tools\sync_translations.ps1." }

# A mesma conferência que o deploy faz: um idioma quebrado não sai daqui.
python (Join-Path $PSScriptRoot "check_translations_all.py")
if ($LASTEXITCODE -ne 0) { throw "As traduções não passaram na conferência (veja acima)." }

$codigos = @()
foreach ($linha in [System.IO.File]::ReadAllLines($revisoes)) {
	if ($linha -match '^([A-Za-z_]+)=\d+$') { $codigos += $Matches[1] }
}
if ($codigos.Count -eq 0) { throw "lang\revisions.list não lista idioma nenhum." }

$ssh = Vps-SshArgs
Vps-Run "rm -rf /var/www/amongus/idiomas.novo && mkdir -p /var/www/amongus/idiomas.novo" | Out-Null
$arquivos = @($codigos | ForEach-Object { Join-Path $lang "$_.json" }) + $revisoes
foreach ($f in $arquivos) {
	if (-not (Test-Path $f)) { throw "Falta $f." }
	scp -q @ssh $f "root@${SiteHost}:/var/www/amongus/idiomas.novo/"
	if ($LASTEXITCODE -ne 0) { throw "Falhou o envio de $(Split-Path $f -Leaf)." }
}
Vps-Run ("chmod 755 /var/www/amongus/idiomas.novo && chmod 644 /var/www/amongus/idiomas.novo/* && " +
	"rm -rf /var/www/amongus/idiomas.velho && " +
	"(test -d /var/www/amongus/idiomas && mv /var/www/amongus/idiomas /var/www/amongus/idiomas.velho || true) && " +
	"mv /var/www/amongus/idiomas.novo /var/www/amongus/idiomas && rm -rf /var/www/amongus/idiomas.velho") | Out-Null

# Confere no ar o que o jogo vai ler.
$no_ar = (Invoke-WebRequest -UseBasicParsing "https://$SiteHost/idiomas/revisions.list").Content
if ($no_ar -is [byte[]]) { $no_ar = [System.Text.Encoding]::UTF8.GetString($no_ar) }
if ($no_ar.Trim() -ne ([System.IO.File]::ReadAllText($revisoes)).Trim()) { throw "O revisions.list no ar não bate com o daqui." }
Write-Host "Idiomas publicados no site: $($codigos -join ', ')"
