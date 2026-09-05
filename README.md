# NVGT_projects

Projetos de audiogames escritos em [NVGT](https://nvgt.gg) — uma engine baseada em AngelScript para
jogos feitos para serem jogados de ouvido. Este repositório serve de versionador e histórico do
desenvolvimento deles.

> O repositório ainda se chama `BGT_projects` por razões históricas: os projetos começaram em BGT
> (Blastbay Game Toolkit) e foram migrados para o NVGT, que é o sucessor espiritual dele.

## Projetos

### [among/](among/) — Among Us Audiogame

Jogo de dedução social multijogador, jogado inteiramente por som posicional e leitor de tela. Até 15
jogadores, com servidor dedicado, contas persistentes, onze minigames de tarefa, sabotagens, dutos,
reuniões com votação e bots.

**Estado: beta.** Jogável do começo ao fim e em testes.

- [Manual do jogador (português)](among/docs/README_ptBR.md)
- [Player manual (English)](among/docs/README_enUS.md)
- [Infraestrutura e publicação](among/infra/README.md)

## Como começar

Cada projeto tem o próprio README com as instruções de compilação. De modo geral é preciso ter o
NVGT instalado e disponível no `PATH`, e compilar com:

```
nvgt -c <projeto>.nvgt
```
