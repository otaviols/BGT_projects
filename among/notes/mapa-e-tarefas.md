# Mapa e tarefas

Zonas, dutos, sorteio de tarefas e como se inventa uma tarefa nova. Leia antes de mexer no mapa ou criar um minijogo.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## Mapa e tarefas

**O grafo de navegação sai do NOME do corredor - e só sabe ligar sala a sala.** `zone_neighbors`
parte `corridor_<sala_a>_<sala_b>` em três e é assim que os bots acham caminho. Uma sala pendurada
num CORREDOR (a do oxigênio, que se abre para o corredor de carga) não cabe nessa forma: o id teria
quatro partes, o grafo a ignoraria e os bots nunca chegariam lá - sem erro nenhum, só bots que não
vão àquela sala. Para esses casos existe `zone_link`/`extra_links`, uma ligação declarada à mão.
Ao acrescentar zona nova, confira as duas coisas separadamente: a GEOMETRIA decide por onde o
jogador anda (`zone_at`/`can_move` são puramente retangulares, e zonas não podem se sobrepor, senão
o jogo diz a sala errada conforme qual for encontrada primeiro), e o GRAFO decide por onde o bot
anda. Sonda: `tools/probes/probe_o2room.nvgt`.

**Ninguém mata de dentro do duto, e ninguém morre dentro dele - a regra vale para as duas pontas
e mora no topo do `try_kill`.** A razão é uma só: a posição de quem está num duto PARA de ser
transmitida (ver `on_move`), então o atacante está mirando o último lugar conhecido, e não onde a
pessoa está - matar por posição congelada é matar através da parede.

Do lado do ATACANTE foi um recado de jogador: escondido, fora do radar e da câmera, ele alcançava
quem passasse ao lado da tampa - assassinato sem contra-jogada, porque não havia como saber que ele
estava ali.

**Invisibilidade não entra nesta regra, em ponta nenhuma.** O fantasma mata e fica visível, que é o
desenho combinado do papel; e proibir matá-lo enquanto sumido seria uma mudança de equilíbrio por
tabela. Eu tinha posto `target.invisible()` junto do duto ao consertar isto, e o usuário cortou: um
conserto de duto mexe em duto. Vale a regra geral - quando uma correção "de graça" cair num papel
que não estava em discussão, ela não é de graça.

Do lado do ALVO era pior do que parece, e é o "bugando" do recado: o morto continuava com `vented`
ligado, e **fantasma ventilado não anda** (`try_move_player` recusa) **nem sai** (o menu do duto
exige estar vivo) - travava no lugar pelo resto da partida, sem mensagem nenhuma. É a MESMA armadilha
que a reunião já tinha tido, aparecendo por outra porta; quando aparecer uma terceira, o lugar de
consertar continua sendo a origem, não o sintoma. Sonda: `tools/probes/probe_vent_kill.nvgt`, que
confere também que o mesmo kill funciona um instante depois com os dois fora do duto - sem isso, uma
recusa por recarga ou alcance passaria por "o duto protegeu".

**Duas redes de dutos, e elas não podem se tocar.** A rede se declara nos `linked_object_id` de
cada duto (lista separada por vírgula), e é o que permite ao jogador deduzir para onde alguém pode
ter ido: "a tampa abriu na navegação, então ele saiu em armas ou no reator". Ligar todos os dutos
numa rede só mataria essa dedução - por isso a rede nova (corredor jogos-estufa ↔ corredor da
segurança) é SEPARADA da antiga, e a sonda `tools/probes/probe_vents.nvgt` confere justamente que
uma não alcança a outra. Duto em corredor tem um custo que duto em sala não tem: o som da tampa é
ouvido por quem está passando. E rede de dois dutos é determinística de propósito (quem ouve sabe
onde ele vai sair) - é a troca de velocidade por previsibilidade.

**Um tipo pode ter VÁRIAS correntes, e a escolhida mora no JOGADOR.** O destino saía de
`map.chain_object_at(tipo, fase)`, que é global: todo mundo com "abastecer os motores" ia ao mesmo
lugar, e rota única é informação de graça para quem deduz - sabia-se de antemão por onde quem estava
abastecendo teria que passar. Hoje o mapa declara duas correntes para `fuel_engines` (depósito ->
reator e depósito -> motor leste), `map.chains_starting_at()` devolve as que começam no ponto
sorteado, o servidor escolhe UMA no sorteio e ela fica em `player_task.chain`. **O cliente não mudou
nada**: ele já só seguia o `object_id` da fase atual.

Duas coisas para lembrar ao acrescentar uma corrente alternativa: as alternativas têm que começar no
MESMO ponto (senão o sorteio entrega uma tarefa que começa noutro lugar - por isso o filtro é por
ponto de início, e não só por tipo), e **sonda que fixa o destino quebra**. A `probe_phases` mandava
`task_fuel_reactor` na segunda fase; com dois destinos ela acertava metade das vezes e, na outra
metade, o servidor não achava a tarefa, não respondia, e a sonda ficava esperando até o prazo -
sintoma "a sonda trava de vez em quando". Hoje ela usa o `next_object_id` que a resposta traz.

E uma propriedade que a sonda do motor descobriu: **o servidor NÃO confere distância para concluir
tarefa** - ele confia no cliente, que só abre a minigame quando está perto. Isso torna a sonda barata
(dá para avançar fases sem caminhar), e é bom saber que está assim de propósito ou não.

**Tarefa de VÁRIAS FASES: a sequência mora no mapa, a fase mora no servidor.** `task_chain`
(`game/map.nvgt`) lista os pontos de uma tarefa na ordem, junto dos objetos - separada deles, um id
renomeado quebraria a corrente em silêncio. `player_task` carrega `phase`/`phase_count` e o
`object_id` da fase ATUAL, então marcador, lista de tarefas e `find_interactable` seguem a fase sem
saber que ela existe. Quem avança é `game_state.advance_task`, no servidor: quem for interrompido
por uma reunião no meio da travessia volta com o galão na mão, e não do começo - punir quem reporta
um corpo seria punir o comportamento que o jogo quer. Três armadilhas que já custaram: a contagem
da equipe conta FASES (`task_count` soma `phase_count`), senão a barra congela durante a travessia
inteira; só o PRIMEIRO ponto da corrente entra no sorteio (`is_chain_start`), senão alguém recebe a
segunda metade de uma tarefa que nunca começou; e o modo de treino roda todas as fases em sequência,
senão "treinar" a tarefa é só a etapa que não tem o que treinar. Sonda:
`tools/probes/probe_phases.nvgt`.

**O sorteio de tarefas tem DOIS tetos, e os dois são do sorteio, não da sala.** Antes ele era
independente por jogador, e a variância mandava.

A **tarefa visível** (hoje só o scan) sai para no máximo `visible_task_cap(jogadores)` pessoas -
20% arredondado para cima, 1 numa sala de 5 e 2 numa de 10. Ela é a única prova de inocência do
jogo, e sem teto uma rodada em cada tantas entregava álibi a quase todo mundo (medido: 4 dos 5
tripulantes numa sala de 6). Há um motivo mecânico junto do de equilíbrio: o scanner é recurso de
UM DE CADA VEZ no servidor, então muita gente com essa tarefa vira fila e "ocupado". 10% foi
considerado e descartado - daria exatamente 1 em qualquer sala até 10 pessoas, o que faz do álibi
um bilhete de loteria em vez de uma economia. **"Visível" é uma LISTA** (`VISIBLE_TASK_TYPES`), e
não um `task_type == "submit_scan"` solto: a segunda tarefa visível precisa nascer com teto e com
anúncio, e a lista é o que obriga a lembrar dos dois.

E **no máximo `MAX_TASKS_PER_ROOM` (2) tarefas do mesmo jogador na mesma sala do mapa** - para
ninguém resolver a lista inteira num canto, num jogo em que andar é fazer barulho. É
**preferência, não parede**: se respeitá-la impedisse fechar o total pedido, o total ganha (a mesma
escolha que o sorteio já faz entre longa e curta). Só morde em navegação (4 pontos) e admin (3); as
outras salas já não têm mais que 2. Quem escreve o sorteio: `pick_task_spots` recebe `want` como
QUANTO ACRESCENTAR, e não como tamanho final - as duas chamadas (longas e curtas) escrevem no mesmo
array, e comparar com `destino.length()` cru fazia a segunda achar a cota cumprida e devolver um
jogador só com as tarefas longas, sem erro nenhum. Sonda: `tools/probes/probe_task_limits.nvgt`
(`probe_task_mix` não pegou porque só IMPRIME o que saiu, sem cobrar o total).

**Tarefa NOVA entra em OITO lugares, e a maioria falha em silêncio.** Na ordem em que se esquece:
o minigame (`game/tasks/<tipo>.nvgt`), o `#include` **e** o `else if` do `task_manager` (só o
include compila e a task nunca abre), o ponto no `map.nvgt` com beacon, `PRACTICE_TASKS` em
`ui/practice_screen.nvgt` (sem isso não dá para testá-la sem montar partida), o beacon em
`ui/onboarding_screens.nvgt` ("Conhecer o mapa"), as constantes **e a lista do build_pack** em
`audio/sound_catalog.nvgt` (sem a lista, o som não entra no `sounds.dat` e a task fica muda só no
jogo compilado), as chaves nos DOIS idiomas, e a seção de tarefas dos dois `docs/README_*`. Decida
também se ela é longa (`LONG_TASK_TYPES`) - o padrão é curta.

**O jeito que funciona de inventar tarefa: pegar a ORIGINAL e traduzi-la para o ouvido.** Foi a
correção do usuário depois de eu propor três levas ruins (tarefas de mundo, mais painéis, e a
mistura revertida). A pergunta certa não é "que mecânica de ouvido falta?" - é **"o que o jogador
faz com as mãos no original, e como isso vira um gesto de ouvido?"**. A fiação já era isso e é a
melhor tarefa do jogo. A biblioteca de sons do usuário (fora do repositório,
`D:\documents\sons among us\sounds among`) tem o conjunto COMPLETO de várias originais que ainda não
existem aqui: calibrar o distribuidor, inserir as chaves, rodar diagnóstico, traçar a rota, ativar
escudos, desviar energia, nó do clima, separar amostras.

`clean_o2_filter` é a primeira feita assim. No original as folhas estão à vista e você arrasta cada
uma até a abertura; aqui elas estão escondidas em seis posições, o aspirador anda entre elas com as
setas, e o som responde: **tique seco = vazio, tique + farfalho = folha**. A versão anterior da
maquete tocava tudo sozinha e o veredito foi "ficou automático" - **o jogador tem que decidir, não
assistir**, e é o mesmo erro que a primeira versão do `water_plants` teve. Três decisões que a
seguram: seis posições para três folhas (com uma em cada, bastava apertar Enter seis vezes sem ouvir
nada); as folhas **se mexem** a cada aspirada, senão dava para mapear a câmara numa varredura e o
resto virava digitação; e o beacon é o MESMO arquivo do ar que toca dentro da task, então de longe se
ouve o filtro puxando e de perto aquele puxar vira o norte fixo.

`calibrate_distributor` é a segunda, e o critério de escolha foi outro: das doze minigames, **nenhuma
pedia acerto de TEMPO** - todas eram "ache e escolha". Foi a mecânica que faltava, e o original já era
ela. É também a tradução mais fácil para o ouvido que existe, porque julgar QUANDO dois sons
coincidem é algo que o ouvido faz muito melhor do que julgar ONDE um som está.

Quatro decisões dela, e o que cada uma evita:

- **Duas pistas para a mesma posição**: o clique caminha no estéreo E sobe de tom. Redundância de
  propósito - quem se guia mal pela panorâmica usa o tom, e vice-versa. Aqui **tom significa
  posição**, o que localmente contraria "um sinal sonoro, um significado": passa porque a referência
  é ENSINADA no começo de cada mostrador (o alvo toca duas vezes, no lugar e no tom dele) e porque
  numa tela fechada não há passo nem marcador disputando aquele canal.
- **O ponteiro é CLIQUE, não zumbido.** O som do distribuidor girando existe e é usado - mas só como
  beacon. Dentro da task ele seria um contínuo por baixo de oito transientes, que é a parede que a
  `stabilize_lines` ensinou a evitar.
- **O passo não desce abaixo de ~200 ms**, que é a reação humana a um som esperado. O mostrador mais
  rápido tem 175 ms e é o limite; quem quiser mais rápido tem que reduzir as POSIÇÕES, não o passo,
  senão acertar deixa de ser ouvir e passa a ser adivinhar. A sonda cobra esse número.
- **O Enter é julgado contra a posição do ÚLTIMO CLIQUE que soou**, e não contra onde o ponteiro
  "estaria" naquele instante. O jogador aperta porque ouviu, e o som já aconteceu quando o dedo
  desce: cobrar o instante puniria a reação em vez da escuta. Apertar tarde cai no clique seguinte -
  erro dele, não do jogo.

Errar não reinicia nada e o ponteiro não para: a punição é o tempo da volta. Sonda:
`tools/probes/probe_distributor.nvgt`, que confere também as CONTAS (as duas pistas variando sempre
no mesmo sentido, e vizinhas suficientemente separadas) - uma tabela de posições errada compila,
roda, e só produz uma tarefa impossível de acertar sem sorte.

**MAQUETE ANTES DE CÓDIGO.** Monte a ideia com `ffmpeg` (um `.wav` com os sons nas posições e nos
tempos certos) e OUÇA antes de escrever a task. Custa dez minutos; a task revertida custou uma
sessão. Duas armadilhas da maquete em si: `adelay=0|0` é recusado (o evento em zero não leva o
filtro), e **make-up gain demais + `alimiter` achatam tudo no mesmo nível** - foi assim que o "tique
vazio" e o "tique com folha" saíram idênticos numa medição, que é justamente a informação que a task
existe para carregar. Meça janelas do arquivo com `volumedetect` em vez de confiar no ouvido para o
balanço: no filtro, o farfalho ficou ~9 dB acima do tique, e esses números viraram as constantes em
dB no topo da task.

**SOM CONTÍNUO NÃO SE LOCALIZA, e vários ao mesmo tempo viram parede.** Isto custou uma tarefa
inteira, escrita e revertida (`stabilize_lines`, commit 53341d0 e a reversão logo atrás - dá para
recuperar o código de lá se um dia servir). A ideia era "ouvir para dentro de uma mistura": quatro
linhas zumbindo juntas, cada uma numa posição, e o jogador diria qual estava oscilando. O veredito
de quem ouviu foi "uma miscelânea de sons todos juntos, não está localizado, ficou horrível" - e
estava certo.

O erro é de acústica, não de ajuste: **o ouvido localiza por ATAQUE**. Um zumbido constante quase
não tem transiente, então mesmo panoramizado ele vira um borrão largo em vez de um ponto; quatro
deles se mascaram e o resultado é ruído. Mistura de verdade - um passo por cima da ambiência da sala,
que é o que o jogo faz o tempo todo - funciona porque há **um fundo estável e UMA coisa intermitente
dentro dele**. Quatro primeiros planos não são uma mistura, são uma parede.

Regra que sai daí, para qualquer tarefa futura: som que precisa ser localizado tem que ter ataque, e
no máximo um elemento contínuo por vez. Antes de escrever a task, monte a ideia com `ffmpeg` e ouça
- misturar quatro arquivos e escutar custa um minuto, escrever a task custou uma sessão.

**Tarefa longa vale mais aqui do que no jogo original.** `LONG_TASK_TYPES` (em
`config/game_constants.nvgt`) marca as tarefas que fazem atravessar a nave ou ficar parado um bom
tempo; o que não é comum nem longo é CURTO, sem terceira lista para sair de sincronia. O motivo não
é paridade com o original: nesta versão andar é fazer barulho, e barulho é a moeda do jogo - uma
tarefa curta feita num canto não produz informação para ninguém, uma longa é álibi para quem a faz
e janela para o impostor. O sorteio é por categoria (`on_start_game`), e quando o mapa não tem
pontos suficientes de uma delas o resto vem da outra: entregar o total pedido importa mais do que a
proporção exata. A configuração é **total + quantas longas** (não uma contagem por categoria): as
curtas são o resto, ninguém faz conta, e as versões anteriores, que só mandavam o total, continuam
entendidas. Sonda: `tools/probes/probe_task_mix.nvgt`.


