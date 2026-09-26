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
# O clone fica AO LADO do BGT_projects (D:\git\game-translations numa máquina, C:\git\... na outra);
# se não existir, é clonado. O caminho é derivado da pasta do projeto, e não fixo: fixo em D:, este
# script falhava numa máquina sem disco D: - e com ele o build_clients inteiro, que o chama primeiro.

param(
	[string]$RepoDir = "",
	[string]$RepoUrl = "https://github.com/otaviols/game-translations.git",
	[string]$Game = "among-us"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
if ($RepoDir -eq "") { $RepoDir = Join-Path (Split-Path -Parent (Split-Path -Parent $root)) "game-translations" }
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
# Quem decide se há o que commitar é o ESTADO do repositório, e não o `$mudou` desta execução: se
# uma rodada anterior copiou os arquivos e morreu antes do commit, esta os acharia iguais e não
# commitaria nunca - a referência ficaria parada na pasta, sem aviso nenhum. Foi exatamente o que
# aconteceu na primeira vez numa máquina nova.
#
# E o git roda com $ErrorActionPreference = Continue: com core.autocrlf=true ele escreve um AVISO de
# fim de linha na saída de erro, e o PowerShell 5.1 em modo Stop trata isso como falha e mata o
# script no meio (sintoma: "NativeCommandError" apontando para o `git add`, que na verdade deu certo).
# Quem diz se falhou é o $LASTEXITCODE.
$ErrorActionPreference = "Continue"
$pendente = git -C $RepoDir status --porcelain -- "$Game/lang" 2>$null
if (-not [string]::IsNullOrWhiteSpace("$pendente")) {
	$versao = (Select-String -Path (Join-Path $root "src\config\game_constants.nvgt") -Pattern 'GAME_VERSION = "([^"]+)"').Matches[0].Groups[1].Value
	git -C $RepoDir add -A "$Game/lang" 2>$null
	if ($LASTEXITCODE -ne 0) { throw "git add no repositório de traduções falhou." }
	git -C $RepoDir commit -q -m "$Game`: idiomas de referência do jogo $versao" 2>$null
	if ($LASTEXITCODE -ne 0) { throw "git commit no repositório de traduções falhou." }
	git -C $RepoDir push -q 2>$null
	if ($LASTEXITCODE -ne 0) { throw "git push do repositório de traduções falhou." }
	Write-Host "Referência (en_US, pt_BR) enviada ao repositório."
}
$ErrorActionPreference = "Stop"
