# Among Us — Audiogame

Jogo de dedução social multijogador **jogado inteiramente por som**, feito para leitor de tela e
áudio posicionado. Escrito em [NVGT](https://nvgt.gg).

Até 15 jogadores, servidor dedicado, contas persistentes, onze minigames de tarefa, sabotagens,
dutos, reuniões com votação e bots. **Em beta**, com jogadores reais.

**[Baixar o jogo](https://amongusaudiogame.z15.web.core.windows.net/)**

## Manuais

| | |
|---|---|
| [Manual do jogador (português)](docs/README_ptBR.md) | teclas, tarefas, sabotagens, como se vence |
| [Player manual (English)](docs/README_enUS.md) | same, in English |

Os dois vão junto com o jogo: quem baixa encontra `LEIAME.md` e `README.md` na pasta.

## Para quem for mexer no código

- [CLAUDE.md](CLAUDE.md) — como compilar, publicar, e as armadilhas do NVGT que já custaram caro aqui.
- [infra/README.md](infra/README.md) — o servidor, o site de download e como operá-los.
- [docs/README_enUS.md](docs/README_enUS.md#for-developers) — estrutura das pastas e comandos de build.

Resumo do essencial:

```
nvgt tools/build_pack.nvgt          # regera sounds.dat (obrigatório se algum som mudou)
nvgt -c AmongUs.nvgt                # cliente
nvgt -c -plinux server_main.nvgt    # servidor
infra\deploy.ps1 -StorageAccount amongusaudiogame
```
