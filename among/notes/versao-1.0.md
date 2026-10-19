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

## A Base Polar hoje (2026-10-19)

**Chamava-se Polus até 2026-10-19.** O nome é nosso, por dentro e por fora (`MAP_POLAR_BASE`,
`"polar_base"`, os sons `polar_base_*`), porque o mapa é INSPIRADO no Polus do original e não uma cópia -
como a nave de sempre, que é "A nave" e não "The Skeld" (decisão do usuário, que também lembrou que
"Among Us" é marca). A seção "Polus" mais acima fala do mapa ORIGINAL, da leva; a pasta de teste do usuário
ainda se chama "teste do Polus".

**Planta aprovada e montada** (`build_polar_base_map`): nave de pouso (começo e as chaves), escritório (mesa e botão: a reunião),
administração, comunicações, armas, O2, elétrica, depósito, sismógrafo, laboratório, descontaminação e a
sala dos espécimes atrás dela - ligados por nove caminhos de NEVE (o lado de fora, `SnowTile` a +7,5 dB,
com o vento `polar_base_outside` em cada caminho) e dois corredores internos. Duas redes de duto. Ambientes da
leva convertidos no volume das salas. **Em construção:** fica em `MAP_IDS_IN_TEST` - o servidor não o
aceita, e o Conhecer o mapa e o treino o oferecem quando a pasta do jogo tem o `mapas_em_teste.txt` (com
algum texto dentro). É como o usuário ouve o mapa antes de ele existir para os jogadores.

**O que ela tem (tudo feito em 2026-10-19), e as decisões que alguém poderia querer desfazer:**
- **Nove tarefas próprias**, uma por vez pela receita (maquete ouvida antes de código; as maquetes ficam
  na pasta de teste do usuário, `C:\apto10\acer\documents\Among Us - teste do Polus`):
  - **o nó do clima** (seis nós nos caminhos de neve + o painel no laboratório; ver tarefas-novas.md);
  - **trocar o galão** (depósito ao escritório: bater nos galões e pegar o cheio pelo TIMBRE, encaixar no
    bebedouro; a versão por tom, "manter a vazão", foi recusada: o jogo não pode ter tom em tudo);
  - **reiniciar o Wi-Fi** (comunicações: desce a alavanca segurando a seta, sai, volta depois de 45 s - a
    reunião não pausa a conta, como no original; DOIS pontos no mesmo lugar, porque a corrente não aceita
    o mesmo ponto duas vezes);
  - **registrar a temperatura** e **alinhar o telescópio** (laboratório, por tom e lado);
  - **inserir as chaves**, a tarefa COMUM (um ponto só, na nave de pouso, como o cartão na nave;
    `COMMON_TASK_TYPES` sorteia por tipo, e o tipo sem ponto no mapa não sai);
  - **guardar os artefatos** (espécimes: cada buraco, ao se CHEGAR nele, toca a peça que encaixa ali;
    desenho do usuário, que trocou no mesmo dia duas versões minhas);
  - **encher os cilindros de O2** (desenho do usuário: prateleira e máquina em lados sorteados, Enter faz
    a coisa do lugar na ordem; duas rodadas, longa);
  - **abrir a roda d'água** (lá fora: girar com as setas em roda; o "emperrar" foi oferecido e ficou de fora).
- **Da nave, ficaram as que fazem sentido lá:** fios, download, asteroides (as armas do original têm) e o
  filtro de O2 saiu (abaixo). E o **exame médico**, a
  tarefa visual, no laboratório como no original - o usuário: tarefa vai onde faz sentido, sala cheia não
  é problema, o sorteio já limita as visuais por partida.
- **O sismógrafo**, a crítica: duas mãos juntas nos dois painéis da sala dele, 90 s (ver
  ciclo-da-partida.md). **O reator** na nave de sempre, a mesma mecânica - a nave aceita cliente antigo,
  então o servidor não o dispara com alguém abaixo de `HOLD_SABOTAGE_PROTOCOL` na partida. Já está no
  manual (a nave é o mapa publicado); falta só o changelog.
- **Portas com painel:** a sala trancada fica 20 s (12 na nave), e na passagem Enter abre o minijogo de
  reflexo. Os sons do minijogo da leva foram RECUSADOS ("bem estranhos", feitos para animação): bipes
  gerados e o interruptor que estava reservado.
- **A descontaminação:** quem entra TRANCA a câmara por 5 s, com o spray; quem chega depois espera a
  rodada dele (o usuário: ficar preso 1 s pela rodada de outro não faria sentido). Várias pessoas juntas,
  e pode matar lá dentro (escolha dele, como no original). Só funciona pelo servidor - o usuário dispensou
  simular no Conhecer o mapa.
- **Câmeras** na administração (no original há sala de segurança própria). O painel de comunicações fica
  nas comunicações, como no original (estava na administração por engano da planta).
- **As reuniões e o botão** ficam no escritório, como no original; começa-se na nave de pouso, onde
  também ficam as chaves (no original é ligar a nave em que se chegou). A planta primeira juntava começo
  e reunião na nave de pouso, e o usuário as separou (2026-10-19).
- **Sem o filtro de O2** (veio da nave e não se sabe se o original o tem; o usuário o tirou).
- **Portas só em sete salas**, como no original (`lockable_rooms`): comunicações, elétrica, laboratório,
  escritório, O2, depósito e armas.

**Como testar antes de lançar, sem publicar nada:** um servidor de TESTE com o arquivo
`mapas_em_teste.txt` ao lado dele, que o faz aceitar os mapas em construção (`maps_in_test_enabled`; o
oficial, sem o arquivo, continua recusando). O local está montado em
`C:\apto10\acer\documents\Among Us - servidor de teste` (o servidor de Windows, `nvgt -c -pwindows
server_main.nvgt`, com dois atalhos para ligar e desligar - ele roda sem janela, e o banco de contas é dele),
e a pasta de teste do usuário aponta para ele com um `server.txt` ("127.0.0.1"; apagar volta ao oficial).
Ali o usuário joga sozinho com bots, ou com dois jogos abertos. Para testar COM GENTE, a ideia combinada
(2026-10-19) é um segundo servidor na VPS, numa porta separada, com o mesmo arquivo - ainda por montar.

**Falta antes de lançar:**
- liberar o mapa (`MAP_IDS_IN_TEST` -> `MAP_IDS`). O aprender os sons JÁ é por mapa (pergunta o mapa,
  e a parte do mapa sai do próprio mapa - `learn_sounds_collection`);
- uma **partida de teste com gente**: sismógrafo, reator, portas e descontaminação só existem pelo servidor;
- a seção da Base Polar no **manual** (`docs/README_*`), o **changelog** e a versão.

**As sabotagens agora são do MAPA** (`game_map.sabotages()`, as que têm painel): o menu do impostor, o
servidor (`trigger_sabotage`), os bots e o treino perguntam ao mapa. Sem isso a Base Polar ofereceria o
oxigênio, que não tem painel lá - sabotagem sem conserto, vitória de graça. **O som geral também**
(`game_map.ship_ambience`): o zumbido da nave é da nave; a Base Polar não tem um. **E a sala de começo** é
dita à parte no Conhecer o mapa (o `explore.intro` deixou de dizer "cafeteria", nos cinco idiomas).

`probe_maps` passa na Base Polar: além do que já conferia, agora pega zona sobre zona, objeto fora da sala
dele, zona inalcançável pelos bots e ligação com buraco no meio - os quatro erros que uma planta nova
comete calada.

## Como um mapa novo entra

1. `build_<mapa>_map()` em `game/map.nvgt` com `name`, `start_room`, `spawn_*`, `gather_*`,
   `ship_ambience`, salas, corredores, objetos; uma linha em `build_map`; o id em `MAP_IDS_IN_TEST`
   enquanto se constrói e em `MAP_IDS` quando sair; `map.<id>.name` em pt_BR e en_US.
2. `probe_maps` passando.
3. O **aprender os sons** monta a parte do mapa a partir dele (pisos, ambientes, marcadores, câmara):
   nada a fazer, a não ser o rótulo de um PISO novo (`footstep_learn_key`) - a sonda acusa rótulo cru.
4. Os **bots** já rodam no mapa novo (a `probe_seismic` os põe nos painéis da Base Polar); o **modo
   gráfico** lê o mapa, mas só foi olhado na nave de sempre.
## O que o mapa novo exigiu do jogo (FEITO; fica como registro do porquê)

- **Escolher o mapa na sala:** a lista de mapas, o campo no formulário, o servidor montando o mapa da
  sala e o cliente o mesmo (`S_GAME_START`) - com protocolo novo (`MAPS_PROTOCOL`), porque um cliente
  antigo jogaria a nave errada.
- **Tudo que assumia a nave de sempre** passou a perguntar ao mapa: Conhecer o mapa, o treino, os bots,
  o modo gráfico, as câmeras, a reunião (o ponto de encontro) e as sondas que montam o mapa. O aprender
  os sons também (2026-10-19).
- **Pisos novos** entram em DUAS tabelas, e o rótulo no aprender os sons (ver som.md: "piso novo entra
  em DUAS tabelas").
