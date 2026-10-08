# Papéis (profissões)

Cada papel, o que ele declara e a razão de cada regra. Leia antes de criar um papel ou mexer numa habilidade.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## Papéis (profissões)

**O papel do jogador tem UMA fonte, e `is_impostor` é derivado dela.** `game_player.role_id` é o
que vale; `set_role()` recalcula `is_impostor` a partir do time do papel. Os dois nunca são
escritos separados - é o que impede um papel de impostor (shapeshifter) de contar como tripulante
na vitória, ou um papel de tripulação de virar alvo proibido de kill. Nada deve atribuir
`is_impostor` direto; quem fizer isso deixa os dois discordando sem que nada acuse (a sonda
`probe_ghost_sabotage` já foi corrigida por causa disso). A sala guarda os papéis em
`lobby_config.role_counts` (id -> quantos, **vazio por padrão em todo preset**) e
`role_chances` (chance de CADA vaga sair, 100 por padrão), o sorteio inteiro mora em
`draw_special_roles` - chamada pelo servidor E pela sonda, porque reimplementá-la na sonda passaria
com o servidor quebrado -, e a habilidade ativa é um pacote só
(`C_USE_ABILITY` -> `run_ability`, um caso por id) em vez de um pacote por papel. Sala com papel
ligado exige `ROLES_MIN_PROTOCOL` e recusa cliente velho na PORTA (entrar) e no INÍCIO (quem já
estava dentro), dizendo quem precisa atualizar. Sonda: `tools/probes/probe_roles.nvgt`.

**Número de papel que a sala ajusta: o PAPEL declara, e o servidor pergunta.** `role_traits.tunables`
lista recarga, duração e janela de gravação com o padrão e os LIMITES (que moram no papel porque só
ele sabe o que é absurdo no caso dele - recarga do engenheiro abaixo da recarga da sabotagem faz
sabotar deixar de ser jogada). A sala guarda em `role_values` **só o que foi mexido**, então mudar
um padrão no código vale para todas as salas antigas em vez de ficar congelado na criação. Nada
disso toca o protocolo: um papel novo com um número novo custa uma linha na declaração e uma chave
de texto - o formulário, a validação, a leitura das regras e o `to_json` percorrem a lista.

O servidor lê por `config.role_value(papel, chave)`, **nunca a constante**; a constante virou só o
padrão. E o cliente recebe a duração de volta no `S_ABILITY_RESULT` (`duration_seconds`), porque ele
acompanha a fantasia e a invisibilidade localmente e não conhece a configuração da sala.

**Preset de sala salvo = `lobby_overrides` + o preset-base, no MESMO formato do protocolo.** Os
ajustes são serializados por `write_to_packet`/`read_from_packet` (`config/lobby_presets.nvgt`), e
não por uma serialização própria: assim um campo novo na sala entra no preset sozinho, sem uma
segunda lista para esquecer de atualizar. Ficam na máquina do jogador (`game_settings.lobby_presets`,
junto das teclas e volumes) - não dependem de conta nem de banco, e o preço assumido é que trocar de
computador os perde. **Nome da sala e código de acesso não entram**: o nome se quer variar, e um
código salvo seria um código que nunca muda. A última usada é gravada sozinha sob o nome VAZIO, que
o formulário recusa - por isso ela convive com as nomeadas sem poder ser sobrescrita. E a leitura
fica em try/catch: o arquivo é do jogador, pode estar truncado, e `parse_json` LANÇA. Sonda:
`tools/probes/probe_presets.nvgt` (a ida e volta é o que falha calado - um campo que não sobrevive
vira uma sala com regra diferente da escolhida, sem nada acusando).

**A sala guarda a INTENÇÃO; quem lida com a realidade é o sorteio.** `validate()` NÃO recorta as
vagas por time nem por quanta gente veio - só pelo absurdo (mais vagas do que a lotação). Recortar
ali reescrevia em silêncio o que o anfitrião digitou, e de um jeito que não voltava: baixar a
lotação apagava os papéis, e subi-la de volta não os trazia. Quem sabe quantos vieram é
`draw_special_roles`, que já deixa de fora o que não couber.

**Papel ligado NÃO é papel garantido, e isso é de propósito.** Cada vaga rola a própria chance
(`role_chance`, 100 = sempre), como no original: com chance abaixo de 100 a tripulação não pode
assumir que o xerife existe, e é essa dúvida que faz o papel valer. Três consequências que
precisam continuar verdadeiras juntas: o `S_GAME_START` anuncia o que a sala **configurou**, nunca
o que saiu (anunciar o resultado entrega de graça a dedução que a chance cria); uma vaga sorteada
para fora **não gasta jogador**, senão "2 xerifes a 50%" tiraria duas pessoas do sorteio dos
outros papéis sem xerife nenhum aparecer; e quando não há gente para todos os papéis (sala pequena,
ou cheia de bots, que nunca recebem papel especial) o que não couber simplesmente não sai, calado -
do lado de quem joga isso é indistinguível de uma chance que não saiu, e é isso que o torna
seguro.

**Quando há mais papéis ligados do que vagas no time, cada papel tem a MESMA chance - e já não
tinha.** O sorteio distribuía as vagas na ordem de `CONFIGURABLE_ROLE_IDS`, então o primeiro da
lista ganhava sempre: com um impostor e metamorfo, fantasma e atirador ligados, saiu metamorfo em
60 mil de 60 mil partidas simuladas. Sintoma relatado: "o papel de impostor é sempre o mesmo". Hoje
as vagas que passam na chance são EMBARALHADAS antes de encontrar gente. As PESSOAS nunca foram
favorecidas - os jogadores já vinham embaralhados, e a sonda mede isso também. O embaralhamento dos
jogadores (`shuffle_player_order`) mora junto do `draw_special_roles`, para a sonda usar a mesma
função. Sonda: `tools/probes/probe_draw_fairness.nvgt` (sem servidor; confere também que `random(a, b)`
inclui o b - sem isso o Fisher-Yates viraria Sattolo e ninguém ficaria no próprio lugar).

**Quem matou só viaja para a vítima e para o assassino** (`announce_kill`, no servidor). O
`S_PLAYER_KILLED` ia à partida inteira com `killer_peer` e `killer_color`: o cliente não os dizia,
mas um cliente modificado sabia o impostor de toda morte - e, com killer == vítima, que o morto era
um xerife que errou (o comentário do `misfire` dizia ter evitado isso, e só tinha evitado o CAMPO).
Regra geral: o cliente é a máquina do jogador, e tudo o que vai para lá, quem quiser lê - "o jogo
não fala" não é segredo. A morte por BOT passava por outro caminho (o tick da sala transmitia tudo a
todos) e pulava também a câmera da vítima, o scanner e o alarme do alarmista; hoje os dois caminhos
passam por `announce_kill`. Sonda: `tools/probes/probe_kill_privacy.nvgt`, que é o cliente
modificado - lê o pacote cru de cada um.

**A exceção deliberada: a TESTEMUNHA colada** (0.40.0). Quem está a até `PLAYER_KILL_RANGE` da vítima
recebe `S_KILL_WITNESSED`, só ela, com quem matou quem - pedido do usuário: colado, fingir que a pessoa
não viu era só confusão. O limite é o próprio alcance do kill, e não o do corpo (2,0), para matar a
dois passos continuar anônimo. Vivo, fora de duto e visível; o disfarce do metamorfo vale (é o rosto
que se vê); o tiro errado do xerife fica de fora (entregaria o xerife); e no ESCURO ninguém vê
(pedido do usuário), pela mesma regra de quem as luzes cegam (`blinded_by_lights`) - o que dá ao
impostor um motivo a mais para apagar as luzes antes de matar. O bot impostor nunca mata com
alguém a menos de `BOT_KILL_WITNESS_RANGE` (6), então nunca gera testemunha. Sonda:
`tools/probes/probe_kill_witness.nvgt`, com a testemunha LONGE como controle.

**A recarga da HABILIDADE fica pausada na reunião; a do kill recomeça.** Até a 0.36.0 a da
habilidade recomeçava inteira no fim da votação (`ability_cooldown_resets_on_meeting`, ligado por
padrão), porque o relógio corria durante a reunião e sem o reset a discussão recarregava de graça.
O usuário achou injusto: quem entrava com a habilidade quase pronta saía com a recarga inteira. A
saída foi PAUSAR (servidor: `tick_cooldowns(delta, em_reuniao)`; cliente: `movement_frozen`), o
que não dá nem tira nada. Os dois lados têm que pausar juntos, senão o cliente anuncia "pronta" e a
tecla é recusada. O kill continua recomeçando, de propósito. Sonda: `tools/probes/probe_meeting_reset.nvgt`,
que espera antes da reunião justamente para "pausou" e "recomeçou" darem números diferentes.

**NEUTROS: o terceiro time (0.41.0, com o Bobo).** `TEAM_NEUTRAL`, sorteado entre os não impostores
(o `draw_special_roles` já manda tudo que não é impostor para esse lado). Vitória própria em
`role_traits.solo_win` (hoje só `SOLO_WIN_EJECTED`); quando se cumpre, `winner = WINNER_NEUTRAL` e
`neutral_winner_peer` diz quem - a partida acaba com ele vencendo sozinho, com o som de vitória que
estava reservado (`SND_VICTORY_DISCONNECT`). Três regras que um neutro NOVO tem que manter: conta como
TRIPULANTE para a maioria (é `!is_impostor`, como no original - sem isso daria vitória aos impostores
antes da hora); não faz tarefa que conte (`tasks_count_for_crew = false`, e `counts_for_task_win`
passou a perguntar isso, e não "não é impostor" - pela regra antiga um neutro travaria a vitória por
tarefas); e é revelado no fim (`neutrals` no `S_GAME_OVER`). O texto de quem venceu mora em
`game_winner_text` (event_speech), usado pelo anúncio E pela tela de resultado. Exige protocolo 7.
As ESTATÍSTICAS da conta (partidas, vitórias por lado, neutras, assassinatos) são gravadas em
`record_match_stats`, no servidor, no tick em que a partida acaba - antes do `reopen_lobby`, que zera
vencedor e papéis. Até aqui só as tarefas eram contadas; o resto das colunas existia e ficava em zero.
Sonda: `tools/probes/probe_jester.nvgt` (os outros dois jogadores são o controle: partida jogada, sem
vitória).

**LOBO MAU: engolido é `alive = false` + `swallowed_by`, e não um terceiro estado.** A alternativa era
deixá-lo vivo com uma marca - e aí cada contagem de vitória, a votação, o radar, a câmera, o alvo de
kill, o reporte, a sabotagem precisariam de uma condição nova, e esquecer uma deixaria um engolido
votando ou sendo morto. Com `alive = false` tudo isso já o exclui de graça; o que pede condição própria
é o CONTRÁRIO, as liberdades do fantasma que ele não tem: andar (`try_move_player`), tarefa
(`on_task_input`), sabotar e habilidade de morto (`player_abilities.swallowed`), passos dos mortos
(`broadcast_to_dead_players`), chat da mesa e a voz (`can_hear_voice`, que decide a barriga ANTES da
regra dos fantasmas - senão os mortos ouviriam os engolidos). Um lugar novo que dê algo ao fantasma
precisa perguntar `swallowed()` também, ou o engolido ganha junto.
- A volta (`release_belly`) é chamada em TODA saída do lobo: `try_kill`, `try_guess`, `tally_votes`,
  `remove_from_lobby`, `take_heartbreaks` (morrer de amor - esquecida até a 0.50.9) e o próprio
  `try_eat` (lobo engolindo lobo). O aviso sai por `pending_events`,
  que o tick entrega depois da apuração e antes da conferência de vitória - o cliente ouve a expulsão
  e SÓ ENTÃO a volta, e a vitória já conta com quem voltou.
- Com gente na barriga, os impostores NÃO vencem por maioria (`anyone_in_belly`, recado #162): o lobo
  engolia a tripulação inteira e entregava a vitória a eles, sem ninguém ter morrido. Decisão do
  usuário: o impostor precisa derrubar o lobo, como a tripulação precisa. De barriga vazia a maioria
  vale normalmente - a alternativa descartada era "lobo vivo bloqueia sempre" (o Town of Us).
  Sonda: `probe_wolf_majority.nvgt`, com a mesma mesa de barriga vazia como controle.
- A vitória dele (`SOLO_WIN_LAST_STANDING`) é conferida ANTES da maioria do impostor: lobo e impostor
  sozinhos dariam a vitória ao impostor pela regra velha. E `hostile_to_crew` impede a tripulação de
  vencer por eliminação com ele vivo, e deixa o xerife acertá-lo sem errar o tiro.
- A reunião não anuncia ninguém (decisão do usuário): `S_MEETING_STARTED.absent` só tira os engolidos
  da lista de votação, e o servidor recusa voto neles (`cast_vote` passou a conferir o alvo).
- A recarga COMEÇA correndo (`ability_starts_on_cooldown`, que o cliente lê em
  `ability_initial_cooldown`): engolir no primeiro segundo, com todos juntos no ponto de partida,
  acabava a partida de três antes de ela começar.
- Engolir tem som próprio (`events/wolf_swallow.ogg`), para o lobo, o engolido e quem está perto
  (posicionado). A primeira versão usava o som do kill para os vizinhos, apostando em "procura o corpo
  e não acha"; com o som próprio, ouvir diz que há um lobo a bordo - sem dizer quem.
- O elenco dos OUTROS clientes guarda o engolido no último ponto em que andou; quem passa por ali não
  o ouve anunciado porque o servidor manda a cada um quem é perceptível perto dele (S_NEARBY_PLAYERS,
  ver notes/som.md). Era um defeito herdado dos dutos, consertado junto.
- O engolido fica ONDE O LOBO ESTÁ, no servidor (`follow_wolves`, a cada tick): o radar dele varre a
  sala do lobo sem regra nova nenhuma no radar, e é cegado pela sabotagem como o LOBO seria (os
  "olhos" em `radar_targets`), não com a imunidade de fantasma. No cliente, a sala é dita quando o
  lobo muda de sala, e a tecla de onde estou responde por ela (recado #160, decisão do usuário: os
  dois modos do radar, do ponto do lobo). Não vaza nada: o engolido só fala com o próprio lobo.
- Na REUNIÃO a barriga ouve a mesa (voz e chat), e a mesa continua sem ouvi-la: a regra mora em
  `can_hear_voice`, ANTES da regra da barriga, e vale só no sentido mesa -> engolido. Era surda à
  discussão inteira, e isso tirava o engolido do jogo justo na hora que mais importa (decisão do
  usuário). É também a base do modo espectador, que vai ouvir do mesmo jeito.
- A VOZ da barriga, no cliente do lobo, toca SEM posição (`g_voice.inside_me`). Posicionada pelo
  elenco, ela ficava presa no ponto onde a pessoa foi engolida enquanto o lobo andava - justo o que o
  papel promete (ouvir as vítimas) virava um som parado no mapa. Sintoma relatado pelo usuário: "a voz
  fica presa lá onde ele comeu".
Sonda: `tools/probes/probe_wolf.nvgt` (engolir, privacidade dos pacotes, voz da barriga, reunião,
expulsão devolvendo, e a vitória por sobrar).

**Engenheiro: o duto dele custa uma dedução, e isso foi pago de propósito.** "A tampa abriu na
navegação, então ele saiu em armas ou no reator" deixa de provar impostor. O som da tampa continua
IGUAL para os dois - fazer um som diferente devolveria a certeza e mataria o papel. O conserto
remoto (`ABILITY_REMOTE_FIX`) acaba com a sabotagem inteira de qualquer lugar, com recarga de
120 s (`ENGINEER_FIX_COOLDOWN_SECONDS`, maior que a recarga da sabotagem de propósito: consertar
toda sabotagem que aparece transformaria a habilidade num botão de desfazer). Ele sai pelo MESMO
`S_SABOTAGE_FIXED` do conserto normal - o resto do jogo não precisa saber que há um segundo jeito
de a sabotagem acabar - e **sem dizer quem consertou**: o sinal é a sabotagem terminar sem ninguém
ter chegado ao painel. Sonda: `tools/probes/probe_engineer.nvgt`.

**Xerife: quem morre no tiro é DECIDIDO, não presumido - e o erro é segredo dele.** `try_kill`
escolhe a vítima (atirador da tripulação + alvo que não é impostor = morre o atirador), então todo
lugar que tratava "o alvo morreu" passou a ler o `peer_id` do PACOTE - foi assim que o `on_kill`
soltava a câmera da pessoa errada. O `misfire` **não vai no `S_PLAYER_KILLED`**: ele é transmitido
à partida inteira, e o campo entregaria de graça que houve um xerife e que ele errou. O aviso vai
em particular, por `S_ERROR`, só para quem atirou; para todos os outros - inclusive para quem ele
mirou, que nem sabe que foi mirado - aquilo é um assassinato qualquer. E `you_were_killed_text`
trata o caso de o assassino ser a própria vítima, senão a fala é "você foi eliminado por <seu
nome>". Sonda: `tools/probes/probe_sheriff.nvgt`.

**Alarmista: o aviso sai da CAPACIDADE (`announces_own_death`) e depois do pacote da morte.** A
ordem importa - invertida, o texto contaria o que aconteceu antes de o som do assassinato tocar. O
`S_DEATH_ALARM` diz nome e SALA, nunca quem matou nem a posição exata: quem quiser mais que isso
tem que ir até lá. E ele entra em `background_event_text`, senão quem estivesse numa task - que é
exatamente onde a pessoa está quando o impostor escolhe matar longe de todos - não ouviria o único
aviso que o papel existe para dar. Sonda: `tools/probes/probe_noisemaker.nvgt`.

**O alarme do alarmista (H) É o botão de emergência, e não uma habilidade com contagem própria.**
Decisão do usuário: mesma contagem de reuniões da sala, mesma recarga, mesmos bloqueios; só muda
poder apertar de qualquer lugar. Uma reunião a mais, ou um anúncio "foi o alarme", entregaria o
papel ao impostor. Por isso o cliente manda o MESMO `C_CALL_MEETING` e não passa por
`on_use_ability` - o servidor já não confere a distância até o botão (ver `on_call_meeting`), então
não há nada a mudar lá. A consequência a saber: a distância é regra só do cliente, para todo mundo;
se um dia o servidor passar a conferi-la, o alarmista precisa entrar como exceção declarada no papel.

**Anjo da guarda: o escudo é conferido no TOPO do `try_kill`, e o lugar dele ali é a regra.** Antes
de consumir a recarga do atacante, porque gastá-la seria um aviso indireto ("apertei, não saiu som
de kill e meu cooldown zerou" só pode significar escudo); e antes do cálculo do tiro errado do
xerife, o que faz o escudo valer contra QUALQUER ataque - uma regra só, sem exceção para explicar.
Nada é transmitido ao lançar: nem ao protegido, nem à partida. O anjo ouve só o nome de quem
protegeu, e **nunca** fica sabendo se o escudo serviu de algo - saber que ele barrou um ataque
seria saber que havia um impostor ali, e fantasma não pode ter essa informação (ele conversa com
outros fantasmas por voz). `ability_usable_alive`/`ability_usable_dead` são DOIS campos porque há
três casos: só vivo, vivo e morto, e só morto - este. Sonda: `tools/probes/probe_guardian.nvgt`.

**Detetive: o rastro é frio, sai em SALAS, e a janela veio da geometria do mapa.** Ele grava por
onde o assassino passou nos `KILLER_TRAIL_SECONDS` seguintes à morte e para - é testemunho (uma
frase que ainda vale na reunião), não caçada (uma direção ao vivo, que apodrece em dez segundos e
faria o detetive perseguir sozinho). **Corredor não entra**: o rastro é para ser DITO, e
`room_id_at` devolve corredor também, então o filtro é explícito (`zone.is_corridor`). **Dentro do
duto nada é gravado**, pela mesma regra que esconde quem está ventilado do radar e da câmera - e
isso faz de ventilar a contra-jogada natural contra o papel. O `killer_peer_id` fica no corpo só
para gravar e **nunca** sai dali.

Os 25 segundos saíram da GEOMETRIA: da borda da cafeteria à borda da armaria são 22 unidades,
exatamente dez segundos a `PLAYER_MOVE_SPEED`. Com a janela de dez que eu tinha posto, o rastro
saía vazio em quase toda morte e o papel nascia morto - a sonda pegou de primeira. **Mexeu no
tamanho do mapa, revise `KILLER_TRAIL_SECONDS`.**

**Lista traduzível vem como CHAVES separadas por "|", e quem junta é o cliente.** O
`tr_server_message` substitui parâmetro como texto cru, então uma lista de salas chegaria como
`room.electrical|room.storage` dentro da frase. Ver `examine_result_text` em `event_speech.nvgt`.

**O ELENCO do cliente é a fonte de todo nome que ele fala sozinho** - quem está por perto, quem
votou, quem está falando no chat de voz. Ele é carregado uma vez, no `S_GAME_START`, e por isso o
disfarce do metamorfo tinha um buraco que só um jogador achou: **chegar perto dele anunciava o nome
verdadeiro**, justamente no momento em que o disfarce mais importa. Hoje o servidor transmite
`S_PLAYER_RENAMED` ao transformar, ao expirar e na reunião, e o cliente renomeia a entrada - o que
fecha todos esses caminhos de uma vez, inclusive os que alguém acrescentar depois. Quem for
acrescentar um lugar que fale nome: use o elenco, não invente outra fonte.

**Metamorfo: todo nome que chega a outro jogador passa por `display_name()`.** Num jogo sem imagem,
"parecer com alguém" é aparecer com o nome dele, e o nome só sai do SERVIDOR - radar, câmera, quem
está na sala. Um ponto esquecido é um buraco no disfarce, sem nada acusando no código.

**Na PARTIDA, nome de jogador sai do servidor por `public_name()` ou `display_name()`, NUNCA por
`username`.** Desde o modo cor (0.38.0): `public_name()` é o nome sem disfarce - ou, no modo cor, a
MARCA da cor (`#cor:red`, ver `color_mark`) -, e `display_name()` é o que os outros veem agora (o
disfarce, se houver). Havia uns dez pontos mandando `username` cru (votos, expulso, alarme, chat,
elenco, parceiros, quem saiu, o anjo) - todos trocados. Onde o nome REAL precisa aparecer, é
deliberado e escrito: o fim da partida diz "Fulano (Vermelho)". Fora de partida (`named_by_color`
falso) os dois são o nome. No cliente, a marca vira cor no idioma de cada um pelo `tr()` (todo
parâmetro) e pelo `spoken_name()` (elenco, chat) - nada mais precisa saber que ela existe.
Metamorfo no modo cor copia a COR do alvo, pela mesma regra. Sonda: `tools/probes/probe_color_mode.nvgt`,
que lê o pacote cru de todo mundo do início ao fim da votação procurando qualquer nome de usuário.

**A vítima é a exceção, e é deliberada: ela ouve o nome VERDADEIRO.** Desde que o elenco passou a
carregar o nome disfarçado, isso exige uma CÓPIA privada: o `S_PLAYER_KILLED` vai à partida sem nome
nenhum, e só a vítima recebe uma versão com `killer_real_name`. Quem morreu já pagou o preço e não tem como contar a
ninguém (não vota, não fala com vivo, e fantasma só é ouvido por fantasma), então mentir para ele
não compra nada ao impostor e só piora o jogo de quem já perdeu - e no caso normal a vítima também
sabe quem a matou, então disfarçar aqui tornaria morrer para o metamorfo pior do que morrer para um
impostor comum. Eu tinha feito o contrário, e o usuário corrigiu. Cuidado ao mexer: o nome
verdadeiro **não pode** entrar no `S_PLAYER_KILLED`, que é transmitido à partida inteira - seria o
mesmo erro do `misfire` do xerife.

**Pendência do disfarce: ele cobre o NOME, não a cor.** Hoje isso não vaza porque todas as cores
usam o mesmo som de kill (os sons por cor têm fallback genérico e nenhum foi criado). No dia em que
existir som de kill por cor, quem estiver perto vai ouvir a cor REAL do metamorfo - e o disfarce
precisa passar a copiar a cor junto (ver `killer_color` no `S_PLAYER_KILLED`).

Três coisas seguram o papel: o SOM da transformação é posicionado e **não leva nome nenhum** (quem
ouve sabe que algo aconteceu ali, não quem nem em quem); a **voz cala** enquanto durar (ela chega
identificada pelo peer, e uma frase o entregaria - a decisão fica na linha do `g_voice.allowed`,
recalculada por quadro, e não numa transição, senão o quadro seguinte a desfaz); e a **reunião
desfaz a fantasia**, porque lá se vota por NOME e dois nomes iguais na lista não são blefe, são
uma tela quebrada. Sonda: `tools/probes/probe_shapeshifter.nvgt`.

**Fantasma: a invisibilidade reaproveita a semântica do DUTO, nos mesmos três pontos.** "Presente
mas imperceptível" já existia, e pendurar `invisible()` ao lado de `vented` em `players_in_room`
(radar E câmera de uma vez), `on_move` (a posição é o que os outros clientes viram PASSOS - sem
isto ele seria invisível no radar e continuaria pisando alto) e `on_voice_frame` cobre tudo sem
inventar condição nova. Um quarto ponto esquecido seria um caminho que continua denunciando ele, e
o silêncio do código não acusaria nada.

**Sumir é silencioso para todo mundo menos para ele**, e isso é decisão de jogo, não esquecimento:
não existe pacote de "som de sumir" para os vizinhos. Eu tinha feito o contrário (o som vazava,
como a tampa do duto) e o usuário corrigiu - o fantasma some de verdade, sem rastro. Quem estava
perto simplesmente para de ouvir os passos daquela pessoa. **Se um dia isso soar forte demais, o
conserto é devolver o som aos vizinhos**, e não enfraquecer a invisibilidade por outro caminho.

Duas sutilezas: **matar devolve ele na hora** (senão não há jogo do outro lado - e o som do
assassinato, esse sim, é público), e a **reunião também** - invisível não é ouvido, e ficar mudo na
mesa sem explicação é um sinal luminoso de quem é o fantasma. E `was_invisible` existe porque `invisible_remaining == 0` não distingue "acabou de
voltar" de "nunca sumiu": sem a marca, o som da volta tocaria para todo mundo, o tempo todo. Sonda:
`tools/probes/probe_phantom.nvgt`.

**Sniper: a única exceção à reunião como pausa, e ela é DECLARADA.** `on_use_ability` recusava
qualquer habilidade fora de `state == "in_progress"`, o que é a regra do jogo escrita em código.
Abrir a exceção com um `if` de nome de papel ali dentro seria furar a regra sem deixar rastro; hoje o
papel declara `ability_usable_in_meeting` e o servidor pergunta pela capacidade, como no resto do
arquivo. Quem ler a recusa encontra a exceção junto dela.

O palpite **não passa por `try_kill`**, embora as duas coisas matem. `try_kill` é o assassinato no
MAPA: deixa corpo, consome a recarga do tiro, revela o fantasma e abre o rastro do detetive - tudo
coisa que depende de haver um lugar onde aconteceu. Na mesa todo mundo está no mesmo ponto, e um
corpo na cafeteria seria reportado no segundo seguinte pela reunião que está em curso. Quem morre na
mesa morre sem corpo: a reunião inteira viu.

Três decisões que seguram o papel, e o que cada uma evita:

- **"Tripulante" não é chutável** (`role_is_guessable` só aceita `CONFIGURABLE_ROLE_IDS`). A maioria
  da mesa é tripulante comum: com ele na lista, bastava apontar qualquer um e o papel virava um
  botão de matar de graça na reunião. O efeito colateral é bom - o poder dele ESCALA com quantos
  papéis especiais a sala ligou, e numa sala sem papéis especiais ele não tem o que apontar.
- **Errar mata ele.** Sem isso, com quatro papéis ligados, ele testa um por reunião até acertar.
- **A mesa não fica sabendo QUEM apontou** (ver `S_MEETING_KILL` no protocolo). O custo dele é o
  risco, não a exposição: revelar o autor faria de todo palpite certo um suicídio e o papel deixaria
  de existir na prática. É o botão a mexer se na prática ele soar forte demais - e não tirar o
  palpite.

O escudo do anjo **não** protege na mesa, e isso não é esquecimento: a reunião já apaga o escudo ao
começar, decisão anterior e com motivo próprio. Uma regra a menos para explicar.

A partida pode ACABAR no meio da reunião (um tripulante a menos pode dar a maioria aos impostores). Não
há código para isso porque a conferência de vitória no `tick` roda a cada quadro e FORA do bloco da
votação - **se um dia ela passar a rodar só no fim da apuração, este caminho quebra em silêncio.**

Sonda: `tools/probes/probe_sniper.nvgt`. Duas travas do jogo morderam ao escrevê-la, e as duas
custaram uma execução: o botão de emergência tem recarga de 15 s **e** cada jogador só chama UMA
reunião por partida - a segunda reunião precisa de outra pessoa chamando, e de insistência em vez de
um instante chutado.

**Prefeito, Trocador, Lixeiro e Carregador (0.47.0, protocolo 10).** Decisões do usuário e o porquê:
- **Prefeito: o peso mora no papel (`vote_weight`) e só a APURAÇÃO o lê** (`tally_votes`). A lista de
  quem votou em quem o mostra uma vez - anunciar o prefeito o faria primeiro alvo e dono da mesa.
- **Trocador: a troca é anunciada no resultado (`swapped_a/b` no `S_VOTE_RESULT`), sem dizer quem
  trocou.** Sem o aviso, uma expulsão com os votos de outra pessoa não faz sentido para ninguém. Uma
  por reunião, a primeira (`swap_a/b` no game_state, zerados no `start_meeting`); os dois têm que
  estar vivos na apuração - quem morreu pelo palpite do sniper no meio desfaz a troca.
- **Lixeiro e Carregador tiram o corpo de `bodies`** e avisam TODOS (`S_BODY_REMOVED`): todo cliente
  guarda os corpos para o som e o reporte, e um corpo que só sumisse no servidor continuaria tocando.
  O carregado fica em `carrier_peers`/`carried_bodies` até ser largado (`S_BODY_PLACED`), e
  `drop_carried_body` roda em TODA saída do carregador (largar, morrer em `try_kill`, ser engolido em
  `try_eat`, sair da sala) - senão o corpo sairia da partida junto com ele. A reunião apaga o
  carregado junto com os outros.
- **O preço dos dois é o SOM** (`S_BODY_SOUND`, posicionado e sem nome): limpar ou arrastar em silêncio
  tornaria o kill sem risco. O arrasto soa no ritmo de um passo (`BODY_DRAG_SOUND_INTERVAL`, em
  `on_move`). Carregando não se mata nem se ventila, e a recusa do kill vem ANTES da da recarga: logo
  depois de matar, "recarregando" esconderia o motivo que diz o que fazer (largar).
Sondas: `probe_body_roles.nvgt` (sem rede, cada caso com controle) e `probe_body_roles_net.nvgt` -
com QUATRO jogadores: com três, o kill dá um contra um e a partida acaba ali, e a sonda passa a falar
com uma sala reaberta onde todos voltaram a tripulante (custou uma rodada achar isso).

**Carrasco, Incendiário e Amnésico (0.47.0, também protocolo 10).** Decisões do usuário e o porquê:
- **O papel pode MUDAR no meio da partida** (Carrasco sem alvo vira Bobo; Amnésico herda). Tudo que o
  cliente só recebia no `S_GAME_START` - recargas, palpites, companheiros de impostor, alvo - vai de
  novo no `S_ROLE_CHANGED` (privado, `send_role_changed`), e `on_role_changed` refaz o que o começo
  da partida montava. Papel novo que precise de algo do início: ponha nos DOIS lugares.
- **Carrasco:** o alvo é sorteado em `assign_execution_targets`, depois dos papéis (só time da
  tripulação, bot incluído; sem candidato, vira Bobo já no início). A vitória mora na apuração, ao
  lado da do Bobo; a conversão em `convert_lost_executioners`, chamada pelo servidor a cada tick - e
  engolido NÃO é alvo perdido, porque pode voltar.
- **Incendiário: sem som nenhum** (decisão do usuário). A mesma tecla encharca e põe fogo; quem
  decide qual é o servidor (`not_doused_count`). O fogo mata sem corpo e encerra a partida na hora.
  A cena da vitória (do jogador Raciel) segura o fim da partida até o som acabar, e **Enter a pula**
  (`on_game_over` em `meeting_ui`): inteira, ela pesava toda partida.
  `hostile_to_crew`, como o lobo.
- **Amnésico: herda QUALQUER papel, impostor inclusive** (decisão do usuário). O corpo fica onde está;
  só os impostores são avisados quando ele vira um deles (`role.teammate_joined`). O kill herdado
  começa recarregando, e a habilidade também (on_use_ability arma a recarga já com o papel novo).
Sondas: `probe_neutral_roles.nvgt` (sem rede) e o caso do `S_ROLE_CHANGED` no `probe_match_packets`.

**Médico (0.47.0): o único limite é a JANELA depois da morte** (`TUNABLE_WINDOW`, decisão do usuário):
quem volta sabe quem o matou e pode contar, e essa é a graça. Voltar é público (`S_PLAYER_REVIVED` a
todos, junto do `S_BODY_REMOVED`), porque a lista de votação e o elenco de todo mundo precisam voltar
a contá-lo - mas não diz quem reviveu. Sondas: o caso no `probe_neutral_roles` (com a janela vencida
como controle) e no `probe_match_packets`.

**Modificadores (0.47.0): marca POR CIMA do papel, configurada como papel.** Ficam nos mesmos
`role_counts`/`role_chances` da sala (formulário, protocolo e configuração salva vêm de graça), mas
numa lista à parte (`CONFIGURABLE_MODIFIER_IDS`): não entram no sorteio dos papéis nem no palpite do
atirador. Quem pergunta "isto a sala pode ligar?" usa `is_configurable_id`/`configurable_ids()`,
nunca só `CONFIGURABLE_ROLE_IDS` - foram três filtros a trocar (leitura do pacote, validação, protocolo
exigido) e mais o formulário e a leitura das regras. Sorteados em `assign_modifiers`, depois dos
papéis e dos alvos, só entre humanos.
- **Flash e Gigante (50% cada lado, decisão do usuário):** a velocidade do servidor vem de
  `config.move_speed() * p.speed_multiplier()` em `try_move_player`, e o cliente recebe a dele no
  `S_GAME_START`. **Os passos dos OUTROS passaram a soar por distância andada** (`step_length`, a
  distância de um passo na velocidade da SALA), e não por tempo: por tempo, todo mundo soava no mesmo
  ritmo, e a cadência era justamente o que o usuário quis que dissesse quem é veloz ou gigante. Na
  velocidade normal dá o mesmo ritmo de antes.
- **Apaixonados:** morrer de amor sai de `take_heartbreaks`, chamada pelo servidor a cada tick - vira
  corpo onde o par estiver (a partida ouve uma morte sem assassino; ele, `S_HEARTBREAK`). Só com a
  partida andando: morto na mesa, o par cai quando a reunião acaba (um corpo na mesa seria reportado
  pela reunião seguinte inteira). Engolido não é morto. Morrer de amor é mais uma SAÍDA do jogo: chama
  `release_belly` e `drop_carried_body` como as outras (até a 0.50.9 não chamava, e o lobo apaixonado
  morria com gente presa numa barriga que não existia mais). A vitória (`WINNER_LOVERS`, o casal e no
  máximo mais um) vem ANTES da maioria do impostor. E o médico que revive um apaixonado traz o par
  junto - sem isso o revivido morria de amor de novo no tick seguinte.
Sonda: `probe_modifiers.nvgt`, com controle em cada caso.

**Papel declara o que pode; o código pergunta por capacidade.** `src/game/roles/role_traits.nvgt`
define cada papel (pode matar, ventilar, sabotar, o que percebe) e `player_abilities` combina isso
com estar vivo. Nada deve voltar a perguntar "é impostor?" para decidir uma ação — era assim que a
resposta virava um booleano em mais de sessenta pontos, e cada papel novo obrigava a revisitar todos.
Um papel novo entra em três passos, descritos no topo daquele arquivo.


