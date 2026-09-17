# sync_translations.ps1
# Sincroniza a pasta lang/ do jogo com o repositório de traduções (github.com/otaviols/game-translations).
#
#   tools\sync_translations.ps1
#
# Nos dois sentidos, e cada sentido é dono de uma coisa:
#   - do repositório PARA o jogo: os idiomas da comunidade (tudo que não é en_US/pt_BR). É assim que
#     uma tradução aprovada lá chega ao build daqui.
#   - do jogo PARA o repositório: en_US e pt_BR, os idiomas embutidos, que são mantidos junto com o
#     código (é aqui que as chaves novas nascem). Vão para lá como REFERÊNCIA, para o tradutor ver o
#     que falta. Se mudaram, vira commit e push.
#
# Chamado no começo do build_clients.ps1, então todo build sai com as traduções mais novas do main.
# O clone fica em D:\git\game-translations (ao lado dos outros repositórios); se não existir, é
# clonado.

param(
	[string]$RepoDir = "D:\git\game-translations",
	[string]$RepoUrl = "https://github.com/otaviols/game-translations.git",
	[string]$Game = "among-us"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$builtin = @("en_US.json", "pt_BR.json")

if (-not (Test-Path (Join-Path $RepoDir ".git"))) {
	Write-Host "Clonando $RepoUrl em $RepoDir..."
	git clone -q $RepoUrl $RepoDir
	if ($LASTEXITCODE -ne 0) { throw "git clone falhou." }
}
git -C $RepoDir pull -q --ff-only
if ($LASTEXITCODE -ne 0) { throw "git pull do repositório de traduções falhou (há mudanças locais lá?)." }

$repoLang = Join-Path $RepoDir "$Game\lang"
if (-not (Test-Path $repoLang)) { throw "Não achei $repoLang no repositório de traduções." }
$gameLang = Join-Path $root "lang"

# repositório -> jogo: idiomas da comunidade
$trazidos = @()
foreach ($f in Get-ChildItem $repoLang -Filter *.json) {
	if ($builtin -contains $f.Name) { continue }
	$destino = Join-Path $gameLang $f.Name
	if (-not (Test-Path $destino) -or (Get-FileHash $f.FullName).Hash -ne (Get-FileHash $destino).Hash) {
		Copy-Item $f.FullName $destino -Force
		$trazidos += $f.Name
	}
}
if ($trazidos) { Write-Host "Traduções atualizadas do repositório: $($trazidos -join ', ')" } else { Write-Host "Traduções da comunidade já em dia." }

# jogo -> repositório: os embutidos, como referência
$mudou = $false
foreach ($name in $builtin) {
	$origem = Join-Path $gameLang $name
	$destino = Join-Path $repoLang $name
	if (-not (Test-Path $destino) -or (Get-FileHash $origem).Hash -ne (Get-FileHash $destino).Hash) {
		Copy-Item $origem $destino -Force
		$mudou = $true
	}
}
if ($mudou) {
	$versao = (Select-String -Path (Join-Path $root "src\config\game_constants.nvgt") -Pattern 'GAME_VERSION = "([^"]+)"').Matches[0].Groups[1].Value
	git -C $RepoDir add -A "$Game/lang"
	git -C $RepoDir commit -q -m "$Game`: idiomas de referência do jogo $versao"
	git -C $RepoDir push -q
	if ($LASTEXITCODE -ne 0) { throw "git push do repositório de traduções falhou." }
	Write-Host "Referência (en_US, pt_BR) enviada ao repositório."
}
