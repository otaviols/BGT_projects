# build_clients.ps1
# Compila o cliente para todas as plataformas que esta máquina consegue, com um nome por pacote.
#
#   tools\build_clients.ps1
#
# Por que existe: o NVGT grava TODO build do cliente com o mesmo nome base (AmongUs.<ext>), seja qual
# for a plataforma. Compilar Linux depois do Windows sobrescrevia o zip do Windows em silêncio, e o
# deploy publicaria um binário Linux no lugar do Windows. Aqui cada build é renomeado na hora, e o
# Windows fica por último - é ele que precisa se chamar AmongUs.zip, o nome que o updater baixa.
#
# Mac só compila se o stub existir (stub\nvgt_mac.bin, que nem toda instalação Windows do NVGT traz).
#
# O formato do pacote depende da plataforma E da versão do NVGT: Windows sai .zip; Linux saía .zip e
# nas builds recentes sai .tar.gz (preserva permissões de execução); Mac fora de um Mac sai .iso
# (o .dmg precisa do hdiutil, que só existe no macOS - o ISO 9660 com Rock Ridge faz o mesmo papel e
# o macOS monta com dois cliques). Por isso cada build aceita mais de um nome de saída e renomeia o
# que apareceu, mantendo a extensão.

param(
	# Pula a caixa de entrada de traduções (útil offline ou quando já foi revisada nesta sessão).
	[switch]$SkipInbox
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
	$nvgtHome = Split-Path -Parent (Get-Command nvgt).Source

	# Traduções, ANTES de compilar: todo build sai com o main do repositório de traduções, e o que
	# os jogadores enviaram pelo jogo é recolhido aqui - se houver algo na caixa de entrada, o
	# build para para você revisar (tools\check_translation.py) e commitar no repositório. Senão a
	# tradução ficaria esquecida no servidor até alguém lembrar de olhar.
	if (-not $SkipInbox) {
		& (Join-Path $PSScriptRoot "..\infra\admin.ps1") traducoes | Out-Host
		$inbox = Join-Path $root "translations_inbox"
		$pendentes = if (Test-Path $inbox) { Get-ChildItem $inbox -Filter *.json } else { @() }
		if ($pendentes) {
			throw "Há $($pendentes.Count) tradução(ões) enviada(s) em $inbox para revisar antes de buildar (ou rode com -SkipInbox)."
		}
	}
	& (Join-Path $PSScriptRoot "sync_translations.ps1") | Out-Host

	# $candidates: os nomes que o NVGT pode gravar para esta plataforma; $suffix: o que vai no nome
	# final ("" para o Windows, que precisa continuar AmongUs.<ext>).
	function Build-One([string]$platform, [string[]]$candidates, [string]$suffix) {
		Write-Host "== $platform =="
		foreach ($c in $candidates) { if (Test-Path $c) { Remove-Item -Force $c } }
		nvgt -c "-p$platform" AmongUs.nvgt
		if ($LASTEXITCODE -ne 0) { throw "Build $platform falhou." }
		$produced = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
		if (-not $produced) { throw "Build $platform não gerou nenhum de: $($candidates -join ', ')" }
		$ext = $produced.Substring("AmongUs".Length) # ".zip", ".tar.gz", ".iso"
		$target = "AmongUs$suffix$ext"
		if ($target -ne $produced) { Move-Item -Force $produced $target }
		"{0}: {1:N1} MB" -f $target, ((Get-Item $target).Length / 1MB)
	}

	Build-One "linux" @("AmongUs.tar.gz", "AmongUs.zip") "-linux"
	if (Test-Path (Join-Path $nvgtHome "stub\nvgt_mac.bin")) {
		Build-One "mac" @("AmongUs.iso", "AmongUs.dmg", "AmongUs.zip") "-mac"
	} else {
		Write-Host "== mac == pulado: sem stub\nvgt_mac.bin em $nvgtHome (ver CLAUDE.md, Compilar)"
	}
	Build-One "windows" @("AmongUs.zip") ""

	# No Windows o AmongUs.exe que o jogador abre é o INICIADOR (launcher.nvgt), e o jogo vira
	# AmongUsGame.exe na mesma pasta: se uma versão do jogo fechar ao abrir, o iniciador ainda está de pé
	# para devolver a anterior. O NVGT compila um script por pacote, então o zip é remontado aqui. Os
	# dois executáveis dividem lib/ - é o mesmo NVGT.
	Write-Host "== iniciador =="
	if (Test-Path launcher.zip) { Remove-Item -Force launcher.zip }
	nvgt -c -pwindows launcher.nvgt
	if ($LASTEXITCODE -ne 0 -or -not (Test-Path launcher.zip)) { throw "Build do iniciador falhou." }
	$stage = Join-Path $env:TEMP "amongus_stage"
	$launcherStage = Join-Path $env:TEMP "amongus_launcher_stage"
	foreach ($d in @($stage, $launcherStage)) { if (Test-Path $d) { Remove-Item -Recurse -Force $d } }
	Expand-Archive AmongUs.zip $stage
	Expand-Archive launcher.zip $launcherStage
	Move-Item (Join-Path $stage "AmongUs.exe") (Join-Path $stage "AmongUsGame.exe")
	Copy-Item (Join-Path $launcherStage "launcher.exe") (Join-Path $stage "AmongUs.exe")
	Remove-Item -Force AmongUs.zip, launcher.zip
	# ZipFile, e não Compress-Archive: o do PowerShell 5.1 grava os caminhos com barra invertida.
	Add-Type -AssemblyName System.IO.Compression.FileSystem
	[IO.Compression.ZipFile]::CreateFromDirectory($stage, (Join-Path $root "AmongUs.zip"))
	Remove-Item -Recurse -Force $stage, $launcherStage
	"AmongUs.zip: {0:N1} MB (iniciador + jogo)" -f ((Get-Item AmongUs.zip).Length / 1MB)

	# Abre o pacote como o jogador abre - pelo iniciador - e confere que o JOGO ficou de pé. Erro na
	# inicialização das globais (a 0.45.0: uma constante declarada depois da global que a usava) compila
	# limpo, passa em toda sonda e fecha o jogo antes de abrir.
	Write-Host "== abrindo o AmongUs.exe compilado =="
	$smoke = Join-Path $env:TEMP "amongus_smoke"
	if (Test-Path $smoke) { Remove-Item -Recurse -Force $smoke }
	Expand-Archive AmongUs.zip $smoke
	$launcher = Start-Process (Join-Path $smoke "AmongUs.exe") -WorkingDirectory $smoke -PassThru
	Start-Sleep -Seconds 8
	$gameExe = Join-Path $smoke "AmongUsGame.exe"
	$game = Get-Process -Name AmongUsGame -ErrorAction SilentlyContinue | Where-Object { $_.Path -eq $gameExe }
	$launcherDone = $launcher.HasExited
	if ($game) { $game | Stop-Process -Force }
	if (-not $launcher.HasExited) { Stop-Process $launcher -Force }
	Start-Sleep -Seconds 1
	Remove-Item -Recurse -Force $smoke -ErrorAction SilentlyContinue
	if (-not $game) { throw "O jogo compilado NÃO ficou aberto. Rode 'nvgt AmongUs.nvgt' para ver o erro." }
	# O iniciador sai assim que vê o sinal do jogo; ainda aberto quer dizer que não viu.
	if (-not $launcherDone) { throw "O iniciador não viu o sinal de que o jogo abriu (ver game_started_marker_path)." }
	Write-Host "ok: o iniciador abriu o jogo e saiu"
}
finally {
	Pop-Location
}
