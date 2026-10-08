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

	# Traduções ANTES de tudo, e nas conferências: uma tradução enviada pelo jogo para o build no meio
	# da publicação (a 0.50.0 parou assim), e o sync muda `lang/` - que é rastreado - e sujaria a árvore
	# que o deploy do servidor exige limpa. Aqui ela aparece no -Conferir, com tempo de promover.
	& infra\admin.ps1 traducoes | Out-Host
	$caixa = Get-ChildItem translations_inbox -Filter *.json -ErrorAction SilentlyContinue
	if ($caixa) { throw "Há $($caixa.Count) tradução(ões) em translations_inbox para revisar e promover (ver notes/traducoes.md)." }
	& tools\sync_translations.ps1 | Out-Host

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
		# lang/ fica de fora: o servidor manda CHAVES e quem traduz é o cliente (ver server_main.nvgt), e
		# só texto de idioma mudando reiniciava o servidor à toa - foi o que a 0.50.6 fez.
		$mudou = git diff --name-only $commitNoAr HEAD -- server_main.nvgt src |
			# Só do cliente: telas, som, e o laço da partida e de quem assiste (src/game/match/, game_loop,
			# spectate_loop) - o servidor não inclui nenhum deles. Sem isto, conserto só de cliente
			# reiniciava o servidor e derrubava quem estava jogando à toa.
			Where-Object { $_ -notmatch '^(among/)?src/(ui|audio|game/match)/' -and $_ -notmatch '^(among/)?src/game/(game_loop|spectate_loop)\.nvgt$' }
		# O GAME_VERSION muda em TODA versão, e sozinho não é motivo para reiniciar o servidor: se é a
		# única linha mexida no game_constants, ele sai da conta.
		$constantes = @($mudou | Where-Object { $_ -match 'src/config/game_constants\.nvgt$' })
		if ($constantes.Count -gt 0) {
			$linhas = git diff -U0 $commitNoAr HEAD -- $constantes[0] | Where-Object { $_ -match '^[+-][^+-]' }
			if (-not ($linhas | Where-Object { $_ -notmatch 'GAME_VERSION' })) {
				$mudou = @($mudou | Where-Object { $_ -notmatch 'src/config/game_constants\.nvgt$' })
			}
		}
		$servidor = [bool]$mudou
		Write-Host ("Servidor no ar: {0}. {1}" -f $commitNoAr, $(if ($servidor) { "Mudou desde então: vai junto." } else { "Nada do servidor mudou: só o cliente." }))
	} else {
		Write-Warning "Não consegui ler o commit do servidor no ar; o servidor vai junto, por garantia."
	}

	# Som: regera se qualquer arquivo de sounds/ for mais novo que o pacote.
	$pacote = Get-Item sounds.dat -ErrorAction SilentlyContinue
	$somNovo = Get-ChildItem sounds -Recurse -File | Where-Object { -not $pacote -or $_.LastWriteTime -gt $pacote.LastWriteTime } | Select-Object -First 1
	if ($somNovo) { Write-Host "Som mais novo que o sounds.dat ($($somNovo.Name)): o pacote será regerado." }
	else {
		# Sem som novo o pacote não é regerado - e um pacote estragado ia junto em toda versão: o da 0.50.0
		# à 0.50.2 tinha um byte trocado que deixava um passo mudo (ver tools/pack_check.nvgt). Confere sempre.
		nvgt tools/check_pack.nvgt
		if ($LASTEXITCODE -ne 0) { throw "O sounds.dat não confere com sounds/ (veja acima). Regere com: nvgt tools/build_pack.nvgt" }
	}

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

	# Backup do banco ANTES de tocar no servidor: é o único dado que não se recria, e a publicação é
	# o momento em que algo pode dar errado com ele. Também vale como lembrete regular - sem isto, o
	# primeiro backup só existiu depois de a assinatura do Azure cair (2026-10-07).
	Passo "backup do banco"
	& infra\admin.ps1 backup

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
	# Os DOIS sites: o novo, que a 0.51.1 em diante consulta, e o do Azure, que as versões até a 0.51.0
	# consultam - se um ficar para trás, parte dos jogadores não é avisada da atualização.
	foreach ($site in @("https://amongus.blindtabern.com/", "https://amongusaudiogame.z15.web.core.windows.net/")) {
		$publicada = (Invoke-RestMethod ($site + "version.json") -Headers @{ "Cache-Control" = "no-cache" }).version
		if ($publicada -ne $versao) { throw "O site $site anuncia $publicada, e não $versao." }
		Write-Host "Site $site : $publicada"
	}
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
