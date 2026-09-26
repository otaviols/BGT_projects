# Maquete da tarefa "calibrar o distribuidor", para OUVIR antes de existir código.
#
# A ideia: o ponteiro do mostrador gira, e você aperta quando ele passa pelo alvo.
#
# Duas pistas para a MESMA posição, de propósito: o clique caminha no estéreo (esquerda -> direita) e
# SOBE de tom junto. Quem não distingue bem o estéreo usa o tom; quem não distingue tom usa o estéreo.
# O alvo é apresentado antes de cada mostrador, com um "boop" no lugar e no tom dele.
#
# O clique é curto de propósito (20 ms): o ouvido localiza por ATAQUE, e zumbido não se localiza -
# foi a lição que custou uma tarefa inteira (ver notes/mapa-e-tarefas.md).
#
# COMO ela é montada, e por que não por amix: este ffmpeg quebra com "Cannot allocate memory" ao
# misturar entradas de 20 ms, e a opção `normalize` do amix não existe nesta build. Como os eventos
# NÃO se sobrepõem, cada um é gerado como um trecho (silêncio + som) e tudo é concatenado - o que de
# quebra evita o ganho que o amix divide.

$ErrorActionPreference = "Stop"
$ff = "D:\Program\winvox\ffmpeg.exe"
$lib = "D:\documents\sons among us\sounds among\Task Panels"
$tmp = Join-Path $PSScriptRoot "maq_partes"
$saida = Join-Path $PSScriptRoot "maquete_distribuidor.wav"

$clique = Join-Path $lib "panel_drillButton.wav"       # ponteiro (20 ms, puro ataque)
$boop   = Join-Path $lib "panel_boardingBoop.wav"      # o alvo, apresentado antes
$certo  = Join-Path $lib "panel_electrical_distributorcorrect1.wav"
$errado = Join-Path $lib "panel_electrical_distributorWrong.wav"

$POSICOES = 8
$eventos = @()

function Pan-De([int]$i) { return -1.0 + 2.0 * $i / ($POSICOES - 1) }
function Tom-De([int]$i) { return -5.0 + 10.0 * $i / ($POSICOES - 1) }  # +-5 semitons na volta

function Add-Evento($arquivo, [int]$ms, [double]$pan, [double]$semitons) {
	$script:eventos += [pscustomobject]@{ arquivo = $arquivo; ms = $ms; pan = $pan; semitons = $semitons }
}

function Add-Mostrador([int]$t0, [int]$alvo, [int]$periodoMs, [int]$voltas, [int]$erroNaVolta) {
	$panAlvo = Pan-De $alvo
	$tomAlvo = Tom-De $alvo
	Add-Evento $boop $t0 $panAlvo $tomAlvo
	Add-Evento $boop ($t0 + 600) $panAlvo $tomAlvo
	$t = $t0 + 1500
	$passo = [int]($periodoMs / $POSICOES)
	for ($v = 0; $v -lt $voltas; $v++) {
		for ($i = 0; $i -lt $POSICOES; $i++) {
			$quando = $t + $v * $periodoMs + $i * $passo
			Add-Evento $clique $quando (Pan-De $i) (Tom-De $i)
			if ($v -eq $erroNaVolta -and $i -eq ($alvo - 1)) {
				Add-Evento $errado ($quando + 60) 0.0 0.0
			}
			if ($v -eq ($voltas - 1) -and $i -eq $alvo) {
				Add-Evento $certo ($quando + 60) 0.0 0.0
				return ($quando + 2100)
			}
		}
	}
	return ($t + $voltas * $periodoMs + 400)
}

# Três mostradores, cada um mais rápido - como as três calibrações do original.
$t = 400
$t = Add-Mostrador $t 5 2400 2 -1          # devagar, acerta na segunda volta
$t = Add-Mostrador ($t + 700) 2 1800 3 1   # mais rápido: erra uma posição antes, e acerta na volta seguinte
$t = Add-Mostrador ($t + 700) 7 1200 2 -1  # rápido, alvo na ponta direita

# --- Gera um trecho por evento e concatena -------------------------------------------------------
if (Test-Path $tmp) { Remove-Item -Recurse -Force $tmp }
New-Item -ItemType Directory -Force $tmp | Out-Null

$ordenados = $eventos | Sort-Object ms
# A DURAÇÃO de cada som tem que ser descontada do intervalo seguinte. Sem isso o trecho de cada
# evento vale (intervalo + som), o compasso escorrega um pouco a cada clique e muito a cada som
# longo - o ponteiro chegaria ao alvo fora do tempo que a maquete queria mostrar.
$duracao = @{}
$duracao[$clique] = 20
$duracao[$boop] = 260
$duracao[$errado] = 250
$duracao[$certo] = 1720

$anterior = 0
$lista = @()
for ($k = 0; $k -lt $ordenados.Count; $k++) {
	$e = $ordenados[$k]
	# adelay=0|0 é RECUSADO pelo ffmpeg - o primeiro evento nunca fica em zero.
	$gap = [math]::Max(1, $e.ms - $anterior)
	$anterior = $e.ms + $duracao[$e.arquivo]
	$fator = [math]::Pow(2.0, $e.semitons / 12.0)
	$taxa = [int](48000 * $fator)
	$theta = ($e.pan + 1.0) * [math]::PI / 4.0
	$gl = [math]::Round([math]::Cos($theta), 4)
	$gr = [math]::Round([math]::Sin($theta), 4)
	$g = "aformat=sample_fmts=s16:sample_rates=48000:channel_layouts=mono,asetrate=$taxa,aresample=48000,pan=stereo|c0=$gl*c0|c1=$gr*c0,adelay=$gap|$gap"
	$parte = Join-Path $tmp ("p{0:d3}.wav" -f $k)
	& $ff -hide_banner -loglevel error -y -i $e.arquivo -af $g -c:a pcm_s16le -ar 48000 -ac 2 $parte
	if ($LASTEXITCODE -ne 0) { throw "falhou no evento $k" }
	$lista += "file '" + ($parte -replace "\\", "/") + "'"
}

$listaTxt = Join-Path $tmp "lista.txt"
Set-Content -Path $listaTxt -Value $lista -Encoding ASCII
& $ff -hide_banner -loglevel error -y -f concat -safe 0 -i $listaTxt -c:a pcm_s16le -ar 48000 -ac 2 $saida
if ($LASTEXITCODE -ne 0) { throw "falhou na concatenacao" }

"eventos: $($ordenados.Count)"
$o = & $ff -hide_banner -i $saida -af volumedetect -f null NUL 2>&1
([regex]::Match(($o -join " "), "Duration: 00:00:\d\d\.\d\d").Value)
([regex]::Match(($o -join " "), "mean_volume: -?\d+\.\d dB").Value)
([regex]::Match(($o -join " "), "max_volume: -?\d+\.\d dB").Value)
"arquivo: $saida"
