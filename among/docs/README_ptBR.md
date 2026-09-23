# Among Us — Audiogame

Um jogo de dedução social para jogar inteiramente de ouvido. Você é um tripulante tentando terminar
as tarefas da nave, ou um impostor tentando eliminar todo mundo sem ser descoberto.

Tudo no jogo é falado pelo leitor de tela e posicionado no espaço: dá pra saber onde está cada
pessoa, cada objeto e cada corpo só pelo som.

*This manual is also available in English: README.md, in this same folder.*

> **Versão beta.** O jogo é jogável do começo ao fim, mas ainda está em testes. Se algo quebrar, o
> arquivo `crash.log` na pasta do jogo guarda o que aconteceu — ele ajuda muito a consertar.

## Como começar

1. Abra o jogo e escolha **Conectar** no menu inicial. Ele já sabe o endereço do servidor; não há
   nada para digitar.
2. Crie uma conta (usuário e senha) ou entre com uma que já tenha. O jogo lembra o último usuário
   que entrou nesta máquina e já deixa o cursor na senha.
3. Escolha uma partida na lista, ou crie a sua.
4. Na sala de espera, o anfitrião aperta Enter para começar. São necessários pelo menos 3 jogadores;
   o máximo é 15.

**Partida só para os amigos:** ao criar a partida, preencha o **código de acesso**. Com código, a
partida não aparece na lista, e só entra quem escolher **Entrar em partida privada com código** e
digitar o mesmo código. Maiúsculas e minúsculas não fazem diferença, então pode ditar de viva voz.
Dentro da sala, a tecla **C** relembra o código para você passar adiante.

**Praticar:** o menu inicial tem **Praticar tarefas e reparos**: qualquer tarefa ou reparo de
sabotagem, sozinho, sem partida — para aprender cada uma com calma antes de valer.

**Conhecer o mapa:** o menu inicial tem a opção **Conhecer o mapa**, que deixa você andar pela nave
sozinho, sem servidor e sem pressão. O nome de cada sala é dito ao entrar nela, Tab lista o que há na
sala, Enter diz o que é o objeto mais próximo, e ESC volta ao menu. É o jeito de aprender onde fica
cada coisa antes de haver um impostor por perto.

**Antes de jogar:** no menu inicial existe a opção **Aprender os sons do jogo**. Ela toca cada som
do jogo com o nome dele. Vale muito a pena passar por ela uma vez — o jogo inteiro depende de
reconhecer esses sons.

## Atualizações

Ao abrir, o jogo verifica se saiu uma versão nova. Havendo uma, ele diz o que mudou e pergunta se
você quer atualizar. Se aceitar, o jogo **se atualiza sozinho**: fecha, baixa a versão nova,
instala e abre de novo. Não há nada para baixar à mão nem pasta para descompactar.

Você pode recusar e continuar jogando na versão atual — a pergunta volta na próxima vez que abrir.

O que mudou em cada versão está no menu inicial, em **Novidades** — e também no arquivo
`NOVIDADES.md`, na pasta `docs` dentro da pasta do jogo (onde este manual também fica).

**Linux e Mac:** as versões para Linux e Mac estão no site, experimentais (a do Linux é um `.tar.gz`;
a do Mac é uma imagem de disco: abra e arraste o jogo para Aplicativos). Nesses sistemas a atualização ainda
é manual: o jogo avisa que saiu versão nova e abre a página de download.

Suas configurações e sua conta não se perdem na atualização: as preferências ficam na pasta de dados
do usuário, e a sua conta vive no servidor.

Se algo der errado no meio do caminho (a internet cair, por exemplo), o jogo é reaberto na versão
anterior, que continua intacta, e você recebe um aviso explicando o que houve. A versão antiga só é
substituída depois que a nova chega inteira e é conferida.

Sem internet, a verificação simplesmente não acontece e nada é dito.

## Mandar um recado para quem faz o jogo

Na lista de partidas existe a opção **Enviar um recado para quem faz o jogo**. Escreva o que achou,
o que quebrou ou o que faria diferente — o jogo confirma quando o recado é guardado.

Vai junto, automaticamente: a versão do jogo, o idioma, se você estava em partida, qual era o seu
papel, em que sala estava, e o `crash.log` (o arquivo que registra por que o jogo fechou sozinho, se
isso tiver acontecido). Você não precisa anotar nada disso — é justamente o que costuma faltar para
conseguir consertar um problema.

Como está em beta, esse é o canal mais útil: um relato com contexto vale mais do que dez "não
funcionou".

**As respostas chegam pelo jogo.** Quando quem faz o jogo responder ao seu recado, você ouve ao
chegar na lista de partidas: "você tem uma resposta a um recado seu". A opção **Respostas aos seus
recados** lê o seu recado e a resposta lado a lado. Não precisa de email nem de nada fora do jogo —
e se quiser continuar a conversa, é só mandar outro recado.

## Configurações

No menu inicial, em **Configurações**:

**Volumes** — um controle para cada família de som: volume geral, ambiente da nave, passos,
marcadores de objetos e radar, tarefas, mortes e alarmes, e menus. Ao mexer num controle você ouve
uma amostra daquela família no volume novo, então dá para ajustar tudo de ouvido sem entrar numa
partida. Se os passos alheios estão sumindo no meio do ambiente, é aqui que se resolve.

**Teclas** — todas as teclas de jogo podem ser trocadas. Escolha a ação, aperte Enter e depois
aperte a tecla nova. Se a tecla já estiver em uso, o jogo avisa de quem ela é em vez de deixar duas
ações brigando. Há também "Restaurar teclas padrão". As teclas listadas neste manual são as de
fábrica.

**Voz e leitor de tela** — escolha a voz do sistema, a velocidade e o volume dela, e se o jogo deve
usar o leitor de tela quando houver um. É o que resolve para quem joga sem NVDA: desmarque "usar
leitor de tela" e acelere a voz do sistema ao seu gosto.

**Idioma** — a lista mostra todos os idiomas instalados. A troca vale na hora, e o jogo abre no
idioma escolhido da próxima vez. O jogo vem com português do Brasil e inglês dos Estados Unidos, e
abre em inglês na primeira vez.

Tudo é salvo na hora e vale já na próxima partida.

## Traduzir o jogo para outro idioma

Qualquer pessoa pode acrescentar um idioma, sem programar e sem esperar por uma versão nova:

1. Na pasta `lang` do jogo, copie `en_US.json` para um arquivo novo com o código do seu idioma —
   por exemplo `fr_FR.json`.
2. Traduza os **valores** (o que está depois dos dois-pontos). As **chaves** (o que está antes) não
   mudam nunca — são elas que o jogo procura.
3. Na primeira linha, escreva o nome do idioma **no próprio idioma**, em `language.name`. É esse
   nome que aparece na lista: quem procura o idioma dele reconhece "Français", não "Francês". Em
   `language.translator`, ponha o seu nome — ele é dito ao lado do idioma na lista.
4. Abra o jogo. O idioma já aparece em **Configurações → Idioma**.
5. Para que todo mundo receba a sua tradução, na lista de partidas escolha **Enviar uma tradução
   para quem faz o jogo** e selecione o idioma. Ela é revisada e entra na versão seguinte; você
   recebe uma resposta dentro do jogo. Para atualizar depois, edite o arquivo e envie de novo.

As traduções ficam em github.com/otaviols/game-translations — quem preferir pode mandar por lá.

**Não precisa traduzir tudo de uma vez.** O que faltar aparece em inglês, então dá para traduzir aos
poucos, e uma tradução antiga continua funcionando quando o jogo ganha textos novos.

Se você vir uma chave crua na tela (algo como `menu.connect`), é sinal de que aquele texto não
existe nem no seu arquivo nem no inglês — vale mandar um recado avisando.

## Como o som funciona

- Esquerda e direita você percebe pelo estéreo, como sempre.
- Norte e sul se percebem pelo **tom**: tudo que está ao sul de você soa mais grave. Som agudo = ao
  norte, som grave = ao sul.
- Quanto mais perto, mais alto.
- Cada tipo de piso tem um som de passo diferente (a cafeteria é de madeira, o armazém é de esteira
  metálica, a estufa é de terra, e assim por diante). Dá pra saber em que sala você está só pelo som
  dos seus próprios passos — e o menu "Aprender os sons do jogo" tem todos eles separados.
- Cada tipo de objeto tem o seu marcador: um som contínuo no lugar dele, que fica mais alto conforme
  você se aproxima. É assim que se acha as coisas.
- Um corpo caído emite um som contínuo até alguém reportar.

## Teclas na sala de espera

| Tecla | Ação |
|---|---|
| Enter | iniciar a partida (só o anfitrião) |
| P | quem está na sala (com o total e quais são bots) |
| C | as regras desta partida (impostores, recargas, tempos, sabotagem) |
| O | mudar as regras (só o anfitrião): os campos vêm com os valores atuais, e o que ficar em branco continua como está. Todo mundo é avisado. |
| B | adicionar um bot (só o anfitrião, até 8) |
| Shift + B | remover o último bot |
| Y | escrever no chat |
| Vírgula / Ponto | navegar pelas mensagens |
| ESC | sair da partida |

## Teclas durante a partida

**Movimento e exploração**

| Tecla | Ação |
|---|---|
| W / A / S / D | andar para norte / oeste / sul / leste |
| Enter | interagir com o que estiver mais perto |
| Tab | radar: próximo alvo |
| Shift + Tab | radar: alvo anterior |
| Ctrl + Tab | trocar o modo do radar (jogadores / objetos da sala) |
| Q | travar o radar no último alvo apontado (ou soltar) |
| C | dizer em que sala você está |
| T | lista das suas tarefas, seu progresso e o do time |
| F1 | medir o ping com o servidor |
| ESC | sair da partida (pede confirmação) |

**Tripulante**

| Tecla | Ação |
|---|---|
| R | reportar um corpo (precisa estar perto dele) |
| G | dizer qual sabotagem está em andamento e onde consertar |
| H | usar a habilidade do seu papel (se o seu papel tiver uma) |
| Enter no botão | chamar reunião de emergência (uma por jogador) |

**Impostor**

| Tecla | Ação |
|---|---|
| K | matar quem estiver ao alcance |
| Enter (num duto) | entrar no duto |
| V (dentro do duto) | sair dele ou viajar para outro |
| G | menu de sabotagem |
| F | trancar as portas de uma sala |

**Reunião e chat**

| Tecla | Ação |
|---|---|
| B | abrir o menu de votação |
| T | quanto tempo falta nesta fase |
| Y | escrever no chat (só funciona durante reuniões) |
| Vírgula / Ponto | mensagem anterior / próxima |
| Shift + Vírgula | primeira mensagem |
| Shift + Ponto | última mensagem |
| Page Up / Page Down | alternar entre as mensagens e os eventos do jogo |

## O radar (Tab)

O radar tem dois modos, alternados com **Ctrl + Tab** (Ctrl e Shift acompanham a tecla do radar,
se você a trocar nas configurações):

- **Jogadores** — cicla por quem está na mesma sala que você. Toca um bipe na posição da pessoa e
  fala o nome dela.
- **Objetos da sala** — cicla pelo que existe na sala onde você está (tarefas, dutos, painéis,
  botão). Serve para conhecer o lugar e saber onde fica cada coisa.

**Travar num alvo (Q):** depois de apontar alguém com o Tab, aperte **Q** e o radar passa a bipar
sozinho naquela pessoa, sem precisar ficar apertando Tab. O tom do bipe sobe conforme ela se
aproxima e desce quando se afasta. A trava continua valendo só dentro da mesma sala: se a pessoa
sair da sala, entrar num duto ou sumir no escuro, o radar avisa e solta. Apontar outra pessoa com o
Tab move a trava para ela; **Q** de novo solta. No modo de objetos a trava funciona igual: bipa no
objeto marcado até você sair da sala.

O radar não funciona com as comunicações sabotadas, em nenhum dos dois modos.

## Tarefas

Cada tripulante recebe algumas tarefas (5 no preset Clássico). Quando **todas** as tarefas de
**todos** os tripulantes estiverem prontas, a tripulação vence. Aperte **T** a qualquer momento para
ouvir o seu progresso e o do time.

As onze tarefas:

- **Conectar fiação** — ligue os pares de fios com o mesmo tom.
- **Baixar dados** — segure Espaço até terminar.
- **Esvaziar lixo** — segure Espaço, depois solte e aperte de novo ao ouvir o bipe.
- **Alinhar motor** — use as setas para trazer o som até o centro (ambos os ouvidos iguais) e Enter
  para travar.
- **Passar cartão** — aperte Enter entre os dois bipes, no ritmo certo.
- **Destravar coletores** — ouça a sequência de tons e repita nas teclas 1 a 4.
- **Abastecer motores** — o painel toca primeiro o **tom do tanque cheio**. Depois segure Espaço: o
  som do enchimento vai subindo, e você solta quando ele chegar naquele mesmo tom. Soltar cedo é
  pouco combustível; passar do ponto transborda. O jogo não avisa a hora — quem julga é você.
- **Limpar asteroides** — cada asteroide vem de uma direção: esquerda, **frente** ou direita. O som
  centralizado nos dois ouvidos é o que vem de frente; atire com a seta correspondente (esquerda,
  **para cima** ou direita).
- **Rolar o dado** (sala de jogos) — o painel pede um número e você rola até tirar ele. O dado soa
  mais agudo quanto maior o resultado.
- **Regar as mudas** (estufa) — três canteiros: um à esquerda, um à frente e um à direita. O
  regador começa no do meio, e você o leva até o canteiro pedido com as setas esquerda e direita
  antes de apertar Espaço. Cada movimento faz a água balançar no lado para onde o regador foi — é
  assim que você sabe onde ele está. Apertar Espaço no canteiro errado não rega nada.
- **Revisar as gravações** (segurança) — ouça o trecho gravado do corredor e conte quantas pessoas
  passaram. Cada uma atravessa por um lado, com o piso e o ritmo dela. Responda nas teclas 1 a 6.
- **Fazer o exame médico** (enfermaria) — fique parado no scanner até ele terminar. É a tarefa
  mais fácil do jogo e a mais valiosa: **quem estiver por perto ouve o scanner rodando e o jogo diz
  quem está sendo examinado** — e impostor não faz tarefa, então ver alguém no exame prova que é
  tripulante. Um impostor fingindo não produz som nenhum para os outros.

**ESC cancela qualquer tarefa**; ela continua pendente e você pode voltar depois. Se uma reunião
começar no meio de uma tarefa, ela fecha sozinha e nada se perde.

**Tarefas comuns:** passar cartão é uma tarefa comum. Ou **todos** os tripulantes a recebem naquela
partida, ou **nenhum** recebe — nunca só alguns, e ela existe num lugar só da nave. Guarde isso: se
alguém disser que estava passando o cartão numa partida em que o cartão não saiu na sua lista, essa
pessoa está mentindo.

**Morreu? Continue fazendo as suas tarefas.** Fantasmas continuam contando para a vitória da
tripulação, atravessam portas trancadas, e são a melhor arma do time depois que a coisa começa a dar
errado. Fantasma atravessa porta fechada, mas **não** atravessa parede — e ouve o esbarrão nela
normalmente, para conseguir se localizar.

**Sabotagem não atinge fantasma.** Com as luzes ou as comunicações fora, você continua com o radar,
com os marcadores das suas tarefas e com o som dos corpos. Sabotagem existe para pressionar quem
ainda pode perder alguma coisa — e você já perdeu; continuar terminando as tarefas é o que ainda
ajuda o time.

## As câmeras da segurança

Na **segurança** existe um posto de câmeras. Aperte **Enter** nele para ligar: você passa a ouvir
uma sala à distância, como se estivesse lá dentro — os passos, o ambiente, o que acontece.

| Tecla | O que faz |
|---|---|
| Setas esquerda / direita | trocar a sala observada |
| Tab (a tecla do radar) | dizer quem está naquela sala |
| ESC ou Enter | desligar |

**Enquanto observa, você fica parado e surdo ao que está à sua volta.** Alguém pode chegar perto sem
que você perceba — é o preço de estar olhando para longe.

**Só uma pessoa por vez** usa o posto. E quem estiver na sala observada ouve **um rádio ligado ao
longe**: é discreto, mas quem presta atenção percebe que está sendo vigiado. Vale nos dois sentidos —
se você é impostor e ouve o rádio, sabe que alguém está de olho; se está na câmera, saiba que o outro
lado também pode saber.

**As comunicações sabotadas derrubam as câmeras.**

## Reuniões e votação

Uma reunião começa quando alguém reporta um corpo ou aperta o botão de emergência. Todo mundo é
levado para a cafeteria e o movimento trava.

Aperte **Y** para conversar e **B** para abrir o menu de votação quando estiver pronto. No menu,
**Enter marca** um nome (ou "pular") e **Enter de novo no mesmo nome confirma** — assim um dedo
apressado não vota em quem estava em cima da lista. Fechar o menu com ESC **não** gasta o seu voto:
você pode abrir de novo. A votação acaba assim que todo mundo votar, ou quando o tempo terminar
(75 segundos no preset Clássico); nos últimos 10 segundos um tique marca o tempo.

**Como se decide:** alguém só é expulso se tiver **mais votos do que "pular"** e do que qualquer
outro. Se a maioria pula, ninguém sai — um voto sozinho não expulsa ninguém. Empate é ninguém.

Duas regras da sala mudam a reunião (na criação e na tecla O): se o jogo **diz em quem cada um
votou**, na hora do voto (clássico) ou só que votou; e se o jogo **diz se o expulso era impostor**.

**Durante a reunião, T diz quanto tempo falta** — e diz de qual fase: se ainda é discussão ou se a
votação já está correndo. Fora da reunião, T continua sendo a sua lista de tarefas.

O chat só existe na sala de espera e durante as reuniões. Não há chat durante a partida — nem entre
impostores.

## Papéis especiais

Além de tripulante e impostor, a sala pode ligar **papéis especiais** — profissões com uma
habilidade própria. Eles vêm **desligados**: uma sala que não mexer nisso joga exatamente o jogo de
sempre.

Quem cria a sala escolhe, para cada papel, **quantos** existem e a **chance** de cada um aparecer
(vazio = sempre). A chance importa: com 50%, ninguém pode assumir que o papel está em jogo, e é
essa dúvida que o torna interessante. As regras da sala (tecla **O**) dizem o que está ligado e com
que chance — mas nunca dizem quem saiu.

Quem tem um papel com habilidade usa a tecla **H**.

**Engenheiro** (tripulação). Faz tarefas como qualquer tripulante e conta para a vitória do time.
Duas coisas o diferenciam:

- **Usa os dutos**, como o impostor. O som da tampa é o mesmo — quem ouvir não sabe qual dos dois
  é. Isso ajuda você a se mover, e atrapalha a sua defesa numa reunião.
- **Conserta a sabotagem em andamento de onde estiver**, com a tecla H, sem precisar chegar ao
  painel. A recarga é longa (dois minutos), então é uma salvada por partida, não um desfazer. E
  todo mundo percebe: a sabotagem acaba sem ninguém ter chegado ao painel.

**Xerife** (tripulação). Ele pode matar, com a **mesma tecla e o mesmo alcance do impostor** — e o
tiro soa igual para quem está por perto. Mas se ele atirar em quem **não** é impostor, **quem morre
é ele**. A pessoa que ele mirou não fica sabendo de nada: ouve um assassinato do lado, como
qualquer vizinho, e não tem como saber que era ela na mira.

Isso faz do xerife uma aposta, não um detector. Errar custa duas pessoas à tripulação de uma vez —
o inocente que continua sob suspeita e o xerife que não está mais lá para defendê-lo.

**Alarmista** (tripulação). Um tripulante comum em tudo — até morrer. Quando ele morre, **a nave
inteira ouve que ele morreu e em que sala**, de onde quer que cada um esteja: no meio de uma
tarefa, do outro lado do mapa, na câmera.

O aviso **não** diz quem matou. E ninguém sabe quem é o alarmista — nem ele tem como avisar que é.
Para o impostor, isso muda a conta de matar num canto escondido: qualquer vítima pode ser ela.

**Anjo da Guarda** (tripulação). Enquanto vivo, é um tripulante comum. **Depois de morrer**, ele
ganha uma jogada: chegar perto de alguém vivo e apertar a tecla da habilidade. Por 30 segundos,
nenhum ataque contra essa pessoa acontece — o impostor aperta para matar e simplesmente nada
acontece.

Três coisas importam aqui:

- **Ele precisa ir até a pessoa**, como o impostor precisaria. Proteger custa a travessia.
- **Ninguém fica sabendo.** Nem quem foi protegido, nem quem tentou matar. Só o anjo ouve o nome de
  quem ele protegeu, na hora de lançar.
- **Ele nunca descobre se serviu de algo.** Saber que o escudo salvou alguém seria saber que havia
  um impostor ali — e um fantasma não pode ter esse tipo de informação.

O escudo vale contra qualquer ataque, inclusive o do xerife: mirar num inocente protegido não mata
ninguém, nem o próprio xerife.

**Detetive** (tripulação). Perto de um corpo, a tecla da habilidade **examina** sem reportar: ele
ouve **há quanto tempo a pessoa morreu** e **por quais salas o assassino passou** nos segundos
seguintes à morte.

- **Examinar não convoca reunião.** Ele decide se chama na hora ou se guarda o que descobriu.
- **Cada corpo só pode ser examinado uma vez** — por ele ou por outro detetive.
- **O exame não aponta ninguém.** É uma lista de salas: quem passou por ali por acaso entra na
  conta junto com o culpado. A pista vale uma discussão, não uma acusação pronta.
- **Duto apaga o rastro.** Se o assassino sumiu por um duto, o exame diz só isso — e essa é a
  contra-jogada do impostor, ao custo do barulho da tampa para quem estiver perto.

**Metamorfo** (impostor). Um impostor que faz tudo o que um impostor faz, e mais uma coisa: com a
tecla da habilidade ele escolhe um jogador vivo, de qualquer lugar do mapa, e **passa a aparecer
com o nome dessa pessoa** — no radar, na câmera e na lista de quem está numa sala.

Quem ele mata é a exceção: **a vítima descobre quem ele é de verdade.** Ela já perdeu, e não tem
como contar a ninguém — não vota, não fala com os vivos, e fantasma só é ouvido por fantasma.

Dura 45 segundos e tem uma recarga longa. Duas coisas o impedem de ser gratuito:

- **Transformar faz barulho**, posicionado, como a tampa de um duto. Quem estiver por perto ouve
  que algo aconteceu ali — sem saber quem, nem em quem.
- **Enquanto disfarçado, ele não fala.** O microfone fecha. Num jogo em que a conversa é metade da
  defesa, ficar mudo é um preço alto.

A **reunião desfaz tudo**: na mesa todo mundo é quem é, porque ali se vota por nome.

**Fantasma** (impostor). Com a tecla da habilidade ele **some por 20 segundos**: não aparece no
radar, não aparece na câmera, **não faz som de passos** e não é ouvido no chat de voz. É o único
papel que tira som em vez de mudar o que ele significa — e, num jogo em que o passo alheio é a
pista principal, alguém atravessando a nave sem pisar é a coisa mais perigosa que existe.

O preço é alto:

- **Sumir e voltar fazem barulho**, no lugar onde ele está. Quem estiver perto ouve — e sabe que
  por alguns segundos tem alguém ali que não vai ouvir mais.
- **Matar devolve ele na hora.** O instante do crime acaba com a invisibilidade, com som e tudo.
- Enquanto sumido, **ele também não fala.**

A reunião também o devolve.

## Sabotagem (impostor)

Três sabotagens, no menu da tecla **G**. Uma de cada vez, com recarga de 30 segundos, e não dá para
repetir a mesma duas vezes seguidas.

- **Luzes** — os corpos ficam mudos, o radar só alcança quem estiver bem colado, o alcance de
  audição geral cai e o botão de emergência para de funcionar. Dura até alguém religar os
  **disjuntores** no painel da Elétrica: são cinco, nas teclas 1 a 5; o painel diz quais estão
  desligados, e cada tecla inverte o seu — religar tudo às cegas desliga os que estavam bons.
  Espaço repete o estado.
- **Oxigênio** — sabotagem crítica. A tripulação tem 90 segundos para consertar **dois** painéis, um
  na Administração e outro na Elétrica, ou perde a partida. Cada painel diz um **código de cinco
  dígitos** ao abrir — o mesmo nos dois — e é preciso digitá-lo; errar apaga e recomeça, e Espaço
  repete o código. Quem ouviu num painel pode ditar para quem está no outro. Enquanto ela estiver ativa, ninguém
  consegue chamar reunião nem reportar corpo. O jogo avisa quanto falta de 30 em 30 segundos, e nos
  **10 segundos finais** conta um a um — se você ouvir a contagem começar, já não dá tempo de trocar
  de painel: termine o que começou.
- **Comunicações** — derruba o radar (nos dois modos), a lista de tarefas, os marcadores das suas
  tarefas e o aviso de corpo por perto. Dura até alguém **sintonizar o rádio** no painel da Navegação: gire o dial com as
  setas até a voz ficar sem estática e segure ali um instante. É o único conserto que se faz de
  ouvido — ninguém consegue ditar a resposta de longe.

**Portas (tecla F)** — tranca todos os corredores de uma sala por 12 segundos, com recarga de 25
segundos. Não tem conserto: as portas reabrem sozinhas. Não é sabotagem, então dá para trancar uma
sala **e** sabotar ao mesmo tempo. Fantasmas atravessam portas trancadas.

**Dutos** — só o impostor usa. Para **entrar**, chegue perto de um duto e aperte **Enter**, como em
qualquer outro objeto. Já **dentro** dele, o **V** abre as opções: sair ali mesmo ou viajar até outro
duto da rede. Entrar num duto some com você do mapa. A rede liga Navegação, Sala de armas e Reator — os três cantos
opostos da nave, sem nenhum corredor direto entre eles. Some de um canto e apareça no outro em
segundos: é um trajeto que ninguém consegue fazer a pé, e é justamente por isso que serve de álibi.

## Como se vence

**Tripulação vence se:**
- todas as tarefas de todo mundo forem concluídas; ou
- todos os impostores forem expulsos.

**Impostores vencem se:**
- ficarem em número igual ou maior que o dos tripulantes vivos; ou
- o oxigênio acabar sem ser consertado.

## Bots

Na sala de espera, o anfitrião pode adicionar bots com **B** (até 8) e remover com **Shift + B**.
Eles andam pela nave, fazem tarefas, votam nas reuniões e podem ser sorteados como impostores —
quando são, caçam, matam quando ninguém está por perto e sabotam. Servem para completar uma partida
ou para treinar sozinho.

## Presets de partida

| Preset | Jogadores | Impostores | Tarefas | Recarga do kill | Sabotagem |
|---|---|---|---|---|---|
| Clássico | até 10 | automático | 5 cada | 25 s | ligada |
| Rápido | até 6 | 1 | 3 cada | 15 s | desligada |
| Caos | até 10 | 3 | 4 cada | 12 s | ligada |

Os presets abaixo são o ponto de partida; o teto de uma partida é **15 jogadores**.

**Dá para ajustar o preset na hora de criar a sala.** Além do preset, a tela de criação tem campos
para máximo de jogadores, número de impostores, tarefas por tripulante, recarga do kill, tempo de
discussão, tempo de votação, reuniões de emergência por jogador e se a sabotagem entra.

**Todos os campos aceitam ficar em branco**, e em branco quer dizer "use o que o preset manda" —
então quem só quer jogar escolhe o preset, confirma e pronto. Quem entrar na sala consegue ver o que
você mudou apertando **C**, e quem estiver escolhendo na lista já vê o resumo.
