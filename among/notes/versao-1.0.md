# Versão 1.0: um mapa novo

O plano da 1.0 e o que a leva nova de sons oferece. Leia antes de mexer em mapa novo, tarefa nova da 1.0
ou sabotagem nova.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## O que foi decidido (2026-10-19)

- **A 1.0 é UM mapa novo**, com tarefas e sabotagens novas. "As pessoas estão ficando meio cansadas do
  mesmo" (palavras do usuário). Quatro mapas foram considerados e descartados para a 1.0: cada mapa é
  salas, ambientes, pisos, umas oito tarefas com maquete cada e sabotagens próprias - meses de trabalho.
- **Passos do jogo original NÃO.** Já foram testados: são baixos demais para um jogo em que o passo é a
  pista principal. O usuário tem outra fonte de passos; os `Footstep*` da leva ficam de fora.
- **Papéis novos NÃO entram na 1.0**, e quando entrarem são nossos: o original pode inspirar, não ser
  copiado. Os `RoleReveal_*`, `role_*`, `viper_*`, `Judge*`, `detective*` da leva ficam de fora.
- **Tarefas novas seguem a receita** de [tarefas-novas.md](tarefas-novas.md): a original do mapa,
  traduzida para o ouvido, com maquete em ffmpeg OUVIDA pelo usuário antes de existir código.

## A leva

`C:\apto10\acer\documents\sons among us\sounds among\adicionais` (a biblioteca do usuário, fora do
repositório): 493 `.wav`, 406 MB junto com o resto da pasta. Os nomes NÃO dizem o mapa; a atribuição
abaixo é pelo jogo original e precisa ser confirmada OUVINDO. Arquivos com ` #número` no nome são
versões alternativas do mesmo som.

### Polus (a sugestão para a 1.0)

- **Ambientes:** `AMB_Exterior`, `AMB_Weather`, `AMB_Outside` (três versões), `AMB_Specimen`,
  `AMB_Laboratory`, `AMB_LabHallway`, `AMB_DecontaminationHall`, `AMB_O2Room`, `AMB_Seismic`, `AMB_Drill`,
  `AMB_Electrical`, `AMB_Communications`.
- **Tarefas:** alinhar o telescópio (`panel_telescopeScan`), consertar o nó do clima
  (`panel_weatherNodeMove`, `Panel_weatherstart`, `panel_weatherSwitch`), inserir as chaves
  (`panel_key*`), abrir a roda d'água (`panel_waterwheel*`), trocar o galão de água (`panel_waterfill`),
  registrar a temperatura (`Panel_TempButton`), reiniciar o Wi-Fi (`panel_Wifi*`), escanear o cartão de
  embarque (`panel_boarding*`), guardar artefatos (`panel_specimen*`), encher cilindros de O2 (`panel_O2*`).
- **Sabotagens:** o sismógrafo (`AMB_Seismic`, dois painéis com duas pessoas, como o reator) e as portas
  com minijogo (`sabotage_doorgame_*`, `sabotage_door_*`).
- **Mundo:** descontaminação (`Decontamination_*`). E a neve: o `SnowTile` do jogo já espera uma sala
  de neve, e os passos dela vêm da fonte do usuário.
- Por que ela: o contraste entre dentro e fora é o melhor para jogar de ouvido - vento e neve lá fora,
  piso e laboratório lá dentro, e o próprio passo diz onde se está.

### MIRA HQ

- **Ambientes:** `AMB_Launchpad`, `AMB_Lockerroom`, `AMB_DecontaminationHall`, `AMB_Laboratory`,
  `AMB_CommsRoom`, `AMB_Hallways`.
- **Tarefas:** comprar bebida (`panel_beverage*`), rodar diagnóstico (`panel_LaunchpadDiagnose*`), separar
  amostras (`panel_sortsample*`), cartão de identidade (`panel_admin_walletout`), plantas
  (`panel_storagePlant*`, `panel_storageWaterplant`), desviar energia (`panel_divertpower_switch`).
- **Sabotagens:** reator (`panel_reactor*`), mais as que o jogo já tem.

### Airship

- **Ambientes:** `AMB_Cockpit`, `AMB_Kitchen`, `AMB_Main`, `AMB_Gaproom`, `AMB_Shower`, `AMB_Bathroom`,
  `AMB_Ventillation`, `AMB_EngineSteam`, `AMB_EngineShock`, `AMB_Electricshock1-4`.
- **Tarefas:** hambúrguer (`panel_burger_*`), polir o rubi (`panel_ruby*`), cofre (`panel_safe*`),
  arquivo (`panel_records*`), armas (`panel_gun*`), manequim (`panel_dressup*`), toalhas (`panel_towel*`),
  privada (`panel_toilet*`), chuveiro (`panel_shower*`), ventiladores (`panel_fans*`), fitas (`panel_vcr*`),
  montar (`panel_reassemble*`).
- **Mundo:** escadas (`ladder_*`), plataforma (`Platform_Moveacross`), portas próprias (`Door_balcony*`,
  `door_bathroom*`).

### The Fungle

- **Ambientes:** `AMB_Beachclose`, `AMB_Beachfar`, `AMB_Jungle`, `AMB_Campfire`, `AMB_Mining`,
  `AMB_Treeroom`, `AMB_Sleepingquarters`, `AMB_Water`, `AMB_Pods`, `AMB_comms`, `AMB_lavapit`.
- **Tarefas (a maior leva):** pescar e grelhar (`task_fish*`), minerar (`task_mine_*`), manivela
  (`task_crank_*`), filhote (`task_critter_*`), assar marshmallow (`task_roast_*`), horta
  (`task_vegetable_*`), peças (`task_parts_*`), polia (`task_pulley_*`), frisbee (`task_frisbee_*`),
  castelo de areia (`task_sandcastle_*`), conchas (`task_shells_*`), amostras (`task_samples_*`),
  pesos (`task_weights_*`).
- **Sabotagem:** a dos cogumelos (`mushroommixup_*`, `sporemushroom_*`).
- **Mundo:** tirolesa (`zipline_*`), escadas, dutos no chão (`ventGround*`, `ventMoveGround*`).

### Sem mapa

`eject_*` (a expulsão de cada mapa), `victory_*`, `report_*`, `HnS_*` (o modo esconde-esconde),
`matchmaking_*`, `endgame_*`. Nada disso é pedido da 1.0.

## Vários mapas: o que já existe (2026-10-19)

- **A lista é `MAP_IDS`** (`game_constants.nvgt`), e o primeiro é o padrão. **Quem monta é `build_map(id)`**
  (`game/map.nvgt`); id desconhecido monta o padrão. A sala guarda `map_name`, o servidor recusa id fora
  da lista (`lobby_config.validate`) e monta o da sala ao criar e ao editar. O cliente monta o que vem no
  `S_GAME_START` e no `S_SPECTATE_START` (`map_name`).
- **O mapa diz o que era constante da nave:** `spawn_x/spawn_y` (nasce-se num raio de 3 em volta),
  `gather_x/gather_y` (a mesa da reunião - era `MEETING_GATHER_X/Y`, que não existe mais) e `start_room`
  (a primeira sala das câmeras e por onde o Conhecer o mapa começa).
- **Protocolo 15 (`MAPS_PROTOCOL`):** sala de outro mapa exige cliente novo (`required_protocol`); o antigo
  montaria a nave de sempre e andaria por paredes que não existem.
- **Telas:** o campo "Mapa" no formulário da sala e a linha "mapa:" nas regras só aparecem com MAIS DE UM
  mapa na lista. O treino e o Conhecer o mapa perguntam o mapa (`choose_map`, `ui/map_choice.nvgt`) - com
  um só, não perguntam. O treino lista as tarefas e os reparos que EXISTEM no mapa escolhido, na ordem do
  manual (`practice_task_types`, `practice_sabotages`).
- **Sonda-portão: `probe_maps.nvgt`.** Para todo mapa da lista: nome falado, sala de começo, ponto de
  nascer dentro dela com o raio inteiro em chão, mesa num lugar onde se pisa, e toda tarefa com minijogo
  (`TASK_MINIGAME_TYPES`, em `task_manager.nvgt` - tipo novo entra na lista E no `else if`).

## Como um mapa novo entra

1. `build_<mapa>_map()` em `game/map.nvgt` com `name`, `start_room`, `spawn_*`, `gather_*`, salas,
   corredores, objetos; uma linha em `build_map`; o id em `MAP_IDS`; `map.<id>.name` em pt_BR e en_US.
2. `probe_maps` passando.
3. O que ainda é da nave de sempre e precisa virar do mapa quando Polus chegar:
   - **o ambiente geral** (`audio.start_ship_ambience`, um som só para a nave inteira) - Polus tem o
     exterior com vento e neve, que é outra coisa;
   - **o texto `explore.intro`**, que diz "Você está na cafeteria" - vira `{room}`, e como é chave que
     muda de texto, muda nos cinco idiomas (regra das traduções);
   - **aprender os sons**, que talvez se divida por mapa (os ambientes e os marcadores de tarefa de cada
     um) - pedido do usuário, a decidir junto com os sons de Polus;
   - **os bots e o modo gráfico** já leem o mapa, mas só foram testados na nave de sempre.
## O que o mapa novo exigia do jogo (feito acima, menos o que está em "Como um mapa novo entra")

- **Escolher o mapa na sala.** Hoje o mapa é um só, construído em código (`build_the_hub_map`), e
  `map_name` já existe na configuração. Precisa: a lista de mapas, o campo no formulário, o servidor
  construir o mapa da sala, o cliente o mesmo (está no `S_GAME_START`) - e protocolo novo, porque um
  cliente antigo jogaria a nave errada.
- **Tudo que hoje assume a nave atual:** "Conhecer o mapa", o treino de tarefas, o aprender os sons, os
  bots (rotas), o modo gráfico, as câmeras (salas da nave), a reunião (o ponto de encontro) e as sondas
  que montam o mapa.
- **Pisos novos** entram em DUAS tabelas (ver som.md: "piso novo entra em DUAS tabelas").
