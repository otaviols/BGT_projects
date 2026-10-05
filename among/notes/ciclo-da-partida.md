# Ciclo da partida

Começo, reunião, sabotagem, morte e fim de partida. Leia antes de mexer em qualquer coisa que dure uma partida.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## Ciclo da partida

**Quando a partida acaba, o laço de pacotes PARA de drenar a fila.** O que vem logo atrás do
`S_GAME_OVER` já é da próxima rodada - o retrato da sala reaberta e, se o anfitrião não esperou, o
`S_GAME_START`. Como nenhuma condição do laço de partida trata um início de partida, continuar
drenando o jogava fora em silêncio, e quem tinha ficado na tela de resultado ficava de fora da
rodada. São TRÊS peças para esse caminho funcionar, e ele só funciona com as três: a tela fecha
sozinha (`run_menu_until_next_match`), o laço para de drenar (o `break` dentro do while), e a sala
de espera pega o `S_GAME_START` que estava na fila. Uma sonda por peça:
`probe_limbo`, `probe_fim_para_proxima` e `probe_entra_na_partida` - as três conferidas contra a
versão sem a correção, onde falham.

**Tela que o jogador pode deixar aberta precisa saber fechar sozinha.** A de resultado é a única
sem pressa - dá para sair para atender o telefone com ela aberta -, e `menu.run()` só sai por tecla.
Quando o anfitrião começava a rodada seguinte, o `S_GAME_START` chegava e ficava na fila sem ninguém
para lê-lo: a pessoa ouvia as vozes da partida nova acontecendo sem ela e não conseguia jogar.
`run_menu_until_next_match` abre o laço do menu e devolve quando esse pacote aparece. Duas coisas
aprendidas junto: a fila é examinada a partir de um CURSOR que só cresce (ninguém a consome com o
menu aberto), porque varrê-la inteira por quadro é o que já travou o jogo; e **uma sonda que
reimplementa a lógica que testa não testa nada** - a primeira versão desta passava com o código
quebrado, e só virou teste de verdade quando passou a chamar a função real e a ser conferida contra
a versão sem a correção (onde ela trava).

**Toda tela que alguém lê enquanto a partida pode começar usa `run_menu_until_next_match`**
(`src/ui/match_aware_menu.nvgt`, saiu do game_loop para a sala de espera poder usar). Hoje são a de
resultado e a de regras da sala (tecla C) - esta é exatamente o que se lê enquanto o anfitrião
aperta Enter. A função aceita itens de AÇÃO (`menu_action_handler`): copiar o resultado, copiar o
código da sala, sem fechar a tela. Uma tela nova desse tipo que abrisse um `menu.run()` comum
repetiria a armadilha acima.

**O fim da partida fecha o posto de câmeras e o scanner ANTES do `S_GAME_OVER`** (no tick da sala,
servidor), e o cliente, ao receber o fim, sai do modo câmera e cala o som de sala observada. Antes o
posto só era liberado ao reabrir a sala, sem aviso: quem estava nas câmeras continuava nelas na tela
de resultado, e todos seguiam ouvindo "alguém observa esta sala". Era o terceiro caminho do mesmo
defeito (estado desfeito só de um lado), depois da reunião e da morte. Sonda:
`tools/probes/probe_camera_game_over.nvgt`, que acaba a partida por MAIORIA - por reunião o defeito
não aparece, porque a reunião já fechava as câmeras - e que falha contra o servidor sem a correção.

**Saiu o último humano no meio da partida: a sala FECHA na hora** (`remove_from_lobby`, servidor). Antes
ela só fechava no fim da partida (`reopen_lobby`) - e só com bots, no modo lobos ou no pega-pega, o fim
não chega: a lista de partidas enchia de salas "em andamento" com oito bots e ninguém (recados #182,
#190, #198, #200). Com espectador, segue. Sonda: `probe_bots_abandoned.nvgt`. Duas armadilhas dela,
descobertas na rodada de controle, que passava sem a correção: com um humano IMPOSTOR, a saída dele já
encerra a partida pelo caminho normal (a sonda sorteia até os dois humanos serem tripulantes); e depois
de `disconnect()` o cliente precisa continuar recebendo `update()`, senão o aviso de saída não sai e o
servidor só percebe a queda por tempo esgotado.

**Bot age PEDINDO, e o servidor executa pelo tratador do jogador** (`bot_request` em game_state,
consumido em `tick_lobbies`). Consertar painel vira um `C_INTERACT` passado a `on_interact`; reportar
corpo, um `C_REPORT_BODY` a `on_report_body` - os mesmos de quem joga, com todos os efeitos (aviso de
painel, reunião, câmeras, scanner, portas). Duas peças deixam isso funcionar e falham caladas se
sumirem: `find_lobby_of_peer` acha a sala de um BOT (ele não é conexão; sem isso o tratador volta na
primeira linha e nada acontece), e `send_to` ignora ids de bot (`BOT_PEER_ID_BASE`), para onde os
tratadores respondem. Tripulante bot MORTO continua fazendo tarefa (e só isso); os passos dele saem
só para os mortos, pela mesma triagem do `on_move`. Tempos de reação (`BOT_*_REACTION`) existem para
o bot não resolver tudo no mesmo quadro - e ele PARA ao ver um corpo, senão passaria por ele antes de
a reação acabar. Sondas: `probe_bot_ai` (estado, sem rede - o oxigênio precisa do `sabotage_timer`
armado, senão "acaba" no primeiro quadro e a sonda passa em 0 s) e `probe_bots_fix_sabotage` (o
caminho de rede de ponta a ponta).

**A reunião é uma PAUSA - com UMA exceção, o sniper.** Ele aponta alguém na mesa e mata se acertar o
papel (ver notes/papeis.md). A exceção é declarada pelo papel (`ability_usable_in_meeting`), e não
escrita como um furo no `if` do servidor: a regra abaixo continua valendo para todo o resto, e quem
ler a recusa encontra a exceção junto dela. Ao acrescentar qualquer coisa que aja na reunião, a
pergunta é a mesma - o papel declara, o código pergunta pela capacidade.

**Ninguém mata,
ninguém usa habilidade e ninguém anda durante a discussão - então todo relógio que continue correndo
ali é tempo ganho de graça, e a reunião vira a melhor jogada de quem tem o relógio mais caro. O
cooldown de kill já era rearmado no fim da votação; a habilidade do papel não era, e o metamorfo saía
da mesa pronto para virar alguém no meio de todo mundo. Hoje quem decide é o PAPEL
(`ability_cooldown_resets_on_meeting`, padrão `true`), e não um `if (é impostor)`: as habilidades de
DEFESA se recusam, com o motivo escrito em cada uma - a sabotagem atravessa a reunião intacta, então
rearmar o engenheiro junto entregaria ao impostor, pelo outro lado, exatamente o que a regra queria
tirar dele. Efeito ATIVO é outra coisa: disfarce, invisibilidade e escudo **caem** ao começar a
reunião, e cada um por um motivo próprio (dois nomes iguais na lista de votação são uma tela
quebrada; invisível não é ouvido e ficar mudo na mesa denuncia o fantasma; um escudo mantido só
queima o relógio numa fase em que ninguém ataca, e quanto sobra dependia do tamanho da discussão).
Regra prática ao acrescentar qualquer timer de partida: decida as DUAS coisas separadamente - a
reunião apaga o efeito? a reunião rearma a recarga? - e escreva as duas no mesmo commit. Sonda:
`tools/probes/probe_meeting_reset.nvgt`.

**A DISCUSSÃO é contada no cliente, não no servidor.** O `S_VOTING_STARTED` sai colado no
`S_MEETING_STARTED`; o servidor manda `discussion_time` dentro do pacote da reunião e é o cliente que
segura o voto até o tempo passar (o `voting_timer` do servidor é discussão + votação juntas, só como
prazo máximo). Consequência para quem escreve sonda: votar assim que a votação abre produz uma
reunião de UM segundo, e aí "a recarga não andou" e "a recarga foi rearmada" dão exatamente o mesmo
número - a primeira versão da `probe_meeting_reset` "reprovou" a correção certa por isso. Quem quiser
medir o efeito de uma reunião precisa esperar a discussão como um jogador esperaria.

**Regra que o cliente também aplica tem que morar numa função só.** O cliente decide se ABRE o
microfone e o servidor decide QUEM ouve; quando as duas decisões são escritas separadas, elas
divergem e vence a mais restritiva - foi assim que a regra "comunicações calam a reunião", mesmo
DESLIGADA, continuava calando a reunião: o servidor deixava a voz passar e o cliente nem capturava.
Hoje as duas perguntam a `voice_down_for` (em `core/sabotage_rules.nvgt`), que recebe se está em
reunião e o que a sala escolheu; a regra da sala viaja no `S_GAME_START` justamente para o cliente
poder responder igual. Sonda: `tools/probes/probe_voice_meeting.nvgt`.

**Som contínuo precisa de UM dono, e o dono é quem reavalia o estado.** O alarme de sabotagem era
ligado em três lugares (início da sabotagem, volta da reunião) e desligado em outros, e cada caminho
esquecia um: quem morria no meio da sabotagem continuava ouvindo, e a volta da reunião religava para
todo mundo, vivo ou não - foi o recado #96, chegado JÁ com a correção parcial no ar. Hoje quem manda
no alarme é `apply_sabotage_effects`, o mesmo lugar que decide os outros efeitos, e que roda no
começo, no conserto, na MORTE e na volta da reunião. O `meeting_ui` não toca mais nele. Sonda:
`tools/probes/probe_alarme_fantasma.nvgt`, que chama a função de verdade e olha o slot do som.

**Conserto de percepção que "não pega" está do OUTRO lado da rede.** "Sabotagem não atinge fantasma"
foi corrigida três vezes e continuava acontecendo, porque todas as correções foram feitas no CLIENTE.
O radar é uma pergunta ao SERVIDOR, e lá a condição (`radar_targets`) olhava só o papel
(`!immune_to_comms`), nunca se a pessoa estava viva. Pior: o cliente do morto aplica a regra certa e
por isso nem recusa a varredura - o pedido sai, e quem responde "o radar está fora" é o servidor. Do
lado de quem joga isso é indistinguível de "não consertaram nada". Antes de mexer numa regra de
percepção, liste os DOIS lados que a aplicam. Sonda: `tools/probes/probe_ghost_radar.nvgt`, que
cobra as duas metades (o fantasma é atendido E o vivo continua cego) - sem a segunda, um radar que
respondesse porque a sabotagem nem começou passaria. Ela reprova contra a produção da 0.33.0.

**Tela bloqueante que o jogador abre no meio de uma FASE precisa saber que a fase acabou.** O menu de
votação usava `menu.run()`, que é `while (monitor()) wait(5)` e só termina por tecla: a apuração
chegava, a partida seguia, e o jogador continuava escolhendo um nome numa votação encerrada, sem
ouvir a apuração, o chat, a sabotagem nem o tique dos dez segundos finais. São TRÊS peças, e as três
são necessárias: o laço é aberto à mão (`run_until_choice`), o `background_callback` chama
`announce_background_events` (falar sem consumir, como as tasks já faziam) e também o `tick_vote_timer`
(o tique é do laço da partida, que não está rodando), e o fim da votação é marcado na CHEGADA do
pacote (`voting_ended` no game_client), porque varrer a fila por quadro é o que já travou o jogo.
Nada é falado ao fechar de propósito: o laço da partida volta no instante seguinte e anuncia a
apuração inteira - uma frase nossa seria cortada por ela.

**Recurso de UM de cada vez precisa ser recusado no servidor E anunciado ao cliente.** O scanner da
enfermaria guarda um `visible_task_peer` só; com três pessoas entrando juntas, elas se sobrescreviam
ali e o fim do scan de uma não avisava ninguém - o som ficava tocando sem relação com quem estava
nele. Hoje `try_start_visible_task` recusa o segundo, e o cliente sabe que está ocupado pelo mesmo
`S_TASK_VISIBLE` que já recebia, então nem abre a minigame. A mesma forma do posto de câmeras.

**Trabalho que o servidor vai recusar não pode ser oferecido ao jogador.** Duas versões disso no
mesmo lugar: o fantasma abria o painel de sabotagem, resolvia a minigame inteira e só então ouvia
que não valia (o servidor exige estar vivo), e quem chegasse num painel de oxigênio já resolvido
digitava o código para nada. O cliente agora sabe dos dois - pelo `alive` que já tinha e pelo
`S_SABOTAGE_PROGRESS`, que diz QUAL painel saiu do caminho - e recusa na hora, explicando. E o
painel aberto fecha sozinho quando alguém resolve antes (`sabotage_ended` no game_client, marcado na
chegada como o `interrupt_pending`, porque varrer a fila por quadro já travou o jogo uma vez).
Sonda: `tools/probes/probe_reparo.nvgt`.

**Sabotar é a única capacidade que sobrevive à morte.** O impostor morto continua sabotando (como
no original) - matar, ventilar e fechar portas acabam com ele. A regra está em dois lugares que
precisam concordar: `player_abilities.can_sabotage()` (que oferece a tecla) e
`game_state.trigger_sabotage` (que decide). Cuidado ao editar: `close_doors_of_room` tem uma linha
de guarda IDÊNTICA à da sabotagem, e uma substituição descuidada troca a função errada sem que nada
acuse - a sonda `tools/probes/probe_ghost_sabotage.nvgt` cobre as duas justamente por isso.

**Voz posicionada é só para quem está VIVO e andando.** `g_voice.spatial = alive && !movement_frozen`
é decidido a cada quadro, e não em cada transição: são três (morrer, começar a reunião, acabar a
votação) e esquecer uma deixa o jogador num estado que nada mais corrige. Na reunião todo mundo está
na mesma mesa; o fantasma fica onde morreu, então com posição ele ouvia a discussão e os outros
fantasmas - espalhados pelo mapa - de tão longe que virava silêncio. Morto não tem lugar.



**ESPECTADOR: uma lista à parte (`game_state.spectators`), e não jogadores marcados.** Toda a lógica
do jogo - votos, kill, vitória, tarefas, papéis, `find_player` - procura em `players`, então um
espectador não tem como influenciar nada por acidente: um `if` esquecido não existe porque não há `if`
a esquecer. O que ele RECEBE é que é escolhido, caso a caso: `broadcast_to_lobby` inclui os
espectadores (é o canal do que é público); o que vai a um jogador só (papel, parceiros, a morte com o
nome do assassino, a barriga) nunca passa por ela. Laços que mandam pacote público jogador a jogador
(a morte pública em `announce_kill`, o som de engolir) precisam incluir a lista à mão - quem
acrescentar um laço assim, lembre. Decisões do usuário: não vê papel até o fim; a voz dele chega só a
outros espectadores; ouve como quem está ao lado de quem acompanha (`spectator_hears`), e a mesa na
reunião; entra marcando A na sala de espera, ou pelo CÓDIGO de uma sala privada em andamento - a pública
em andamento só aparece na lista, porque nem sempre quem joga se sente tranquilo sendo assistido, e quem
tem o código foi convidado. Pelo mesmo motivo quem joga OUVE quando alguém começa ou para de assistir,
com o nome (`announce_spectator`, recado #185; só a clientes do protocolo 11, que conhecem a fala). Na reabertura da sala os espectadores voltam como membros: quem tinha
marcado A continua marcado, quem entrou pelo código fica para jogar (a não ser que a sala esteja cheia).
No cliente é uma tela própria (`spectate_loop.nvgt`), e não `run_game` com bandeira, pelo mesmo
motivo da lista à parte. Sonda: `probe_spectate.nvgt` (inclusive que nenhum pacote com papel ou
assassino chega a quem assiste).

**MODOS (`lobby_config.game_mode`): pega-pega e todos lobos.** O modo vem da PREDEFINIÇÃO e não é um
campo solto, porque cada modo é um pacote de regras que só faz sentido junto - e `validate()` o AMARRA
(sem duto, sem sabotagem, sem papel especial; pega-pega com 1 pegador), para o formulário do anfitrião
não desmontar o modo depois. Os dois não têm reunião (`meetings_allowed()`, que `start_meeting`, o botão
e o reporte perguntam - e os bots param de tentar reportar), nem porta trancada. A vitória mora num
ramo próprio do tick, ANTES da conferência clássica: todos lobos vence o último de pé (e não o "ele e
mais um" do lobo clássico); pega-pega vence o pegador só com TODOS pegos (um contra um ainda é pega, ao
contrário da maioria), e a tripulação pelas tarefas, com os fantasmas ajudando. A largada do pegador
(`TAG_HEAD_START_SECONDS`) é recusada no servidor (`try_move_player`, kill recarregando 10 s, o bot
parado) e contada no cliente para todos. Decisões do usuário; sonda: `probe_game_modes.nvgt` (sem rede,
com os controles de cada regra) e `probe_game_modes_net.nvgt`.
