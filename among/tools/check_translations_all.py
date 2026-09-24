#!/usr/bin/env python3
"""Confere TODOS os idiomas de uma vez e devolve código de saída - é o portão do deploy.

    python tools/check_translations_all.py

Existe porque `check_translation.py` confere um arquivo e sempre sai com 0: ele é a resposta que se
manda ao tradutor ("faltam estas chaves"), não um teste. Este aqui é o teste.

O que é ERRO (sai com 1) e o que é só aviso vem de quem MANTÉM cada idioma:

  - `en_US` e `pt_BR` são EMBUTIDOS, mantidos junto do código: toda chave nova nasce nos dois. Um
    deles fora de sincronia com o outro é defeito nosso, e defeito que só aparece para o jogador -
    ele ouve a chave crua ou o texto em inglês no meio de uma frase em português. É ERRO.
  - os outros idiomas são da COMUNIDADE e vêm quando vêm. Chave faltando neles cai no inglês, que é
    o comportamento desenhado. É AVISO, com a contagem, para saber a quem responder.
  - envio de jogador parado em `translations_inbox/` é ERRO: foi exatamente assim que a 0.32.0 subiu
    com um espanhol COMPLETO esperando na caixa. Processe (ver CLAUDE.md, "Como uma tradução chega")
    ou mova para `translations_inbox/processadas/`.

O deploy chama isto antes de qualquer outra coisa - antes de compilar imagem, antes de avisar
jogador -, para a recusa custar zero.
"""

import io
import json
import sys
from pathlib import Path

RAIZ = Path(__file__).resolve().parent.parent
LANG = RAIZ / "lang"
INBOX = RAIZ / "translations_inbox"
REFERENCIA = LANG / "en_US.json"

# Mantidos aqui, junto do código. Ver CLAUDE.md, seção Traduções.
EMBUTIDOS = {"pt_BR"}


def carregar(caminho):
    with io.open(caminho, encoding="utf-8-sig") as f:
        return json.load(f)


def main():
    if not REFERENCIA.exists():
        print("ERRO: nao achei %s" % REFERENCIA)
        return 1
    try:
        ref = carregar(REFERENCIA)
    except Exception as e:
        print("ERRO: %s nao abre: %s" % (REFERENCIA.name, e))
        return 1
    chaves_ref = set(ref.keys())

    erros = []
    for arquivo in sorted(LANG.glob("*.json")):
        codigo = arquivo.stem
        if codigo == "en_US":
            continue
        try:
            dados = carregar(arquivo)
        except Exception as e:
            erros.append("%s nao abre: %s" % (arquivo.name, e))
            continue
        chaves = set(dados.keys())
        faltam = chaves_ref - chaves
        # Chave que existe SÓ aqui: alguém acrescentou texto num idioma e esqueceu do inglês. No
        # embutido isso é tão defeito quanto faltar - o inglês é a referência de todo mundo, então a
        # chave órfã nunca chega a nenhum outro idioma.
        sobram = chaves - chaves_ref

        if codigo in EMBUTIDOS:
            if faltam:
                erros.append("%s (embutido) esta sem %d chave(s) que o ingles tem: %s"
                             % (codigo, len(faltam), ", ".join(sorted(faltam)[:8])))
            if sobram:
                erros.append("%s (embutido) tem %d chave(s) que o ingles nao tem: %s"
                             % (codigo, len(sobram), ", ".join(sorted(sobram)[:8])))
            if not faltam and not sobram:
                print("ok    %s (embutido): %d chaves, em dia com o ingles" % (codigo, len(chaves)))
        else:
            quanto = 100.0 * (len(chaves_ref) - len(faltam)) / max(1, len(chaves_ref))
            estado = "ok   " if not faltam else "aviso"
            print("%s %s (comunidade): %d de %d chaves (%.0f%%)%s"
                  % (estado, codigo, len(chaves_ref) - len(faltam), len(chaves_ref), quanto,
                     "" if not faltam else " - %d faltando, caem no ingles" % len(faltam)))
            if sobram:
                print("      (%d chave(s) que o ingles nao tem mais - sobra de versao antiga)" % len(sobram))

    pendentes = sorted(INBOX.glob("*.json")) if INBOX.exists() else []
    if pendentes:
        erros.append("ha %d traducao(oes) enviada(s) por jogador esperando revisao em "
                     "translations_inbox/: %s" % (len(pendentes), ", ".join(p.name for p in pendentes)))

    if erros:
        print("")
        for e in erros:
            print("ERRO: %s" % e)
        return 1
    return 0


if __name__ == "__main__":
    sys.exit(main())
