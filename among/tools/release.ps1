# release.ps1
# Publica uma versão do começo ao fim, conferindo cada passo que já falhou calado alguma vez.
#
#   tools\release.ps1                     # o normal: confere, compila, publica (servidor só se mudou)
#   tools\release.ps1 -DrainSeconds 600   # mais paciência com quem está jogando
#   tools\release.ps1 -Conferir           # só as conferências, sem compilar nem publicar
#
# ANTES de rodar, à mão (é escrita, não dá para automatizar): subir GAME_VERSION e escrever a entrada
# do changelog nos dois idiomas - ver CLAUDE.md, "Publicar uma versão". E COMMITAR. O resto é daqui:
#
#   1. version.json gerado do changelog (o make_version_json recusa versões que não batem);
#   2. árvore limpa (a imagem do servidor leva o nome do commit);
#   3. a versão é MAIS NOVA que a do site (senão nenhum jogador seria avisado);
#   4. sounds.dat regerado se algum som mudou (sem isso o build sai com o som velho, calado);
#   5. servidor publicado só se o código dele mudou desde o commit que está no ar;
#   6. clientes (com o teste de abertura), Android, servidor; push; deploy;
#   7. confere no ar: o site anuncia a versão nova, e o pod roda o commit novo.
#
# Rode SEM redirecionar a saída (nada de `| Tee-Object`, `*>&1`): o deploy morre no PowerShell 5.1
# quando a saída do docker build é redirecionada - ver notes/infra-e-deploy.md.
#
# Por que existe: publicar eram cinco passos à mão, e cada um já escapou uma vez - version.json para
# trás, sounds.dat velho, zip do Linux no lugar do Windows, servidor publicado sem precisar
# derrubando partidas, e a 0.45.0 que não abria.

param(
	[int]$DrainSeconds = 300,
	[switch]$Conferir,
	[string]$StorageAccount = "amongusaudiogame",
	[string]$Namespace = "amongus"
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Push-Location $root
try {
	function Passo([string]$t) { Write-Host ""; Write-Host "== $t ==" }

	$versao = ([regex]::Match((Get-Content src\config\game_constants.nvgt -Raw), 'GAME_VERSION\s*=\s*"([^"]+)"')).Groups[1].Value
	if (-not $versao) { throw "Não achei GAME_VERSION em src\config\game_constants.nvgt." }
	Passo "versão $versao"

	python tools\make_version_json.py
	if ($LASTEXITCODE -ne 0) { throw "O version.json não saiu (veja acima): a versão do changelog bate com GAME_VERSION nos dois idiomas?" }

	$sujo = git status --porcelain
	if ($sujo) { throw "Há mudanças por commitar (inclusive o version.json, se ele acabou de mudar):`n$($sujo -join "`n")" }

	# Comparada como versão, e não como texto: "0.10.0" é MAIOR que "0.9.0".
	$noAr = ""
	try { $noAr = (Invoke-RestMethod "https://amongusaudiogame.z15.web.core.windows.net/version.json").version } catch { }
	if ($noAr) {
		if ([version]$versao -le [version]$noAr) { throw "O site já anuncia a $noAr; a $versao não seria oferecida a ninguém. Suba GAME_VERSION." }
		Write-Host "No ar: $noAr -> publicando $versao"
	} else {
		Write-Warning "Não consegui ler a versão do site; seguindo sem essa conferência."
	}

	# Servidor: compara com o commit que ESTÁ no ar (a etiqueta da imagem). O que não é tela, som,
	# texto do jogador ou ferramenta entra no servidor; na dúvida, publica.
	$imagem = kubectl -n $Namespace get deploy amongus-server -o jsonpath='{..image}' 2>$null
	$commitNoAr = if ($imagem -match ':([0-9a-f]{7,})$') { $Matches[1] } else { "" }
	$servidor = $true
	if ($commitNoAr) {
		$mudou = git diff --name-only $commitNoAr HEAD -- server_main.nvgt src lang |
			Where-Object { $_ -notmatch '^among/src/(ui|audio)/' -and $_ -notmatch '^src/(ui|audio)/' }
		$servidor = [bool]$mudou
		Write-Host ("Servidor no ar: {0}. {1}" -f $commitNoAr, $(if ($servidor) { "Mudou desde então: vai junto." } else { "Nada do servidor mudou: só o cliente." }))
	} else {
		Write-Warning "Não consegui ler o commit do servidor no ar; o servidor vai junto, por garantia."
	}

	# Som: regera se qualquer arquivo de sounds/ for mais novo que o pacote.
	$pacote = Get-Item sounds.dat -ErrorAction SilentlyContinue
	$somNovo = Get-ChildItem sounds -Recurse -File | Where-Object { -not $pacote -or $_.LastWriteTime -gt $pacote.LastWriteTime } | Select-Object -First 1
	if ($somNovo) { Write-Host "Som mais novo que o sounds.dat ($($somNovo.Name)): o pacote será regerado." }

	if ($Conferir) { Write-Host ""; Write-Host "Conferências ok. Nada compilado nem publicado (-Conferir)."; return }

	if ($somNovo) {
		Passo "sons"
		nvgt tools/build_pack.nvgt
		if ($LASTEXITCODE -ne 0) { throw "build_pack falhou." }
	}

	Passo "clientes"
	& tools\build_clients.ps1
	Passo "Android"
	& tools\build_android.ps1
	if ($servidor) {
		Passo "servidor"
		nvgt -c -plinux server_main.nvgt
		if ($LASTEXITCODE -ne 0) { throw "Build do servidor falhou." }
	}

	Passo "push"
	git push
	if ($LASTEXITCODE -ne 0) { throw "git push falhou." }

	Passo "deploy"
	if ($servidor) {
		& infra\deploy.ps1 -StorageAccount $StorageAccount -DrainSeconds $DrainSeconds
	} else {
		& infra\deploy.ps1 -StorageAccount $StorageAccount -SkipServer
	}

	Passo "conferindo no ar"
	$publicada = (Invoke-RestMethod "https://amongusaudiogame.z15.web.core.windows.net/version.json").version
	if ($publicada -ne $versao) { throw "O site anuncia $publicada, e não $versao." }
	Write-Host "Site: $publicada"
	if ($servidor) {
		$head = (git rev-parse --short HEAD).Trim()
		$imagem = kubectl -n $Namespace get deploy amongus-server -o jsonpath='{..image}'
		if ($imagem -notmatch [regex]::Escape(":$head")) { throw "O servidor roda $imagem, e não o commit $head." }
		Write-Host "Servidor: $imagem"
	}
	Write-Host ""
	Write-Host "$versao publicada."
}
finally {
	Pop-Location
}
