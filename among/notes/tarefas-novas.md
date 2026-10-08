# Tarefas novas: como inventar e como construir

A receita que funcionou, os dois casos que a produziram, e a fila de ideias. Leia antes de propor ou
escrever um minijogo.

Isto é sobre INVENTAR e MONTAR uma tarefa. O que o mapa e o sorteio fazem com ela depois - pontos,
fases, tetos, longa ou curta - está em [mapa-e-tarefas.md](mapa-e-tarefas.md).

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## A receita, na ordem

Os passos estão nesta ordem porque pular um deles já custou caro, e o que custou está escrito
abaixo de cada um.

1. **Pegue uma tarefa ORIGINAL do jogo.** Não invente a mecânica do zero (ver o porquê logo
   abaixo). A biblioteca de sons do usuário - fora do repositório, na pasta de documentos dele -
   tem o conjunto COMPLETO de várias que ainda não existem aqui; a fila no fim deste arquivo diz
   quais.
2. **Pergunte o que o jogador faz COM AS MÃOS no original**, e só depois como aquilo vira um gesto
   de ouvido. "Que mecânica de ouvido falta?" é a pergunta errada e produziu três levas recusadas.
3. **Escolha pela mecânica que o jogo NÃO tem.** Foi assim que o distribuidor entrou: das doze
   minigames de então, nenhuma pedia acerto de tempo. Uma tarefa que repete a mecânica de outra
   acrescenta trabalho, não jogo - e espalhar a MESMA tarefa por mais salas foi recusado
   explicitamente ("fica estranho").
4. **MAQUETE EM FFMPEG, e o usuário OUVE, antes de existir código.** Não é etapa opcional: é a
   etapa que separou as duas tarefas que ficaram das duas que foram jogadas fora.
5. **Meça o balanço com `volumedetect` em janelas**, não de ouvido. Os números viram constantes em
   dB no topo da task - lá, e não nos arquivos, para ajustar sem reconverter nada.
6. **Escreva a task** e registre nos oito lugares (abaixo).
7. **Sonda que confira as CONTAS**, e não só que o ponto existe. Uma tabela de posições errada
   compila, roda e só produz uma tarefa impossível de acertar sem sorte.
8. **O usuário ouve no modo de treino** ("Praticar tarefas e reparos") antes de publicar.

E a regra que atravessa tudo: **o jogador tem que DECIDIR, não assistir.** As duas primeiras
maquetes tocavam a tarefa sozinhas e o veredito foi "ficou automático né" - o mesmo erro que a
primeira versão do `water_plants` teve.

## De onde a receita veio

Ela não foi pensada: foi a correção do usuário depois de eu propor **três levas ruins** (tarefas de
mundo, mais painéis, e a mistura que acabou revertida). A frase dele é o passo 1 e o passo 2 inteiros:
pegar a ORIGINAL e traduzi-la para o ouvido, em vez de perguntar que mecânica de ouvido falta.

O argumento que fechou a questão: **a fiação já era isso** - uma tarefa original traduzida - e é a
melhor tarefa do jogo. Não havia por que procurar método novo quando o que funcionava já estava lá.

A biblioteca de sons do usuário fica fora do repositório, na pasta de documentos dele (era
`D:\documents\sons among us\sounds among` na máquina antiga - ele confirma o caminho quando precisar).
Ela tem o conjunto COMPLETO de várias originais que ainda não
existem aqui: calibrar o distribuidor, inserir as chaves, rodar diagnóstico, traçar a rota, ativar
escudos, desviar energia, nó do clima, separar amostras.

## Os dois casos que a receita produziu

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

## A maquete, e as duas armadilhas dela

**MAQUETE ANTES DE CÓDIGO.** Monte a ideia com `ffmpeg` (um `.wav` com os sons nas posições e nos
tempos certos) e OUÇA antes de escrever a task. Custa dez minutos; a task revertida custou uma
sessão. Duas armadilhas da maquete em si: `adelay=0|0` é recusado (o evento em zero não leva o
filtro), e **make-up gain demais + `alimiter` achatam tudo no mesmo nível** - foi assim que o "tique
vazio" e o "tique com folha" saíram idênticos numa medição, que é justamente a informação que a task
existe para carregar. Meça janelas do arquivo com `volumedetect` em vez de confiar no ouvido para o
balanço: no filtro, o farfalho ficou ~9 dB acima do tique, e esses números viraram as constantes em
dB no topo da task.

## A lição de acústica que custou uma tarefa

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


## Os oito lugares de registro

**Tarefa NOVA entra em OITO lugares, e a maioria falha em silêncio.** Na ordem em que se esquece:
o minigame (`game/tasks/<tipo>.nvgt`), o `#include` **e** o `else if` do `task_manager` (só o
include compila e a task nunca abre), o ponto no `map.nvgt` com beacon, `PRACTICE_TASKS` em
`ui/practice_screen.nvgt` (sem isso não dá para testá-la sem montar partida, e a
`probe_explore_tasks` acusa), o beacon em
`ui/onboarding_screens.nvgt` ("Conhecer o mapa"), as constantes **e a lista do build_pack** em
`audio/sound_catalog.nvgt` (sem a lista, o som não entra no `sounds.dat` e a task fica muda só no
jogo compilado), as chaves nos DOIS idiomas, e a seção de tarefas dos dois `docs/README_*`. Decida
também se ela é longa (`LONG_TASK_TYPES`) - o padrão é curta.

## A fila de ideias

Cada uma com: a original, a MECÂNICA que ela traria (o critério que importa), como traduzir para o
ouvido, e o risco. Os conjuntos de som foram conferidos na pasta `Task Panels` da biblioteca - o que
está escrito aqui existe lá.

**Esta lista é uma proposta, não um plano.** Nenhuma delas passou pela maquete, e é na maquete que
metade das ideias morre.

### 1. Rodar diagnóstico (`panel_LaunchpadDiagnose*`)

A mecânica nova é **detectar irregularidade num ritmo** - o jogo não tem nada parecido. No original
você acha o sistema que está piscando errado e roda o diagnóstico nele.

Para o ouvido: os sistemas tiquetaqueiam **um por vez, em rodízio**, cada um numa posição; um deles
tem o tique fora do compasso (atrasado, ou dobrado). O jogador aponta qual.

O rodízio é o detalhe que a faz funcionar: quatro tiques SIMULTÂNEOS seriam exatamente a parede que
a `stabilize_lines` ensinou a evitar. Em rodízio há um fundo de compasso e UMA coisa estranha dentro
dele, que é a única forma de mistura que este jogo suporta.

Risco: a irregularidade tem que ser grosseira o bastante para quem não tem ouvido musical. Medir na
maquete qual desvio é perceptível - começar por dobro/metade do intervalo, não por 10%.

### 2. Encher o tanque / a roda d'água (`panel_waterwheel*`, `panel_waterfill`)

A mecânica nova é **parar na hora certa de um movimento contínuo** - um alvo analógico, diferente do
distribuidor, que tem posições discretas. É "quanto", não "quando".

Para o ouvido: a água enche e o tom do jorro SOBE com o nível. O alvo é um tom tocado antes (como o
alvo do distribuidor). Solta a tecla quando o jorro chegar nele. Passar do ponto transborda.

Isto explora a coisa que o ouvido faz melhor que qualquer outro sentido: comparar duas alturas. O
distribuidor já provou que a referência ensinada no começo funciona.

Risco: é o caso em que um som CONTÍNUO é o portador da informação, o que contraria a regra geral.
Passa porque é **um** contínuo, com começo e fim claros, e porque a decisão é um instante - mas é
exatamente o tipo de coisa que a maquete tem que responder antes de existir código.

### 3. Separar amostras (`panel_sortsamplePickup01-04`, `panel_sortsampleDrop01-04`)

A mecânica nova é **classificar por timbre**: reconhecer o que uma coisa É pelo som dela, e não onde
ela está.

Para o ouvido: cada amostra tem um som próprio ao ser pegada; há três ou quatro recipientes, um em
cada posição, e cada um aceita um tipo. O jogador pega, ouve, e leva até o recipiente certo. O drop
confirma ou recusa.

Risco: vira decoreba se os timbres não forem obviamente diferentes entre si, e vira sorte se forem
parecidos. Os quatro arquivos de pickup existem justamente porque no original são quatro tipos - vale
medir se eles se distinguem sozinhos, sem rótulo falado.

### 4. Traçar a rota (`panel_nav_chartcourse`, `panel_nav_chartcourseFinal`)

A mecânica nova é **seguir uma sequência de direções** sem a lista na frente - memória de trajeto, não
de tons (o `unlock_manifolds` já faz memória de sequência, mas de números ditos).

Para o ouvido: a rota é ditada uma vez como uma série de direções (o som de cada etapa sai do lado
dela), e o jogador a repete nas setas. Errar uma etapa faz voltar à anterior, não ao começo.

Risco: é o mais perto de algo que já existe, e o ganho é menor. Fica depois das três de cima.

### 5. Ativar escudos (`panel_sheildON`, `panel_sheildOFF`)

A mecânica seria **agir sobre alguns itens de uma varredura e não sobre outros** - o ponteiro passa
por todos e você só aperta nos que estão desligados.

Risco alto de ser o distribuidor de novo com outra roupa, e foi por isso que ficou no fim da fila. Só
vale se a maquete mostrar que "ligado" e "desligado" se distinguem na passagem, sem o ponteiro parar.

### O que já foi recusado, para não voltar

- **Tarefas de mundo, mais painéis, e "espalhar as mesmas tarefas pela nave"**: três levas propostas
  e recusadas antes de a receita existir. "Não quero ficar espalhando as mesmas tasks pela nave não,
  fica estranho."
- **`stabilize_lines`**: escrita e revertida (commit 53341d0 e a reversão atrás dele). Quatro zumbidos
  simultâneos, um por posição. O motivo está acima, em SOM CONTÍNUO - e o código está no histórico se
  um dia servir de peça.
- **Inserir as chaves**: o conjunto de som existe e a tradução é óbvia (cada chave tem um tom, cada
  fenda responde com um tom, casa os dois) - mas é a mecânica do `fix_wiring` outra vez. O usuário
  marcou como "melhor colocarmos depois".
