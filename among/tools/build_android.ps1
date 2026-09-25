# build_android.ps1
# Gera o APK do cliente e CONFERE o que saiu dentro dele.
#
#   tools\build_android.ps1 [-KeepIdsig]
#
# Por que não entrou no build_clients.ps1: o APK ainda não é distribuído (o site serve Windows), o
# build demora bem mais que os outros, e o updater do jogo não sabe instalar APK. Quando o Android
# virar plataforma publicada, este script vira uma linha lá dentro.
#
# Toda a configuração do bundle vai por `-s chave=valor`, na linha de comando, e não num .nvgtrc
# commitado. O motivo é a versão: gravada num arquivo de configuração, ela seria um SEGUNDO lugar
# para lembrar de subir junto do GAME_VERSION, e o projeto já tem essa armadilha resolvida assim no
# version.json - quem esquece descobre pelo jogador. Aqui ela é LIDA do código, então não há como
# divergir.
#
# Duas configurações valem uma explicação:
#   * product_identifier é o nome do aplicativo para o Android, e no Android ele é também o CAMINHO
#     da pasta de dados do jogo. Mudá-lo depois de alguém instalar não atualiza o aplicativo: cria
#     um segundo, e o jogador perde as preferências e a conta lembrada. Ou seja, ele se escolhe uma
#     vez.
#   * android_install = 0 porque o padrão é 1, "perguntar se instala no aparelho conectado", e essa
#     pergunta é um diálogo que fica esperando resposta - num build automatizado ela trava tudo sem
#     dizer o que está esperando (aconteceu: dez minutos de "compilando").

param(
	# O .idsig é a assinatura incremental, usada só para instalar via adb depressa. Não vai para o
	# jogador; o APK já é assinado por dentro (META-INF).
	[switch]$KeepIdsig
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
	$APP_ID = "com.otaviols.amongusaudiogame"
	$APP_NAME = "Among Us Audiogame"

	# A versão sai do código, que é a única fonte dela.
	$constantes = Get-Content "src\config\game_constants.nvgt" -Raw
	if ($constantes -notmatch 'GAME_VERSION\s*=\s*"([^"]+)"') { throw "Não achei GAME_VERSION em src\config\game_constants.nvgt." }
	$versao = $Matches[1]
	Write-Host "== android == versão $versao, identificador $APP_ID"

	if (-not (Test-Path "sounds.dat")) { throw "Falta sounds.dat - rode `nvgt tools/build_pack.nvgt` antes." }

	foreach ($f in @("AmongUs.apk", "AmongUs.apk.idsig", "AmongUs-android.apk")) {
		if (Test-Path $f) { Remove-Item -Force $f }
	}

	# O índice de idiomas, que só o Android usa: dentro de um APK não há pasta para listar, então
	# find_files("lang/*.json") volta vazio e o jogador ficaria preso no idioma padrão (ver
	# get_available_languages em src/i18n.nvgt). É gerado AQUI, imediatamente antes de compilar, para
	# não existir uma segunda lista de idiomas que possa envelhecer sem ninguém notar.
	# A extensão não é .json de propósito: em lang/ tudo que termina em .json é um idioma, tanto para o
	# jogo quanto para o conferidor de traduções do deploy.
	$idiomasIndice = (Get-ChildItem "lang" -Filter *.json | ForEach-Object { $_.BaseName }) | Sort-Object
	Set-Content -Path "lang\index.list" -Value $idiomasIndice -Encoding UTF8

	nvgt -c -pandroid `
		-s "build.product_identifier=$APP_ID" `
		-s "build.product_name=$APP_NAME" `
		-s "build.product_version=$versao" `
		-s "build.android_manifest=tools/android/AndroidManifest.xml" `
		-s "build.android_install=0" `
		AmongUs.nvgt
	if ($LASTEXITCODE -ne 0) { throw "Build android falhou." }
	if (-not (Test-Path "AmongUs.apk")) { throw "O build terminou sem gerar AmongUs.apk." }

	# --- Conferir o que entrou -----------------------------------------------------------------
	# "Compilou" não prova nada neste projeto (ver CLAUDE.md). Um APK sem o sounds.dat roda MUDO, um
	# sem lang/ fala o nome das chaves de tradução, e um com o manifesto interno do NVGT não pede
	# microfone - e nenhum desses três dá erro em lugar nenhum.
	Add-Type -AssemblyName System.IO.Compression.FileSystem
	$zip = [System.IO.Compression.ZipFile]::OpenRead((Join-Path $root "AmongUs.apk"))
	try {
		$nomes = $zip.Entries.FullName

		$exigidos = @("assets/sounds.dat", "lib/arm64-v8a/libgame.so", "lib/arm64-v8a/libphonon.so")
		foreach ($e in $exigidos) {
			if ($nomes -notcontains $e) { throw "O APK saiu sem $e." }
		}

		# Todo idioma que existe em lang/ tem que estar no APK: um faltando é um jogador que escolhe
		# o idioma dele e não encontra.
		$idiomas = (Get-ChildItem "lang" -Filter *.json).Name
		foreach ($i in $idiomas) {
			if ($nomes -notcontains "assets/lang/$i") { throw "O APK saiu sem lang/$i." }
		}
		if ($nomes -notcontains "assets/lang/index.list") { throw "O APK saiu sem lang/index.list: a tela de idiomas ficaria vazia no celular." }

		# O manifesto vai compilado (AXML binário), mas os nomes das permissões continuam legíveis
		# dentro dele como texto UTF-16 - é o suficiente para confirmar que o nosso foi usado, e não
		# o template do NVGT.
		$mf = $zip.GetEntry("AndroidManifest.xml")
		$ms = New-Object System.IO.MemoryStream
		$mf.Open().CopyTo($ms)
		$texto = [System.Text.Encoding]::Unicode.GetString($ms.ToArray())
		if ($texto -notmatch "RECORD_AUDIO") { throw "O APK saiu sem permissão de microfone: o build ignorou tools/android/AndroidManifest.xml." }
		if ($texto -notmatch [regex]::Escape($APP_ID)) { throw "O APK saiu com outro identificador (esperado $APP_ID)." }

		"idiomas: $($idiomas -join ', ')"
	}
	finally { $zip.Dispose() }

	Move-Item -Force "AmongUs.apk" "AmongUs-android.apk"
	if (-not $KeepIdsig -and (Test-Path "AmongUs.apk.idsig")) { Remove-Item -Force "AmongUs.apk.idsig" }
	"{0}: {1:N1} MB" -f "AmongUs-android.apk", ((Get-Item "AmongUs-android.apk").Length / 1MB)
	# O adb não está no PATH: ele vem dentro da instalação do NVGT, junto do aapt2 e do apksigner que
	# acabaram de gerar este APK - não é preciso instalar o SDK do Android para nada disto.
	$adb = Join-Path (Split-Path -Parent (Get-Command nvgt).Source) "android-tools\adb.exe"
	Write-Host "Para instalar num aparelho ligado por USB com depuração ligada:"
	if (Test-Path $adb) { Write-Host "  & `"$adb`" install -r AmongUs-android.apk" }
	else { Write-Host "  adb install -r AmongUs-android.apk" }
}
finally {
	Pop-Location
}
