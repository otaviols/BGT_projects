# Histórico de versões

Tudo que mudou no jogo, da versão mais nova para a mais antiga.

*Also available in English: [CHANGELOG_enUS.md](CHANGELOG_enUS.md).*

Este arquivo é a **fonte** das novidades que o jogo anuncia ao atualizar: o `version.json` publicado
no site é gerado a partir da primeira entrada daqui (ver `tools/make_version_json.py`). Escrever a
novidade em dois lugares seria garantir que um dia os dois discordem.

---

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

Versão de arrumação: nada muda para quem joga. Por dentro, os papéis passaram a declarar o que podem
fazer (preparando o terreno para papéis novos), os sons foram organizados em pastas por categoria e
os manuais ganharam um lugar próprio.

## 0.15.0

- **Sabotagem não atinge mais fantasmas.** Depois de morrer você mantém o radar, os marcadores das
  suas tarefas e o som dos corpos. Sabotagem existe para pressionar quem ainda pode perder algo.
- O registro de erros passou a cobrir também os menus, e a ser gravado mesmo quando a pasta do jogo
  é protegida.
- A pasta temporária deixada por uma atualização é limpa sozinha.

## 0.14.2

Som novo para o começo da rega, na tarefa da estufa.

## 0.14.1

- **Partidas de até 15 jogadores** (eram 10), com cinco cores novas.
- A atualização automática deixou de falhar em máquinas onde a pasta temporária é bloqueada: o jogo
  procura um lugar onde realmente consiga escrever.

## 0.13.0

- Na sala de espera, **P** diz quem está presente e **C** diz as regras da partida.
- A lista de salas mostra impostores, recarga do kill e sabotagem em cada item.
- A criação de sala ganhou ajustes finos sobre o preset: jogadores, impostores, tarefas, tempos de
  reunião e sabotagem.

## 0.12.0

- **Idiomas plugáveis**: basta pôr um arquivo na pasta `lang` para o idioma aparecer no jogo, e o
  que não estiver traduzido cai no inglês.
- As mensagens que vêm do servidor passaram a ser traduzidas no idioma de cada jogador.
- Corrigidos os avisos de cadastro, que apareciam sem tradução.

## 0.11.1

- O jogo abre em **inglês** por padrão; quem prefere português troca uma vez em Configurações e a
  escolha fica salva.
- Corrigido o manual sobre os dutos: entra-se com **Enter** no duto, e o **V** só serve para sair ou
  viajar quando já se está dentro.

## 0.11.0

**Novo: mandar um recado para quem faz o jogo**, direto da lista de partidas. A versão, o que você
estava fazendo e o registro de erros vão junto automaticamente.

## 0.10.1

Corrigida a tecla **T** na reunião: ela não respondia porque a leitura da tecla ficava num trecho
que o jogo pula justamente enquanto a reunião acontece.

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
