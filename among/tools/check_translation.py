#!/usr/bin/env python3
"""Confere uma tradução contra o en_US.json: o que falta, o que sobra, o que está vazio.

    python tools/check_translation.py lang/es_LATAM.json
    python tools/check_translation.py translations_inbox/es_LATAM__bauti.json

Sai com código 1 se o arquivo nem abrir ou não tiver language.name. Chaves faltando NÃO são erro:
o jogo cai no inglês para elas - mas a lista é o que se manda ao tradutor como resposta ao recado
("faltam estas"), então ela vem completa, em ordem.
"""

import io
import json
import sys
from pathlib import Path

RAIZ = Path(__file__).resolve().parent.parent
REFERENCIA = RAIZ / "lang" / "en_US.json"


def carregar(caminho: Path) -> dict:
    with io.open(caminho, encoding="utf-8-sig") as f:
        return json.load(f)


def main() -> int:
    if len(sys.argv) < 2:
        print(__doc__)
        return 1
    alvo = Path(sys.argv[1])
    try:
        traducao = carregar(alvo)
    except Exception as e:  # noqa: BLE001 - o que interessa é dizer que não abre, e por quê
        print(f"ERRO: {alvo} não é um JSON válido: {e}")
        return 1
    referencia = carregar(REFERENCIA)

    nome = traducao.get("language.name", "")
    tradutor = traducao.get("language.translator", "")
    print(f"{alvo.name}: {len(traducao)} chaves | language.name = {nome!r} | tradutor = {tradutor or '(não informado)'}")
    if not nome:
        print("ERRO: falta language.name - o jogo não consegue mostrar este idioma na lista.")
        return 1

    faltam = [k for k in referencia if k not in traducao]
    sobram = [k for k in traducao if k not in referencia and not k.startswith("language.")]
    vazias = [k for k, v in traducao.items() if isinstance(v, str) and v.strip() == ""]
    iguais = [k for k, v in traducao.items() if k in referencia and v == referencia[k] and len(v) > 12]

    print(f"faltam {len(faltam)} (o jogo usa o inglês nelas):")
    for k in faltam:
        print(f"  {k}: {referencia[k]!r}")
    if sobram:
        print(f"sobram {len(sobram)} (chaves que o jogo não conhece - provavelmente renomeadas):")
        for k in sobram:
            print(f"  {k}")
    if vazias:
        print(f"vazias {len(vazias)}:")
        for k in vazias:
            print(f"  {k}")
    if iguais:
        print(f"iguais ao inglês {len(iguais)} (talvez ainda por traduzir):")
        for k in iguais:
            print(f"  {k}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
