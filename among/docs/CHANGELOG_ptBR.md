# Histórico de versões

Tudo que mudou no jogo, da versão mais nova para a mais antiga.

*Also available in English: [CHANGELOG_enUS.md](CHANGELOG_enUS.md).*

Este arquivo é a **fonte** das novidades que o jogo anuncia ao atualizar: o `version.json` publicado
no site é gerado a partir da primeira entrada daqui (ver `tools/make_version_json.py`). Escrever a
novidade em dois lugares seria garantir que um dia os dois discordem.

---

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
