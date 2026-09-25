# Som

O que toca, para quem e com que volume, e como os arquivos de áudio são organizados. Leia antes de acrescentar, mexer ou silenciar qualquer som.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## Som: o que toca, quando e para quem

**Caminho de arquivo montado à mão é bomba-relógio.** Depois que os sons foram para subpastas, a task
de rever a gravação continuou pedindo `sounds/Tile3.wav` - arquivo que não existe mais - e rodava
MUDA: o NVGT não reclama de som que não existe, e o `check_sounds` não pega, porque só compara o
catálogo com o disco e esse caminho era construído dentro da task. Todo caminho de som sai de uma
função do catálogo (`footstep_sound_path()`, por exemplo), nunca de concatenação local.

**Quem percebe QUEM se decide no servidor, não no cliente.** O cliente recebe a posição de todo mundo
(é o que faz o áudio posicionado funcionar), então é tentador montar "quem está nesta sala" ali mesmo
- e a câmera de segurança nasceu assim. Só que o cliente não sabe quem está dentro de um duto: isso é
privado de quem entrou, e tem que continuar sendo. A câmera então contava o impostor escondido,
entregando a dedução que o duto existe para negar; mandar o estado de duto para todos os clientes
consertaria a lista criando um vazamento pior. A regra mora em `game_state.players_in_room()`, e o
radar e a câmera são duas perguntas para a MESMA função.

**Regra de percepção fica em `src/core/sabotage_rules.nvgt`, nunca espalhada.** Quem é atrapalhado
por qual sabotagem se decide num lugar só. Espalhada como `if` em cada ponto (marcadores, radar, som
de corpo, alcance de audição), bastava esquecer um para o jogador ficar cego pela metade sem que nada
denunciasse. É também onde papéis novos vão declarar o que enxergam.

**`audio_form` nasce SEM foco em controle nenhum** (`control_focus = -1` em `form.nvgt`) — chame
`f.focus(<primeiro campo>)` depois de criar os controles, em todo form. Sintoma: o jogador abre a
tela, digita, aperta Enter e nada acontece; funciona depois que ele aperta Tab. Foi assim com a caixa
de mensagem do chat, o usuário no primeiro login, o nome da partida e os volumes.

**Espera que consome um pacote não pode jogá-lo fora se ele for o único.** As esperas por resposta do
servidor tiram pacotes da fila com `poll_message()`, e o que não for o pacote esperado **some**. A
confirmação da criação da sala é o próprio `S_LOBBY_STATE` - o único retrato que o anfitrião sozinho
recebe, já que o próximo só vem quando alguém entra. Descartá-lo deixava P e C mudos até chegar a
segunda pessoa. Quem espera devolve o pacote a quem chamou.

**Não espere um som acabar com `wait` de duração fixa.** O número é um chute da duração do arquivo, e
quem trocar o som depois não tem como saber que havia um `wait` casado com ele. Falha em silêncio:
dois sons por cima um do outro, sem erro. A fita da sala de segurança dura 3457 ms e o código
esperava 1200 — os primeiros passos, que a task pede para CONTAR, tocavam por cima do chiado. Use
`wait_for_task_sound()` (em `src/game/tasks/task_common.nvgt`), que espera `sound.playing` virar
false sem parar de bombear `client.update()`. Som que não existe devolve handle nulo, e a espera
passa direto.

**`play_wait()` e `wait(n)` seco não têm lugar dentro de uma minigame.** `play_wait` prende a
thread inteira até o som acabar: nada de rede, nada de ESC, e - o sintoma que chegou no recado #97
como "o jogo não responde na tarefa dos coletores" - nenhuma tecla é vista. Quem decorou a
sequência digita rápido, e a tecla apertada durante o eco da anterior simplesmente sumia; o
jogador ficava esperando uma resposta que não vinha. Som que precisa acabar antes do próximo
passo: `wait_for_task_sound(task_sound(..., pitch), client)`; pausa: `task_wait(ms, client)`; eco
de tecla: toca e NÃO espera. Medido em `tools/probes/probe_play_wait.nvgt`: o `play_wait` em si
volta direito (504 ms no tom grave), o problema é o que não acontece enquanto ele não volta.

**Task visível (o scan) é prova porque o SERVIDOR a anuncia, não o cliente.** O cliente manda
`C_START_TASK` ao abrir qualquer minigame; só para o scan o servidor confere que aquele jogador tem a
task pendente e está vivo, e então manda `S_TASK_VISIBLE` a todos menos a ele. Impostor não tem a
task, então nada é anunciado por mais que o cliente dele finja - é isso que faz o som do scanner na
enfermaria valer como álibi. O fim vem pelo `C_TASK_INPUT` (sucesso ou cancelamento), e reunião,
morte e saída também encerram (`end_visible_task`), senão o scanner soaria numa sala vazia.

**"Task concluída" é UM som, tocado num lugar só.** `SND_TASK_COMPLETE` sai do `task_manager` ao
fechar a task com sucesso (e de `run_sabotage_panel` para reparos). Nenhuma task toca um som de
"acabei" por conta própria: `BeepComplet` e `CardComplet` são feedback de ETAPA (lixo puxado, cartão
aceito) e só entram onde a etapa existe. Antes cada task tocava um som diferente no fim, por cima do
som certo.

**Cada som tem UM ouvinte pretendido — decida quem antes de tocar.** Num jogo em que a informação é o
som, tocar para todo mundo entrega de graça o que devia custar. O assassinato são dois sons com
públicos diferentes: a **morte** é pessoal e só a vítima ouve; o **kill** é espacial e é o único
sinal do crime para quem está por perto. Tocar os dois para todos, como era antes, dava o crime de
graça aos vizinhos e fazia a vítima ouvir a própria morte como se fosse de outra pessoa.

**Fantasma não é atingido por sabotagem.** Ele já perdeu o que tinha a perder, e continua fazendo
tarefas pelo time — cegá-lo não cria tensão, só torna tedioso o que ainda ajuda.


## Arquivos de som

**Todo som do jogo é `.ogg`** (Vorbis, qualidade 3), fora dois `.mp3` antigos. Foram 45 MB de wav
virando 3,4 MB sem perda audível, e o download caiu de 60 MB para 29 MB. Som novo entra convertido:
`D:\Program\winvox\ffmpeg.exe` (existe na máquina, mas **não está no PATH**) com
`-c:a libvorbis -q:a 3`. Depois da conversão, o maior peso do zip passou a ser `lib/phonon.dll`
(17,9 MB comprimido), que é o motor de áudio posicionado e não sai.

**Volume de som se mede, não se chuta - e depois se confere de ouvido.** Os arquivos de passo vinham
com até 15 dB de diferença entre pisos, o que num jogo onde o passo alheio é a pista principal deixa
uma sala segura por acidente. A correção é por piso, em dB, em `footstep_volume_db()` - no código, e
não nos arquivos, para ajustar sem reconverter. Duas armadilhas na medição: RMS do arquivo inteiro
inclui o silêncio do fim e subestima som curto (medir só o que está acima de 1% do pico), e mesmo a
medida certa erra para mais - o Tile pedia +17,6 pela conta e ficou alto demais, voltando para +15.

**Família de volume nova entra em QUATRO lugares.** `SOUND_FAMILY_<NOME>` (índice), `..._COUNT`,
`SOUND_FAMILY_IDS` (o id vai para o disco como `volume_<id>`, então acrescente sempre no FIM para
não mexer no que o jogador já ajustou), `SOUND_FAMILY_NAME_KEYS`, e `FAMILY_PREVIEW_SOUNDS` em
`ui/settings_screens.nvgt` - as três listas são indexadas pelo mesmo número, e uma menor que
`COUNT` é leitura fora do array na tela de volumes. Mais as chaves de texto nos dois idiomas.

**A medição de piso virou ferramenta: `python tools/measure_footsteps.py <prefixo>`.** Ela dá o corpo
em dBFS, a correção sugerida contra o MetalTile (a referência) e - o número que decide - o **ruído de
fundo** depois da correção. Um piso muito baixo só pode ser levantado se o silêncio entre as batidas
for silêncio de verdade: o vidro pediu +19,2 dB e isso só passou porque o ruído dele está em -70,5
dBFS, indo para -53 com o ganho, praticamente o mesmo -54,8 do próprio MetalTile. Ela reproduz os
números que já estavam no código (EarthTile, O2Tile) dentro de ~1,5 dB, e a diferença é sempre a
mesma: **a conta exagera e o ouvido puxa de volta** (o Tile pedia +17,6 e ficou em +15; o vidro pede
+19,2 e ficou em +17). Aplique a conta, depois confira de ouvido.

**Piso novo entra em DUAS tabelas, e esquecer uma falha calado.** `footstep_variant_count` tem um
fallback de 2 variantes: um piso com oito arquivos que não esteja na tabela toca sempre os dois
mesmos passos, e nada acusa - o som existe, só soa repetitivo. `FOOTSTEP_FLOOR_PREFIXES` é a outra:
sem ela os arquivos não entram no `sounds.dat` e o piso fica mudo no jogo compilado (funciona
rodando do fonte, que é o pior jeito de descobrir). E o ganho do piso se MEDE: o O2Tile veio 9 dB
acima da referência, com picos a -1,7 dB - sem correção, uma sala de seis por seis viraria a mais
barulhenta do mapa por acidente de gravação. Sonda: `tools/probes/probe_floor.nvgt`.

**Tela que ABRE com som tem que FECHAR com som.** Os painéis de luzes e de oxigênio abriam com a
tampa (`SND_OPEN_PANEL`, o som de interagir com o objeto no mapa) e terminavam em silêncio - nada
marcava o fim, nem ao concluir nem ao desistir. O `panel_close.ogg` existia no repositório e nunca
tinha sido tocado; hoje sai em `run_sabotage_panel`, nos dois fins, porque o som é da TAMPA e não do
resultado. O de comunicações fica de fora: ele tem o par próprio (`SND_COMMS_PANEL_*`) e tocar os
dois seria a mesma tampa fechando duas vezes. As tasks já tinham o par (`SND_TASK_INPROGRESS` ao
abrir, `SND_TASK_COMPLETE` ao terminar) - a assimetria era só dos painéis.

**Nome de arquivo de som é `snake_case`, e a PASTA diz o que o som É** - não onde ele toca:
`events/` é acontecimento do mundo (morte, alarme, transformação, vitória), `beacons/` é marcador
posicionado contínuo (o corpo no chão está aqui, não em events), `ui/` é interface (menu, votação,
chat), `world/` é objeto do cenário (porta, painel, esbarrão), `ambience/`, `steps/`, `tasks/`. Os
nomes foram uniformizados de uma vez (`bodyReport` -> `body_reported`, `emergence` ->
`sabotage_alarm`, `killed<N>` -> `death<N>`, que se confundia com `kill`); só `steps/` ficou como
estava, porque os nomes ali são PREFIXOS lidos pelo código (`FOOTSTEP_FLOOR_PREFIXES`).

Renomear som é seguro **desde que** o `check_sounds` rode depois: ele compara catálogo e disco nos
dois sentidos e é a única coisa que pega um caminho errado - som que não carrega falha em silêncio.
Cuidado com as referências CONCATENADAS (`"sounds/events/death" + i + ".ogg"`, o prefixo em
`sound_variants_list`): elas não aparecem numa busca pelo caminho inteiro.

**Som novo vai na subpasta certa de `sounds/`** (`steps`, `ambience`, `beacons`, `tasks`, `events`,
`ui`, `world`) e o caminho no catálogo inclui a pasta. Depois de mexer em som, rode
`nvgt tools/check_sounds.nvgt`: ele compara o catálogo com o disco **nos dois sentidos** e é a única
coisa que pega um caminho errado — som que não carrega falha em silêncio, sem erro nenhum.


## Sinais sonoros e preferências de audição

**O bip de alvo ao alcance é SÓ de quem pode agir, e só na TRANSIÇÃO.** Impostor, xerife e anjo
ouvem quando alguém entra no alcance da ação deles - é o equivalente sonoro do botão de matar
acendendo no original. Tocar no ALVO avisaria a vítima de que o impostor está do lado dela, então
ele nunca sai para mais ninguém. Na transição, e não contínuo, pela regra de sempre: num jogo em que
se está tentando ouvir passos, um bip repetido por cima é ruído. E ele usa a MESMA
`roster.nearest_alive` (com as mesmas exclusões) da tecla que vai agir, senão prometeria um alvo que
a tecla depois recusa. Pode ser preferência pessoal, e não regra da sala, porque **não cria
informação nova**: a posição de todos já chega ao cliente, isto é só apresentação.

**O quanto o tom cai para o sul é do JOGADOR** (`spatial_pitch_percent`, padrão 6%, teto 25%): há
quem não distinga norte de sul com o padrão e queira reforçar, e quem ache que variar o tom
descaracteriza os passos - o piso se reconhece pelo timbre - e prefira desligar. O `sound_pool`
GUARDA o valor, então mexer na preferência exige `apply_pitch_setting()` no pool que está tocando
(`g_active_spatial`); sem isso o ajuste só valeria na partida seguinte e o jogador testaria achando
que não funcionou. Os dois ajustes ficam numa tela PRÓPRIA ("Como eu ouço o jogo"), separada dos
volumes: eles mudam a INFORMAÇÃO que o som carrega, não o quão alto ele é.

**Um sinal sonoro, um significado.** O tom do bip do radar dizia a DISTÂNCIA (130 perto, 80 longe)
enquanto o tom de todo o resto do jogo diz NORTE/SUL (`SPATIAL_SOUTH_PITCH_DECREASE`, 6%). Os 50%
da distância engoliam os 6% da direção, e o radar deixava de responder à única pergunta que o
estéreo não responde sozinho: "está acima ou abaixo de mim?". Hoje o tom do radar é só direção, e a
distância fica por conta da atenuação do som posicionado. Antes de usar tom, volume ou repetição
para uma informação nova, veja o que aquele canal já significa em outro lugar do jogo.


