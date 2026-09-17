#!/usr/bin/env python3
"""Gera infra/site/version.json a partir da primeira entrada dos changelogs.

Rode da raiz do projeto (a pasta among), depois de escrever a entrada nova:

    python tools/make_version_json.py

Por que existe
--------------
A novidade de uma versão era escrita em dois lugares - no changelog e no version.json - e duas
cópias da mesma frase acabam divergindo: alguém corrige uma e esquece a outra, e o jogador passa a
ver no jogo algo diferente do que está escrito no histórico. Aqui o changelog é a FONTE, e o
version.json é derivado dele.

O script também confere que a versão do changelog bate com GAME_VERSION no código. Elas precisam
andar juntas: se o version.json ficar para trás, ninguém é avisado da atualização; se ficar à
frente, o jogo anuncia uma versão que ele mesmo já é.
"""

import io
import json
import re
import sys
from pathlib import Path

# O console do Windows nem sempre aceita UTF-8; sem isto um travessão ou uma seta nas notas derruba o
# script DEPOIS de gravar o arquivo, e o erro parece ser do version.json.
sys.stdout = io.TextIOWrapper(sys.stdout.buffer, encoding="utf-8", errors="replace")

RAIZ = Path(__file__).resolve().parent.parent
CHANGELOGS = {
    "pt_BR": RAIZ / "docs" / "CHANGELOG_ptBR.md",
    "en_US": RAIZ / "docs" / "CHANGELOG_enUS.md",
}
CONSTANTES = RAIZ / "src" / "config" / "game_constants.nvgt"
SAIDA = RAIZ / "infra" / "site" / "version.json"


def primeira_entrada(caminho: Path):
    """Devolve (versão, texto) da primeira seção '## x.y.z' do arquivo."""
    texto = caminho.read_text(encoding="utf-8")
    secoes = re.split(r"^## (\d+\.\d+\.\d+)\s*$", texto, flags=re.MULTILINE)
    # split devolve [antes, versão, corpo, versão, corpo, ...]
    if len(secoes) < 3:
        sys.exit(f"ERRO: não achei nenhuma entrada '## x.y.z' em {caminho.name}.")
    return secoes[1], secoes[2].strip()


def para_uma_linha(corpo: str) -> str:
    """Transforma a entrada em um parágrafo só.

    O jogo FALA essas notas pelo leitor de tela, e marcação de lista ou negrito viram ruído no meio
    da frase - "asterisco asterisco Novo asterisco asterisco". Aqui a formatação é removida e os
    itens viram uma sequência de frases.
    """
    linhas = []
    for linha in corpo.splitlines():
        linha = linha.strip()
        if not linha:
            continue
        linha = re.sub(r"^[-*]\s+", "", linha)          # marcador de lista
        linha = re.sub(r"\*\*(.+?)\*\*", r"\1", linha)  # negrito
        linha = re.sub(r"`(.+?)`", r"\1", linha)        # código
        linha = re.sub(r"\[(.+?)\]\(.+?\)", r"\1", linha)  # link
        linhas.append(linha)
    texto = " ".join(linhas)
    texto = re.sub(r"\s+", " ", texto).strip()
    # garante ponto final: a frase é emendada com o resto do aviso falado
    if texto and texto[-1] not in ".!?":
        texto += "."
    return texto


def versao_do_codigo() -> str:
    m = re.search(r'GAME_VERSION\s*=\s*"([^"]+)"', CONSTANTES.read_text(encoding="utf-8"))
    if not m:
        sys.exit("ERRO: não achei GAME_VERSION em src/config/game_constants.nvgt.")
    return m.group(1)


def main() -> None:
    notas = {}
    versoes = set()
    for idioma, caminho in CHANGELOGS.items():
        if not caminho.exists():
            sys.exit(f"ERRO: não achei {caminho}.")
        versao, corpo = primeira_entrada(caminho)
        versoes.add(versao)
        notas[idioma] = para_uma_linha(corpo)

    if len(versoes) != 1:
        sys.exit(f"ERRO: os changelogs começam em versões diferentes: {sorted(versoes)}. "
                 "A entrada nova precisa ser escrita nos dois idiomas.")

    versao = versoes.pop()
    no_codigo = versao_do_codigo()
    if versao != no_codigo:
        sys.exit(f"ERRO: o changelog está na {versao} mas GAME_VERSION é {no_codigo}. "
                 "As duas precisam bater - senão o jogo anuncia uma versão errada, ou nenhuma.")

    # Dois formatos no mesmo arquivo, de propósito:
    #
    #   notes             - texto único, em inglês. É o que os clientes até a 0.17.0 leem; eles
    #                       esperam uma string e quebrariam se achassem um objeto no lugar.
    #   notes_by_language - um texto por idioma, lido pelos clientes novos.
    #
    # O campo antigo pode sair quando não houver mais ninguém nessas versões.
    conteudo = {
        "version": versao,
        "url": "AmongUs.zip",
        "notes": notas.get("en_US", ""),
        "notes_by_language": notas,
    }
    SAIDA.write_text(json.dumps(conteudo, ensure_ascii=False, indent="\t") + "\n",
                     encoding="utf-8")
    print(f"version.json gerado para a {versao}:")
    for idioma, texto in notas.items():
        print(f"  {idioma}: {texto[:90]}{'...' if len(texto) > 90 else ''}")


if __name__ == "__main__":
    main()
