# build_clients.ps1
# Compila o cliente para todas as plataformas que esta máquina consegue, com um nome por pacote.
#
#   tools\build_clients.ps1
#
# Por que existe: o NVGT grava TODO build do cliente como AmongUs.zip, seja qual for a plataforma.
# Compilar Linux depois do Windows sobrescrevia o zip do Windows em silêncio, e o deploy publicaria
# um binário Linux no lugar do Windows. Aqui cada build é renomeado na hora, e o Windows fica por
# último - é ele que precisa se chamar AmongUs.zip, o nome que o updater dos jogadores baixa.
#
# Mac só compila se o stub existir (stub\nvgt_mac.bin, que a instalação Windows do NVGT não traz).

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
	$nvgtHome = Split-Path -Parent (Get-Command nvgt).Source

	function Build-One([string]$platform, [string]$target) {
		Write-Host "== $platform =="
		nvgt -c "-p$platform" AmongUs.nvgt
		if ($LASTEXITCODE -ne 0 -or -not (Test-Path "AmongUs.zip")) { throw "Build $platform falhou." }
		if ($target -ne "AmongUs.zip") { Move-Item -Force "AmongUs.zip" $target }
		"{0}: {1:N1} MB" -f $target, ((Get-Item $target).Length / 1MB)
	}

	Build-One "linux" "AmongUs-linux.zip"
	if (Test-Path (Join-Path $nvgtHome "stub\nvgt_mac.bin")) {
		Build-One "mac" "AmongUs-mac.zip"
	} else {
		Write-Host "== mac == pulado: sem stub\nvgt_mac.bin em $nvgtHome (ver CLAUDE.md, Compilar)"
	}
	Build-One "windows" "AmongUs.zip"
}
finally {
	Pop-Location
}
