# Mapa: The Hub

Layout simplificado inspirado no Skeld clássico. Definido em `map.nvgt`, função `build_the_hub_map()`.
Coordenadas em "unidades de mundo" (ver `PLAYER_MOVE_SPEED` em `config/game_constants.nvgt`), sem
rotação de personagem. Compartilhado entre cliente e servidor: o servidor usa pra validar posição e
alcance de interação, o cliente usa pra tocar os sons posicionados certos.

## Salas (11)

| Sala (id) | Nome (i18n) | Centro (x, y) | Metade largura/altura | Piso |
|---|---|---|---|---|
| `cafeteria` | Cafeteria | 0, 0 | 12 x 8 | Tile |
| `weapons` | Sala de armas | -30, 0 | 8 x 8 | Tile |
| `navigation` | Navegação | 35, 0 | 8 x 8 | MetalTile |
| `admin` | Administração | 0, -25 | 8 x 8 | Tile |
| `medbay` | Enfermaria | -30, -25 | 8 x 8 | Tile |
| `storage` | Armazém | 0, -50 | 10 x 8 | EarthTile |
| `electrical` | Elétrica | -30, -50 | 8 x 8 | MetalTile |
| `reactor` | Reator | -30, -75 | 8 x 8 | MetalTile |
| `games` | Sala de jogos | 0, 25 | 10 x 8 | Tile |
| `greenhouse` | Estufa | 35, 25 | 8 x 8 | EarthTile |
| `security` | Segurança | 25, -25 | 8 x 8 | MetalTile |

Cafeteria é o centro/hub: dela saem corredores pra weapons, navigation, admin e games. As salas mais
antigas formam uma cadeia vertical do lado esquerdo: weapons → medbay → electrical → reactor, com um
ramal de admin → storage → electrical.

A ala sul (games, greenhouse) fecha um **circuito**: cafeteria → games → greenhouse → navigation →
cafeteria. É a única parte do mapa onde se pode dar a volta sem refazer o caminho, o que muda como
perseguição e fuga funcionam por ali. Security é um ramal de admin, sem saída.

## Corredores (13)

Cada corredor liga duas salas adjacentes; as bordas dos retângulos encostam exatamente (sem gap),
então dá pra andar de uma sala pra outra sem "buraco" no meio.

| Corredor (id) | Liga | Centro (x, y) | Metade largura/altura |
|---|---|---|---|
| `corridor_cafeteria_weapons` | cafeteria ↔ weapons | -17, 0 | 5 x 3 |
| `corridor_cafeteria_navigation` | cafeteria ↔ navigation | 19.5, 0 | 7.5 x 3 |
| `corridor_cafeteria_admin` | cafeteria ↔ admin | 0, -12.5 | 3 x 4.5 |
| `corridor_weapons_medbay` | weapons ↔ medbay | -30, -12.5 | 3 x 4.5 |
| `corridor_admin_medbay` | admin ↔ medbay | -15, -25 | 7 x 3 |
| `corridor_admin_storage` | admin ↔ storage | 0, -37.5 | 3 x 4.5 |
| `corridor_medbay_electrical` | medbay ↔ electrical | -30, -37.5 | 3 x 4.5 |
| `corridor_storage_electrical` | storage ↔ electrical | -16, -50 | 6 x 3 |
| `corridor_electrical_reactor` | electrical ↔ reactor | -30, -62.5 | 3 x 4.5 |
| `corridor_cafeteria_games` | cafeteria ↔ games | 0, 12.5 | 3 x 4.5 |
| `corridor_navigation_greenhouse` | navigation ↔ greenhouse | 35, 12.5 | 3 x 4.5 |
| `corridor_games_greenhouse` | games ↔ greenhouse | 18.5, 25 | 8.5 x 3 |
| `corridor_admin_security` | admin ↔ security | 12.5, -25 | 4.5 x 3 |

O id do corredor **não é decorativo**: a IA dos bots monta o grafo de navegação a partir dele (ver
`zone_neighbors`), então um corredor fora da convenção `corridor_<sala_a>_<sala_b>` deixa os bots
parados sem nenhum erro visível.

Fora dessas 11 salas + 13 corredores não existe chão (`game_map.is_walkable()` retorna falso) - o
jogador toca `wall_bump.wav` e não se move ao tentar entrar nessa área.

## Objetos interativos

**Vents** (só o impostor usa): uma rede só, com as três bocas totalmente conectadas entre si. Elas
ficam nos três cantos do mapa, e nenhum par é ligado por corredor direto — é isso que faz do vent um
atalho impossível de fazer a pé. (Antes ficavam empilhadas na coluna oeste, paralelas ao corredor que
já ligava as três salas, e por isso não serviam pra quase nada.)

| Vent | Sala | Posição | Liga com |
|---|---|---|---|
| `vent_navigation` | navigation | 35, 6 | vent_weapons, vent_reactor |
| `vent_weapons` | weapons | -30, 6 | vent_navigation, vent_reactor |
| `vent_reactor` | reactor | -30, -71 | vent_navigation, vent_weapons |

**Tasks** (22 pontos, 11 tipos; cada jogador recebe um subconjunto aleatório - ver `config/lobby_config.nvgt`):

| Task | Sala | Posição | Tipo |
|---|---|---|---|
| `task_wiring_admin` | admin | 3, -22 | fix_wiring |
| `task_wiring_electrical` | electrical | -33, -47 | fix_wiring |
| `task_wiring_navigation` | navigation | 38, 3 | fix_wiring |
| `task_download_admin` | admin | -3, -28 | download_data |
| `task_download_navigation` | navigation | 32, -3 | download_data |
| `task_download_weapons` | weapons | -33, 3 | download_data |
| `task_garbage_cafeteria` | cafeteria | 5, 5 | empty_garbage |
| `task_garbage_storage` | storage | 3, -53 | empty_garbage |
| `task_align_navigation` | navigation | 35, -3 | align_engine |
| `task_card_admin` | admin | -3, -22 | swipe_card |
| `task_manifolds_reactor` | reactor | -33, -78 | unlock_manifolds |
| `task_manifolds_medbay` | medbay | -33, -28 | unlock_manifolds |
| `task_fuel_storage` | storage | -3, -53 | fuel_engines |
| `task_fuel_reactor` | reactor | -27, -78 | fuel_engines |
| `task_asteroids_weapons` | weapons | -27, -3 | clear_asteroids |
| `task_asteroids_navigation` | navigation | 32, 3 | clear_asteroids |
| `task_dice_games` | games | -4, 27 | roll_dice |
| `task_download_games` | games | 5, 22 | download_data |
| `task_plants_greenhouse` | greenhouse | 33, 27 | water_plants |
| `task_garbage_greenhouse` | greenhouse | 38, 22 | empty_garbage |
| `task_records_security` | security | 23, -27 | review_records |
| `task_wiring_security` | security | 28, -22 | fix_wiring |

Cada sala da ala sul tem a task exclusiva dela **e** uma task corriqueira: sala com um motivo só
pra existir vira sala que ninguém visita, e sala que ninguém visita não serve nem de esconderijo nem
de álibi.

**Tasks comuns** (`COMMON_TASK_TYPES` em `config/game_constants.nvgt`): hoje só `swipe_card`. Regra:
ou TODO tripulante recebe, ou NENHUM recebe — sorteado uma vez por partida, no `on_start_game`. Por
isso o cartão tem **um único ponto no mapa inteiro** (admin): é o que faz de "eu estava passando o
cartão" uma frase que a mesa consegue verificar contra a própria lista de tarefas.

**Painéis de sabotagem:**

| Painel | Sala | Posição | Sabotagem |
|---|---|---|---|
| `panel_lights_electrical` | electrical | -27, -53 | lights |
| `panel_oxygen_admin` | admin | 3, -22 | oxygen |
| `panel_oxygen_electrical` | electrical | -33, -53 | oxygen |

**Botão de emergência:** `emergency_button`, na cafeteria (0, 3).

## Ambiente sonoro

- **8 decorações de sala** (`ambient_<sala>`), uma no centro de cada sala, tocando
  `sounds/ambient_<sala>.wav` em volume constante dentro da sala inteira (não só perto do centro).
- **9 decorações de corredor** (`passage_<sala>_<sala>`), mesma técnica, todas tocando
  `sounds/passage.wav` (um arquivo só, compartilhado).
- Ambas usam `game_map.find_zone()` pra descobrir o tamanho real da sala/corredor e cobrir ela
  inteira (ver `audio_manager.start_room_ambience()`).

## Adicionando um mapa novo

A estrutura já é genérica (`map_zone`, `map_object`, `game_map`) - qualquer mapa novo só precisa de
uma função `game_map@ build_XXX_map()` que retorna zonas + objetos, do mesmo jeito que
`build_the_hub_map()` faz. O que ainda falta pra múltiplos mapas funcionarem de fato:

1. O servidor escolhe o mapa só uma vez, sempre `build_the_hub_map()`, em `on_create_lobby()`
   (`network/server.nvgt`). Precisaria de uma lista de mapas disponíveis e deixar o host escolher
   (ex: um campo a mais na tela de criar lobby).
2. **O cliente ignora completamente qual mapa o servidor está usando.** `run_game()`
   (`game/game_loop.nvgt`) sempre chama `build_the_hub_map()` direto, mesmo o servidor já mandando
   `map_name` dentro do pacote `S_GAME_START`. Enquanto só existir "the_hub" isso não dá problema,
   mas no dia que existir um segundo mapa, o cliente vai montar o mapa errado (paredes, tasks e
   posições diferentes das do servidor) sem avisar nada. Precisa de um registro tipo
   `game_map@ build_map_by_name(const string&in name)` que escolha a função certa a partir do
   `map_name` recebido.
