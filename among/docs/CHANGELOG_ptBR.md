# Histórico de versões

Tudo que mudou no jogo, da versão mais nova para a mais antiga.

*Also available in English: CHANGELOG.md, in this same folder.*

---

## 0.25.2

Toda tarefa (e todo reparo) termina com o mesmo som de concluído. Antes cada uma tocava um som
diferente no fim, por cima do certo.

## 0.25.1

- **Quando o servidor for atualizar, você é avisado antes**: "o servidor vai reiniciar em tantos
  minutos". Partidas novas não começam até lá, e as que estão rolando têm esse tempo para acabar.
  Antes o servidor trocava sem aviso e derrubava todo mundo no meio do jogo.
- Se a conexão com o servidor cair, o jogo agora diz isso e volta ao menu inicial — antes ficava
  parado numa nave vazia, sem explicação.
- Chat: as mensagens não interrompem mais a fala, e o cursor do histórico fica onde você o deixou
  quando chega mensagem nova. Com a caixa de mensagem aberta você continua ouvindo o chat, e ela
  fecha sozinha quando a reunião acaba.

## 0.25.0

- **Votação justa.** Alguém só é expulso se tiver mais votos do que "pular" e do que qualquer outro.
  Antes, nove pessoas pulando e uma votando expulsavam alguém.
- **Votar em dois passos**: Enter marca um nome, Enter de novo confirma. E um tique marca os últimos
  dez segundos da votação.
- **Regra nova de sala: votos abertos ou secretos.** Abertos, o jogo diz em quem cada um votou na
  hora do voto; secretos, só que votou.
- **Consertar sabotagem agora dá trabalho.** Luzes: religar os cinco disjuntores da Elétrica, que o
  painel diz quais estão desligados. Oxigênio: cada painel dita um código de cinco dígitos, o mesmo
  nos dois. Comunicações: sintonizar o rádio com as setas até a voz ficar sem estática.
- Sons novos: o de alguém ser expulso, a música de vitória de cada time, os de marcar e confirmar o
  voto, os dos reparos, e os sons certos para abrir e fechar tarefas e menus.
- O radar travado (Q) agora funciona também no modo de objetos. E o modo do radar volta a ser só
  Ctrl + a tecla do radar — a tecla M saiu.
- Com as comunicações sabotadas, a lista de tarefas também não responde.
- **Praticar tarefas e reparos**, no menu inicial: qualquer tarefa ou reparo, sozinho, sem partida.
  Era o "modo de treino" que vocês pediram.

## 0.24.1

O marcador do scanner da enfermaria ganhou o som definitivo.

## 0.24.0

- **Nova tarefa: o exame médico**, na enfermaria. Fique parado no scanner até ele terminar. Quem
  estiver por perto ouve o scanner rodando e o jogo diz quem está sendo examinado — como impostor
  não faz tarefa, é a prova de que aquela pessoa é tripulante. Fingir não produz som para os outros.
- **Nova regra de sala: dizer ou não se o expulso era impostor.** Desligada, cada expulsão vira
  dúvida em vez de resposta. Fica na criação da sala e na tecla O.
- Sons novos na tarefa dos asteroides: o tiro e o estouro do acerto, vindo da direção certa.

## 0.23.0

- **Espanhol!** O jogo ganhou a primeira tradução feita por um jogador. Está em Configurações, na
  opção Idioma.
- **Enviar uma tradução pelo jogo.** Traduziu o jogo para o seu idioma? Na lista de partidas, escolha
  "Enviar uma tradução para quem faz o jogo": ela é revisada e entra na versão seguinte, e você
  recebe uma resposta dentro do jogo. O manual explica como fazer a tradução. Na lista de idiomas, o
  nome de quem traduziu é dito ao lado do idioma.

## 0.22.5

Corrigida a atualização automática, que nas duas versões anteriores fechava o jogo e não voltava.
Quem está na 0.22.3 ou 0.22.4 precisa baixar esta versão pelo site uma vez; daí em diante a
atualização volta a ser sozinha.

## 0.22.4

Limpeza nos textos que vão com o jogo: o histórico de versões perdeu uma nota interna que não dizia
nada a quem joga, e os manuais passaram a se referir um ao outro pelos nomes que existem na pasta.

## 0.22.3

Versão para Mac no site, experimental, ao lado da de Linux. É uma imagem de disco: abra e arraste o
jogo para Aplicativos. Quem usa Mac ou Linux: conta o que aconteceu pelo recado dentro do jogo.

## 0.22.2

Todas as teclas de ação da partida podem ser trocadas nas configurações — faltavam a de travar o
radar (Q) e a de trocar o modo do radar, que agora tem a tecla M (Ctrl + Tab continua valendo).

## 0.22.1

Os manuais e o histórico de versões passaram para uma pasta `docs` dentro da pasta do jogo, em vez
de ficarem soltos ao lado do executável.

## 0.22.0

- **Novidades dentro do jogo.** O menu inicial ganhou "Novidades": escolha uma versão e ouça o que
  mudou nela. O histórico também vai como arquivo na pasta do jogo (NOVIDADES.md).
- **Volume da música** separado nas configurações. Hoje é só a música do menu, mas quem quer ouvir
  os passos abaixa a música e mais nada.
- **Versão para Linux** no site, experimental. Nesses sistemas a atualização ainda é manual: o jogo
  avisa e abre a página de download.

## 0.21.0

**As respostas aos seus recados chegam pelo jogo.** Quando quem faz o jogo responder, você ouve ao
chegar na lista de partidas, e a opção "Respostas aos seus recados" lê o seu recado e a resposta
lado a lado. Sem email, sem sair do jogo.

## 0.20.0

Três coisas que vocês pediram:

- **Conhecer o mapa**, no menu inicial: ande pela nave sozinho, sem servidor e sem pressão. O nome
  de cada sala é dito ao entrar, Tab lista o que há nela, Enter diz o que é o objeto mais próximo.
- **O anfitrião muda as regras com a sala já aberta**, com a tecla O. Os campos vêm com os valores
  atuais, e todo mundo na sala é avisado da mudança.
- **Radar travado**: depois de apontar alguém com o Tab, Q faz o radar bipar sozinho naquela
  pessoa, com o tom subindo conforme ela se aproxima. Solta sozinho quando ela sai da sala.

## 0.19.0

**Novo: partidas privadas.** Ao criar uma partida, preencha o código de acesso: ela some da lista e
só entra quem escolher "Entrar em partida privada com código" e digitar o mesmo código. Dentro da
sala, a tecla C relembra o código para você passar aos amigos. Foi o pedido mais repetido de vocês.

Corrigido: sair de uma partida no meio e entrar em outra fazia você ser expulso da nova quando a
antiga terminava, como se tivesse vencido.

Quando não dá para entrar numa partida, o jogo agora diz por quê: código errado, sala cheia ou
partida já começada.

## 0.18.5

Passo novo no piso de madeira, e equilibrado com os demais: o de agora vinha mais forte que os outros
pisos, e não mais fraco.

## 0.18.4

O passo no piso de madeira ficou um pouco mais alto — ainda soava fraco ao lado dos outros pisos.

## 0.18.3

O passo no piso de azulejo voltou a ser o de sempre. O som novo que entrou na versão passada era
abafado demais, e nenhum ajuste de volume resolvia.

## 0.18.2

Sons novos: passos, ambientes de sala e o alarme do botão de emergência foram trocados por versões
melhores. Os passos ganharam mais variações, então andar pela nave soa menos repetitivo.

Alguns pisos soavam bem mais baixo que outros — o de azulejo chegava a quase sumir. Todos foram
equilibrados: agora se ouve alguém chegando com a mesma clareza em qualquer chão.

O download caiu pela metade, de 60 MB para 29 MB, sem perder qualidade audível.

## 0.18.1

- **Sabotar as comunicações agora fecha a câmera de quem estava olhando.** Antes o posto caía do lado
  do servidor mas o jogador continuava preso nele: parado, ouvindo outra sala, sem conseguir andar.
- **A gravação da segurança voltou a tocar os passos.** Ela rodava muda desde a 0.16.0, e sem os
  passos não havia nada para contar.
- Agora podem passar de uma a **seis** pessoas na gravação, e a resposta vai nas teclas 1 a 6.

## 0.18.0

**Quem está dentro de um duto não aparece mais nas câmeras.** A lista de quem está na sala observada
contava também o impostor escondido, e isso entregava de graça exatamente a dedução que o duto existe
para negar: se a câmera diz que tem gente numa sala vazia, todo mundo sabe o que aquilo significa.
Agora a sala observada mostra só quem está de fato andando por ela.

## 0.17.5

- Na sala de espera, **P** (quem está aqui) e **C** (as regras da partida) já respondem para quem
  acabou de criar a sala. Antes ficavam mudos até a segunda pessoa entrar.
- Ao abrir a caixa de mensagem com **Y**, o cursor já começa dentro dela: dava para digitar e apertar
  Enter sem que nada acontecesse, porque era preciso apertar Tab antes. O mesmo valia para o campo de
  usuário na primeira entrada, o nome da partida ao criar uma sala e os volumes nas configurações.

## 0.17.4

Na task de rever a gravação, na segurança, a fita rebobinando ainda estava tocando quando os
primeiros passos começavam — e um passo por cima do chiado da fita é justamente o passo que não dá
para contar. Agora a gravação só começa depois que a fita para.

## 0.17.3

O som de morte agora toca **só para quem morreu**. Quem mata e quem estiver por perto ouve o som do
assassinato, que dá a direção do crime sem dizer quem foi. Antes os dois sons tocavam para todo mundo
ao mesmo tempo: entregava o assassinato de graça a quem estava por perto e, para a vítima, soava como
se outra pessoa tivesse morrido.

## 0.17.2

Duas correções no posto de câmeras:

- A tecla do radar agora diz quem está na **sala observada**. Antes ela respondia pela sala onde o
  seu personagem estava parado, o que tornava a câmera inútil para procurar alguém.
- As salas passaram a ficar em ordem alfabética, e o jogo anuncia a posição ("Cafeteria, 3 de 11").
  A seta para a direita avança, a seta para a esquerda volta para a sala anterior, e nas pontas a
  lista dá a volta — dá para aprender de cor quantas vezes apertar para chegar em cada sala.

## 0.17.1

As novidades que o jogo anuncia ao atualizar agora aparecem no **seu idioma**. Antes elas vinham
sempre em português, mesmo para quem joga em inglês — que é o idioma padrão do jogo.

## 0.17.0

**Novo: o posto de câmeras da segurança.** Um jogador por vez observa uma sala à distância, ouvindo
o que acontece lá como se estivesse no lugar. As setas trocam de sala, a tecla do radar diz quem
está nela, e ESC desliga.

Enquanto observa, você fica parado e sem ouvir o que está à sua volta — alguém pode chegar perto sem
que você perceba. E quem estiver na sala observada ouve um rádio ligado ao longe: discreto, mas quem
presta atenção nota que está sendo vigiado.

As comunicações sabotadas derrubam as câmeras.

## 0.16.0

Versão de manutenção. Nada muda para quem joga.

## 0.15.0

- **Sabotagem não atinge mais fantasmas.** Depois de morrer você mantém o radar, os marcadores das
  suas tarefas e o som dos corpos. Sabotagem existe para pressionar quem ainda pode perder algo.
- Quando o jogo fecha por um erro, ele agora consegue registrar o que houve em mais situações — e
  avisa você se não conseguir, em vez de deixar você procurando um arquivo que não existe.
- A atualização não deixa mais arquivos sobrando na pasta do jogo.

## 0.14.2

Som novo para o começo da rega, na tarefa da estufa.

## 0.14.1

- **Partidas de até 15 jogadores** (eram 10), com cinco cores novas.
- A atualização automática passou a funcionar em computadores onde antes ela falhava.

## 0.13.0

- Na sala de espera, **P** diz quem está presente e **C** diz as regras da partida.
- A lista de salas mostra impostores, recarga do kill e sabotagem em cada item.
- A criação de sala ganhou ajustes finos sobre o preset: jogadores, impostores, tarefas, tempos de
  reunião e sabotagem.

## 0.12.0

- **Qualquer pessoa pode traduzir o jogo** para outro idioma, e ele aparece na lista de idiomas sem
  precisar de uma versão nova. O que não estiver traduzido aparece em inglês. O passo a passo está no
  manual.
- Os avisos que vêm do servidor agora aparecem no seu idioma.
- Corrigidas as mensagens de cadastro, que apareciam sem tradução.

## 0.11.1

- O jogo abre em **inglês** por padrão; quem prefere português troca uma vez em Configurações e a
  escolha fica salva.
- Corrigido o manual sobre os dutos: entra-se com **Enter** no duto, e o **V** só serve para sair ou
  viajar quando já se está dentro.

## 0.11.0

**Novo: mandar um recado para quem faz o jogo**, direto da lista de partidas. A versão, o que você
estava fazendo e o registro de erros vão junto automaticamente.

## 0.10.1

Corrigida a tecla **T** durante a reunião: ela não respondia.

## 0.10.0

- Dois impostores em vez de três em partidas de até 14 jogadores.
- Fantasma agora ouve quando esbarra na parede.
- Na reunião, **T** diz quanto tempo falta.
- A sabotagem de oxigênio avisa o tempo restante de 30 em 30 segundos e conta os 10 finais.
- A tarefa dos asteroides ganhou asteroide vindo de frente, com a seta para cima.

## 0.9.3

O título da janela mostra a versão do jogo.

## 0.9.2

- A **atualização automática funciona de verdade**: antes ela só abria o site de download.
- Corrigido o menu de aprender os sons, onde os passos e o som de morte não tocavam nada.

## 0.9.1

Corrigido o menu de aprender os sons: os passos e o som de morte não tocavam nada no jogo instalado.

## 0.9.0

Primeira versão distribuída em beta.

- Atualização automática.
- Escolha de idioma nas configurações.
- Tarefa de combustível de volta ao formato antigo (soltar quando o tom do enchimento casar com o
  tom de referência).
- Tarefa de regar refeita: é preciso levar o regador até o canteiro certo.
