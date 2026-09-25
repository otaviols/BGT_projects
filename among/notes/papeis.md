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

**Papel declara o que pode; o código pergunta por capacidade.** `src/game/roles/role_traits.nvgt`
define cada papel (pode matar, ventilar, sabotar, o que percebe) e `player_abilities` combina isso
com estar vivo. Nada deve voltar a perguntar "é impostor?" para decidir uma ação — era assim que a
resposta virava um booleano em mais de sessenta pontos, e cada papel novo obrigava a revisitar todos.
Um papel novo entra em três passos, descritos no topo daquele arquivo.


