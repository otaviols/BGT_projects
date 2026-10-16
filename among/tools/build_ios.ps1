# build_ios.ps1
# Gera um .ipa de TESTE do cliente, para instalar num iPhone com o Apple ID de quem testa.
#
#   tools\build_ios.ps1          # -> AmongUs-ios.ipa, sem assinatura
#
# Não é plataforma publicada: não entra no build_clients.ps1 nem no deploy. O .ipa sai SEM assinatura
# (o NVGT não assina para iOS), e o iPhone só instala o que foi assinado - quem instala assina na hora,
# com uma ferramenta como o Sideloadly e um Apple ID gratuito (a instalação vale 7 dias).
#
# A configuração vai por `-s chave=valor`, como no build_android.ps1, e pelo mesmo motivo: a versão é
# LIDA de GAME_VERSION. O identificador é o mesmo do Android. O que o NVGT não deixa configurar (a
# permissão do microfone, a orientação, a versão mínima do iOS) é acertado depois, no Info.plist, por
# tools/patch_ios_ipa.py - o porquê de cada um está lá.

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
	$APP_ID = "com.amongusaudiogame.game"
	$APP_NAME = "Among Us Audiogame"

	$constantes = Get-Content "src\config\game_constants.nvgt" -Raw
	if ($constantes -notmatch 'GAME_VERSION\s*=\s*"([^"]+)"') { throw "Não achei GAME_VERSION em src\config\game_constants.nvgt." }
	$versao = $Matches[1]
	Write-Host "== ios == versão $versao, identificador $APP_ID"

	if (-not (Test-Path "sounds.dat")) { throw "Falta sounds.dat - rode `nvgt tools/build_pack.nvgt` antes." }
	foreach ($f in @("AmongUs.ipa", "AmongUs-ios.ipa")) { if (Test-Path $f) { Remove-Item -Force $f } }

	# O mesmo índice de idiomas do Android: dentro do pacote do app, listar a pasta pode não funcionar
	# (ver get_available_languages em src/i18n.nvgt).
	$idiomasIndice = (Get-ChildItem "lang" -Filter *.json | ForEach-Object { $_.BaseName }) | Sort-Object
	Set-Content -Path "lang\index.list" -Value $idiomasIndice -Encoding UTF8

	nvgt -c -pios `
		-s "build.product_identifier=$APP_ID" `
		-s "build.product_name=$APP_NAME" `
		-s "build.product_version=$versao" `
		AmongUs.nvgt
	if ($LASTEXITCODE -ne 0 -or -not (Test-Path "AmongUs.ipa")) { throw "O NVGT não gerou o AmongUs.ipa." }

	python tools/patch_ios_ipa.py AmongUs.ipa AmongUs-ios.ipa
	if ($LASTEXITCODE -ne 0) { throw "Não consegui acertar o Info.plist." }
	Remove-Item -Force "AmongUs.ipa"
	$mb = [math]::Round((Get-Item "AmongUs-ios.ipa").Length / 1MB, 1)
	Write-Host "AmongUs-ios.ipa pronto ($mb MB), sem assinatura - assine ao instalar (Sideloadly)."
}
finally { Pop-Location }
