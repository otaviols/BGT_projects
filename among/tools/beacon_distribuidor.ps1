# Gera o beacon do distribuidor a partir dos sons originais do jogo.
#
#   tools\beacon_distribuidor.ps1
#
# Por que ele é GERADO e não só convertido, e por que isto vale um script: a primeira versão era o
# arquivo original inteiro (6,62 s de zumbido uniforme), baixado 13 dB para cair na MEDIANA DO NÍVEL
# MÉDIO dos outros beacons. O critério estava errado e o jogador ouviu na hora: "o som funciona, é que
# é muito baixo e não dá pra ouvir, principalmente no ambiente com vários sons".
#
# Média é energia, não presença. Um zumbido tem energia constante e nenhum ataque - então não se
# localiza e afunda na ambiência. É a MESMA lição de acústica que já havia custado uma tarefa inteira
# (ver notes/mapa-e-tarefas.md): o ouvido localiza por ataque.
#
# Os beacons que funcionam neste jogo são curtos e se repetem - `plants` tem 0,42 s, `vent` 1,09 s,
# `asteroid` 1,02 s. A repetição é o que cria ataque. Então este é um PULSO de 0,9 s: o clique do
# ponteiro (ataque puro) seguido do zumbido do distribuidor decaindo. Em loop soa "tec-vrum,
# tec-vrum": dá para apontar de onde vem, e sobrevive no meio de outros sons.
#
# O clique é o MESMO arquivo que o ponteiro usa dentro da task, de propósito - de longe é a máquina
# girando, de perto aquele mesmo tec vira o ponteiro. É o que o filtro de O2 já faz com o ar.
#
# Montado com o FILTRO `concat`, e não com o demuxer nem com `amix`: nesta build do ffmpeg o demuxer
# concat para depois do primeiro trecho quando ele tem 20 ms, e o amix quebra com "Cannot allocate
# memory" ao misturar entradas curtas.
#
# MONO de propósito: o beacon é posicionado em 3D pelo motor, e som estéreo não espacializa direito
# (a mesma armadilha do microfone no chat de voz). O beacon do filtro de O2 também é mono.

$ErrorActionPreference = "Stop"
$ff = "D:\Program\winvox\ffmpeg.exe"
$lib = "D:\documents\sons among us\sounds among\Task Panels"
$root = Split-Path -Parent $PSScriptRoot
$out = Join-Path $root "sounds\beacons\distributor_pannel.ogg"

# Nível final pretendido: médio perto de -20 dB, pico perto de -7. Isso o põe um pouco acima do
# `empty_garbage` (-21,5), que é o mais alto entre os beacons aceitos - o pulso já compra presença
# pela forma, então não precisa de mais. Se soar alto, o ajuste é aqui: mexa no volume geral no fim
# da cadeia, não nos dois pedaços separados.
$F = "aformat=sample_fmts=fltp:sample_rates=48000:channel_layouts=mono"
$graph = "[0:a]$F,volume=+1dB[a];" +
	# O trecho a partir de 3 s é onde o zumbido tem mais corpo. O decaimento começa em 0,30 s e dura
	# 0,56: antes disso é corpo, depois é cauda, e sobra uma folga curta antes da volta do loop.
	# Exponencial foi tentado e cortava o zumbido em 0,3 s - o pulso ficava magro (médio -28).
	"[1:a]atrim=start=3.0:end=3.88,asetpts=N/SR/TB,$F,volume=-1dB,afade=t=out:st=0.30:d=0.56[b];" +
	"[a][b]concat=n=2:v=0:a=1,volume=-2dB[out]"

& $ff -hide_banner -loglevel error -y `
	-i (Join-Path $lib "panel_drillButton.wav") `
	-i (Join-Path $lib "panel_electrical_Distributorspin.wav") `
	-filter_complex $graph -map "[out]" -c:a libvorbis -q:a 3 -ar 48000 -ac 1 $out
if ($LASTEXITCODE -ne 0) { throw "ffmpeg falhou" }

$o = & $ff -hide_banner -i $out -af volumedetect -f null NUL 2>&1
$mn = [regex]::Match(($o -join " "), "mean_volume: (-?\d+\.\d)").Groups[1].Value
$mx = [regex]::Match(($o -join " "), "max_volume: (-?\d+\.\d)").Groups[1].Value
"{0}: {1}  medio {2} dB  pico {3} dB  {4:N0} KB" -f (Split-Path -Leaf $out),
	[regex]::Match(($o -join " "), "Duration: 00:00:\d\d\.\d\d").Value, $mn, $mx, ((Get-Item $out).Length/1KB)
Write-Host "Regerou o beacon. Rode 'nvgt tools/build_pack.nvgt' antes de compilar, senao o jogo sai com o som antigo."
