# Among Us — Audiogame

Um jogo de dedução social para jogar inteiramente de ouvido. Você é um tripulante tentando terminar
as tarefas da nave, ou um impostor tentando eliminar todo mundo sem ser descoberto.

Tudo no jogo é falado pelo leitor de tela e posicionado no espaço: dá pra saber onde está cada
pessoa, cada objeto e cada corpo só pelo som.

> **Versão beta.** O jogo é jogável do começo ao fim, mas ainda está em testes. Se algo quebrar, o
> arquivo `crash.log` na pasta do jogo guarda o que aconteceu — ele ajuda muito a consertar.

## Como começar

1. Abra o jogo e escolha **Conectar** no menu inicial. Ele já sabe o endereço do servidor; não há
   nada para digitar.
2. Crie uma conta (usuário e senha) ou entre com uma que já tenha. O jogo lembra o último usuário
   que entrou nesta máquina e já deixa o cursor na senha.
3. Escolha uma partida na lista, ou crie a sua.
4. Na sala de espera, o anfitrião aperta Enter para começar. São necessários pelo menos 3 jogadores;
   o máximo é 10.

**Antes de jogar:** no menu inicial existe a opção **Aprender os sons do jogo**. Ela toca cada som
do jogo com o nome dele. Vale muito a pena passar por ela uma vez — o jogo inteiro depende de
reconhecer esses sons.

## Atualizações

Ao abrir, o jogo verifica se saiu uma versão nova. Havendo uma, ele diz o que mudou e pergunta se
você quer atualizar. Se aceitar, o jogo **se atualiza sozinho**: fecha, baixa a versão nova,
instala e abre de novo. Não há nada para baixar à mão nem pasta para descompactar.

Você pode recusar e continuar jogando na versão atual — a pergunta volta na próxima vez que abrir.

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
   nome que aparece na lista: quem procura o idioma dele reconhece "Français", não "Francês".
4. Abra o jogo. O idioma já aparece em **Configurações → Idioma**.

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
| C | dizer em que sala você está |
| T | lista das suas tarefas, seu progresso e o do time |
| F1 | medir o ping com o servidor |
| ESC | sair da partida (pede confirmação) |

**Tripulante**

| Tecla | Ação |
|---|---|
| R | reportar um corpo (precisa estar perto dele) |
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

O radar tem dois modos, alternados com **Ctrl + Tab**:

- **Jogadores** — cicla por quem está na mesma sala que você. Toca um bipe na posição da pessoa e
  fala o nome dela.
- **Objetos da sala** — cicla pelo que existe na sala onde você está (tarefas, dutos, painéis,
  botão). Serve para conhecer o lugar e saber onde fica cada coisa.

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
  passaram. Cada uma atravessa por um lado, com o piso e o ritmo dela. Responda nas teclas 1 a 4.

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

## Reuniões e votação

Uma reunião começa quando alguém reporta um corpo ou aperta o botão de emergência. Todo mundo é
levado para a cafeteria e o movimento trava.

Aperte **Y** para conversar e **B** para abrir o menu de votação quando estiver pronto. Fechar o
menu de votação com ESC **não** gasta o seu voto: você pode abrir de novo. A votação acaba assim que
todo mundo votar, ou quando o tempo terminar (75 segundos no preset Clássico).

**Durante a reunião, T diz quanto tempo falta** — e diz de qual fase: se ainda é discussão ou se a
votação já está correndo. Fora da reunião, T continua sendo a sua lista de tarefas.

O chat só existe na sala de espera e durante as reuniões. Não há chat durante a partida — nem entre
impostores.

## Sabotagem (impostor)

Três sabotagens, no menu da tecla **G**. Uma de cada vez, com recarga de 30 segundos, e não dá para
repetir a mesma duas vezes seguidas.

- **Luzes** — os corpos ficam mudos, o radar só alcança quem estiver bem colado, o alcance de
  audição geral cai e o botão de emergência para de funcionar. Dura até alguém consertar o painel na
  Elétrica.
- **Oxigênio** — sabotagem crítica. A tripulação tem 90 segundos para consertar **dois** painéis, um
  na Administração e outro na Elétrica, ou perde a partida. Enquanto ela estiver ativa, ninguém
  consegue chamar reunião nem reportar corpo. O jogo avisa quanto falta de 30 em 30 segundos, e nos
  **10 segundos finais** conta um a um — se você ouvir a contagem começar, já não dá tempo de trocar
  de painel: termine o que começou.
- **Comunicações** — derruba o radar (nos dois modos), os marcadores das suas tarefas e o aviso de
  corpo por perto. Dura até alguém consertar o painel na Navegação.

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

Ao criar a partida dá para sobrescrever o tempo de recarga do kill.
