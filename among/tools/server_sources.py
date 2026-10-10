# server_sources.py
# Lista os arquivos do projeto que entram no SERVIDOR: server_main.nvgt e tudo o que ele inclui, seguindo os
# #include. Usado pelo release.ps1 para decidir se o servidor precisa ser republicado.
#
# Era uma lista de pastas "só do cliente" para EXCLUIR, e ela errava para o lado caro: um arquivo do cliente
# fora dela (src/config/game_settings.nvgt, na 0.51.7) fazia o release reiniciar o servidor à toa -
# derrubando quem jogava. Seguir os includes responde o que o servidor de fato compila.
#
#   python tools/server_sources.py          (da raiz do projeto: um caminho por linha, com /)
import os
import re
import sys

raiz = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
# Fora do código, mas dentro da imagem: mudou, o servidor muda.
extras = ["infra/Dockerfile", "infra/docker-entrypoint.sh"]

vistos = set()
pilha = [os.path.join(raiz, "server_main.nvgt")]
while pilha:
    f = os.path.normpath(pilha.pop())
    # Include que não está no projeto é da biblioteca do NVGT (speech.nvgt...): não é nosso.
    if f in vistos or not os.path.exists(f):
        continue
    vistos.add(f)
    with open(f, encoding="utf-8", errors="replace") as arq:
        for linha in arq:
            m = re.match(r'\s*#include\s+"([^"]+)"', linha)
            if m:
                pilha.append(os.path.join(os.path.dirname(f), m.group(1)))

if len(vistos) < 5:
    sys.exit("server_sources: achei so %d arquivo(s) - o server_main.nvgt mudou de lugar?" % len(vistos))
for f in sorted(vistos):
    print(os.path.relpath(f, raiz).replace("\\", "/"))
for e in extras:
    print(e)
