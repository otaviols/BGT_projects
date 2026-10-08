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

**As SAÍDAS de uma sala saem da geometria** (`game_map.openings_of`): toda zona que encosta numa
parede abre uma passagem na faixa de sobreposição. É o que alimenta o aviso de "alinhado com uma
saída" (`game/exit_alignment.nvgt`, pedido antigo dos jogadores): dentro de uma SALA, entrar na faixa
de uma passagem toca um som do lado dela, uma vez; volta a tocar só depois de sair do eixo e voltar.
Corredor não avisa (está sempre alinhado com as duas pontas), e a passagem por onde se entrou fica
calada. Zona nova ganha as saídas sozinha - mas só se ENCOSTAR de verdade (bordas iguais, com
tolerância de 0,01); zona com uma folga entre as paredes fica sem saída e sem aviso. Sonda:
`tools/probes/probe_exit_alignment.nvgt`, que lista as saídas de toda sala (para ler) e simula alguém
andando. O som é provisório (`SND_EXIT_ALIGNED`, o clique de menu).

**No Conhecer o mapa, Enter num ponto de tarefa abre a tarefa** (recado #217; decisão do usuário:
TODAS as do mapa). Usa o mesmo `run_task_minigame` e o cliente desconectado do treino
(`make_offline_client`, por isso o `explore_screen` inclui o `practice_screen`). Cada ponto abre a
SUA fase (`explore_phase_of`): o depósito é pegar o combustível, o motor é despejar - o caminho entre
os dois é o jogador que faz, como na partida, e é justamente o que o menu de treino não ensina.
Ponto e tipo novos entram sozinhos (é o `task_manager` que abre); a sonda
`probe_explore_tasks.nvgt` confere que todo ponto é de um tipo que também está em `PRACTICE_TASKS`.

**A porta trancada é a BORDA da sala, não o corredor** (0.50.9, recado #220). `map.closed_door_zones`
guarda ids de SALA, e `game_map.door_blocks` - a regra inteira, usada pelo `can_move` e pelo aviso de
"porta trancada" - recusa o passo que troca de zona quando uma das duas está trancada. Até a 0.50.8 a
porta era o corredor inteiro, e isso dava dois defeitos: quem estava no corredor entrava na sala
trancada (para não congelá-lo, a regra deixava sair por qualquer ponta), e o corredor da carga para a
elétrica, onde o oxigênio está pendurado, trancava o oxigênio junto com a carga ou a elétrica. O som
da porta toca em cada passagem (`openings_of`), não no meio do corredor. Sonda:
`tools/probes/probe_doors.nvgt`, que acha as passagens pela geometria em vez de coordenadas fixas.

**Um tipo pode ter VÁRIAS correntes, e a escolhida mora no JOGADOR.** O destino saía de
`map.chain_object_at(tipo, fase)`, que é global: todo mundo com "abastecer os motores" ia ao mesmo
lugar, e rota única é informação de graça para quem deduz - sabia-se de antemão por onde quem estava
abastecendo teria que passar. Hoje o mapa declara duas correntes para `fuel_engines` (depósito ->
reator e depósito -> motor leste), `map.chains_starting_at()` devolve as que começam no ponto
sorteado, o servidor escolhe UMA no sorteio e ela fica em `player_task.chain`, que vai ao cliente no
`S_GAME_START` (campo `chain` de cada tarefa).

**O cliente também calcula o destino, e calculava pelo MAPA.** Esta nota já disse "o cliente não
mudou nada" - era falso: ao terminar uma fase, o cliente avança sozinho, sem esperar o servidor
(`advance_task_locally`), e o próximo ponto saía de `map.chain_object_at`, que só conhece a PRIMEIRA
corrente. Quem foi sorteado para o motor leste ouvia "leve ao reator", entregava lá e o servidor, que
esperava no motor, não achava a tarefa e ficava calado. Sintoma: ninguém nunca "pegou a variação do
motor", a tarefa soava concluída para o jogador e a barra da equipe nunca chegava a 100%. A
correção da resposta do servidor (`S_TASK_RESULT`) não salvava, porque procura a tarefa pelo ponto
antigo, que o cliente já tinha trocado. Hoje os dois lados perguntam a `player_task.chain_object(fase)`.
A `probe_phases` não pegava isso porque seguia o destino da RESPOSTA do servidor, e não fazia o que o
cliente faz. Hoje ela calcula como o cliente e joga até ver os dois destinos.

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

**Inventar e montar uma tarefa nova tem arquivo próprio: [tarefas-novas.md](tarefas-novas.md).** A
receita (original -> tradução para o ouvido -> maquete em ffmpeg -> ouvir -> código), os oito lugares
onde uma tarefa nova se registra, os dois casos que produziram a receita, a lição do som contínuo e a
fila de ideias estão lá. Aqui fica o que o mapa e o sorteio fazem com a tarefa depois de ela existir.
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


