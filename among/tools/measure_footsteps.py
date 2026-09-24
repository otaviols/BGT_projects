#!/usr/bin/env python3
"""Mede o nivel dos passos de um piso e diz quanto de correcao ele precisa.

    python tools/measure_footsteps.py MetalTile GlassTile
    python tools/measure_footsteps.py --dir "D:\\documents\\sons among us\\sounds among\\Player\\Footsteps\\Glass" GlassTile

Existe porque a medicao e refeita a mao toda vez que um piso novo entra, e ela tem uma armadilha que
ja custou caro duas vezes: **RMS do arquivo inteiro subestima som curto**, porque o silencio do fim
entra na conta. Um passo tem 30 ms de batida e 200 ms de nada; medido inteiro, ele parece 10 dB mais
fraco do que soa. O numero que vale e o RMS do CORPO - so as amostras acima de 1% do pico.

A referencia do jogo e o MetalTile (ver footstep_volume_db em src/audio/sound_catalog.nvgt), e a
correcao sugerida e simplesmente a diferenca para ele. Ela e SUGESTAO: o Tile pediu +17,6 pela conta,
ficou alto demais no ouvido e voltou para +15. Meca, aplique, e depois confira de ouvido.
"""

import argparse
import array
import math
import os
import struct
import subprocess
import sys
from pathlib import Path

FFMPEG = r"D:\Program\winvox\ffmpeg.exe"
RAIZ = Path(__file__).resolve().parent.parent
PADRAO = RAIZ / "sounds" / "steps"
REFERENCIA = "MetalTile"
LIMIAR_DO_CORPO = 0.01  # 1% do pico


def amostras(caminho):
    """Decodifica para PCM 16 bits mono e devolve as amostras."""
    dados = subprocess.run(
        [FFMPEG, "-v", "quiet", "-i", str(caminho), "-f", "s16le", "-ac", "1", "-"],
        stdout=subprocess.PIPE, check=True).stdout
    vetor = array.array("h")
    vetor.frombytes(dados[:len(dados) - (len(dados) % 2)])
    return vetor


def db_do_corpo(caminho):
    """RMS em dBFS contando SO as amostras acima de 1% do pico (ver o cabecalho)."""
    v = amostras(caminho)
    if len(v) == 0:
        return None
    pico = max(abs(x) for x in v)
    if pico == 0:
        return None
    limiar = pico * LIMIAR_DO_CORPO
    corpo = [x for x in v if abs(x) >= limiar]
    if not corpo:
        return None
    soma = sum(float(x) * float(x) for x in corpo)
    rms = math.sqrt(soma / len(corpo))
    # O RUIDO: o que sobra fora do corpo. E ele que decide se um piso muito baixo pode ser
    # levantado - um passo que precisa de +19 dB so serve se o silencio entre as batidas for
    # silencio de verdade, senao o ganho sobe o chiado junto e o piso inteiro fica sujo.
    fundo = [x for x in v if abs(x) < limiar]
    ruido = -120.0
    if fundo:
        s2 = sum(float(x) * float(x) for x in fundo)
        r2 = math.sqrt(s2 / len(fundo))
        if r2 > 0:
            ruido = 20.0 * math.log10(r2 / 32768.0)
    return (20.0 * math.log10(rms / 32768.0), 20.0 * math.log10(pico / 32768.0),
            len(corpo) / float(len(v)), ruido)


def medir_piso(prefixo, pasta):
    arquivos = sorted(p for p in Path(pasta).iterdir()
                      if p.is_file() and p.stem.startswith(prefixo) and p.stem[len(prefixo):].isdigit())
    if not arquivos:
        return None
    linhas = []
    for a in arquivos:
        r = db_do_corpo(a)
        if r is None:
            continue
        linhas.append((a.name,) + r)
    if not linhas:
        return None
    # Media em energia, nao em dB: media aritmetica de dB pesa demais o arquivo fraco.
    energia = sum(10.0 ** (l[1] / 10.0) for l in linhas) / len(linhas)
    return 10.0 * math.log10(energia), linhas


def main():
    ap = argparse.ArgumentParser()
    ap.add_argument("prefixos", nargs="+", help="prefixo do piso, ex.: MetalTile GlassTile")
    ap.add_argument("--dir", default=None, help="pasta dos arquivos (padrao: sounds/steps)")
    ap.add_argument("--detalhe", action="store_true", help="mostra arquivo por arquivo")
    args = ap.parse_args()

    ref = medir_piso(REFERENCIA, PADRAO)
    if ref is None:
        print("ERRO: nao consegui medir a referencia %s em %s" % (REFERENCIA, PADRAO))
        return 1
    print("referencia %s: corpo %.1f dBFS (%d variantes)" % (REFERENCIA, ref[0], len(ref[1])))
    print("")

    for prefixo in args.prefixos:
        pasta = Path(args.dir) if args.dir else PADRAO
        r = medir_piso(prefixo, pasta)
        if r is None:
            print("%-14s nao achei variantes em %s" % (prefixo, pasta))
            continue
        corpo, linhas = r
        picos = max(l[2] for l in linhas)
        espalhamento = max(l[1] for l in linhas) - min(l[1] for l in linhas)
        ruido = max(l[4] for l in linhas)
        correcao = ref[0] - corpo
        print("%-14s corpo %6.1f dBFS  |  pico mais alto %6.1f dB  |  %d variantes  |  "
              "espalhamento %.1f dB  |  ruido de fundo %6.1f dBFS"
              % (prefixo, corpo, picos, len(linhas), espalhamento, ruido))
        print("%-14s correcao sugerida: %+.1f dB  ->  ruido iria para %.1f dBFS"
              % ("", correcao, ruido + correcao))
        # Os pisos que ja existem no jogo tem 7 a 12 dB de espalhamento entre variantes: e variacao
        # natural de gravacao, nao defeito. So vale avisar quando foge MUITO disso.
        if espalhamento > 14.0:
            print("%-14s ATENCAO: as variantes diferem %.1f dB entre si, bem mais que os pisos atuais "
                  "(7 a 12) - uma delas soa como passo mais longe, sem razao de jogo." % ("", espalhamento))
        # -60 dBFS de ruido e inaudivel debaixo de um passo; acima de -50 comeca a aparecer como
        # chiado quando o piso e levantado.
        if ruido + correcao > -50.0:
            print("%-14s ATENCAO: com essa correcao o ruido de fundo sobe para %.1f dBFS e passa a ser "
                  "audivel. Levante menos, ou escolha outro piso." % ("", ruido + correcao))
        if args.detalhe:
            for nome, c, p, frac, rn in linhas:
                print("    %-24s corpo %6.1f  pico %6.1f  ruido %6.1f  (corpo e %.0f%% do arquivo)"
                      % (nome, c, p, rn, frac * 100))
        print("")
    return 0


if __name__ == "__main__":
    sys.exit(main())
