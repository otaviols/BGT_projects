# Rede, telas e saguão

Protocolo, sessão, fila de pacotes, telas bloqueantes e sala de espera. Leia antes de mexer no protocolo ou escrever uma tela nova.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## Tradução e identidade

**O recado do saguão é a ÚNICA exceção à regra abaixo, e por um motivo que não tem volta.** Ele é
texto escrito à mão por quem opera o servidor (`infra\set_notice.ps1`, ou
`tools/server_admin.nvgt notice "..."`), e não existe chave de tradução para uma frase que ainda não
foi escrita. Por isso ele viaja como TEXTO, e o cliente sempre o anuncia atrás de um rótulo que ELE
traduz ("Recado do servidor:") - sem o rótulo, soaria como a fala de outro jogador. Duas decisões
que o sustentam: ele vai em TODO `S_HALL_STATE`, e não uma vez no login (avisar de uma manutenção só
alcançaria quem ainda não chegou), e quem decide se vale falar de novo é o cliente, que guarda o
último que ouviu; e ele mora no BANCO (`server_settings`), porque um recado que some no próximo
deploy não serve para avisar de nada - o que obriga a LEMBRAR DE APAGÁ-LO quando deixar de valer.
Sonda: `tools/probes/probe_hall_notice.nvgt` (precisa do `AMONGUS_ADMIN_TOKEN`).

**Texto do servidor viaja como chave de tradução, nunca como frase pronta.** Os jogadores de uma
partida podem estar em idiomas diferentes, e quem sabe o idioma de cada um é o cliente dele. Ver
`MSG_KEY_FIELD` e `tr_server_message` em `src/network/protocol.nvgt`.

**Idiomas são plugáveis.** Basta pôr um `.json` em `lang/` para o idioma aparecer no jogo; o nome dele
sai da chave `language.name`, no próprio idioma. O que faltar cai no inglês. **Ao acrescentar
qualquer texto novo, acrescente a chave nos dois idiomas.**

**Nada de identidade é traduzido** — nome de jogador, nome de bot. Traduzir faria duas pessoas na
mesma partida acusarem "nomes" diferentes pela mesma pessoa.

## Quem decide o quê: servidor e cliente

Os princípios estão no CLAUDE.md (validar no servidor, desfazer nos dois lados, uma função só para a
regra que os dois aplicam). Aqui ficam os casos concretos que os originaram.

**O posto de câmeras: estado desfeito só de um lado.** O servidor derrubava o posto
de câmeras (sabotagem, reunião, morte) e avisava a partida inteira qual sala estava sob observação -
menos a única pessoa que precisava saber, a que estava observando. O cliente dela continuava na
câmera: parado, ouvindo outra sala, apertando ESC sem efeito porque para o servidor ele já tinha
saído. Sintoma: jogador travado sem mensagem nenhuma. Por isso `release_cameras_any()` devolve QUEM
foi liberado, e todo lugar que fecha o posto passa por `close_cameras()`.


## Telas, laço do cliente e fila de pacotes

**`dictionary.get(chave, valor&out)` com chave AUSENTE deixa `valor` com LIXO.** É como o
AngelScript trata parâmetro `&out` que a função não escreveu - o valor inicial da variável NÃO é
preservado. Foi o bug "todo mundo votou nele e deu empate": sem nenhum "pular" a chave `-1` não
existia, a contagem de pular saía com um número qualquer e ganhava a apuração. Só com alguém
pulando a chave existia e funcionava - sintoma que parece regra de jogo, não bug. Sempre
`exists()` antes de `get()`, ou use o retorno booleano de `get()`.

**Tela bloqueante ANUNCIA sem consumir.** Enquanto uma minigame (ou o painel de reparo, ou a caixa
de mensagem) está aberta, o laço principal não roda e nada do que chega é dito - o jogador ficava
surdo ao chat e à contagem do oxigênio até fechar a tela. `announce_background_events`
(`src/core/event_speech.nvgt`) fala o que chegou e MARCA o pacote (`announced`), que continua na
fila para o laço principal tratar com todos os efeitos dele; quem fala depois pergunta antes
(`if (!pkt.announced) speak(...)`). Consumir o pacote ali perderia os efeitos - elenco, alarme,
marcadores - e o jogo passaria a dizer coisas que não aconteceram. O texto de cada evento mora num
lugar só, em `event_speech.nvgt`, porque agora há dois pontos que o dizem. O gancho por quadro das
minigames é `task_tick()`, que anuncia e responde se a task deve parar - `task_cancel_requested`
sozinho não anuncia nada. E, pela mesma razão que derrubou o jogo antes, cada pacote é examinado
UMA vez (`announce_checked`): nada de reexaminar a fila inteira por quadro. **Decisão: nenhum aviso
larga a task por conta própria** (nem o oxigênio) - avisar é do jogo, decidir é do jogador.
Sonda: `tools/probes/probe_background_events.nvgt`.

**A fila de pacotes do cliente tem TETO, e nada pode varrê-la por quadro.** A fila só é esvaziada
pelo laço principal, e o jogador passa minutos fora dele (task, câmera, caixa de mensagem) enquanto
os pacotes continuam chegando - posição de outro jogador chega a cada quadro DELE, então numa
partida cheia são centenas por segundo. O que derrubava o jogo não era o tamanho em si: era
`task_cancel_requested` varrendo a fila INTEIRA a cada quadro para procurar reunião/corpo/fim. Com
10 mil pacotes isso custava 13,9 ms por quadro (medido em `tools/probes/probe_queue.nvgt`), o
quadro atrasava, a fila crescia mais, e a janela parava de responder - o "não está respondendo" no
meio da partida que vários jogadores relataram. Duas correções: `game_client.enqueue` marca
`interrupt_pending` na CHEGADA (a consulta virou O(1)) e descarta a posição mais antiga acima de
`MAX_INCOMING_PACKETS`. Só POSIÇÃO é descartável - ela é estado, a mais nova substitui a anterior e
já viaja pelo canal não confiável; descartar um EVENTO deixaria o cliente contando uma partida que
não existe. Ao escrever qualquer tela bloqueante nova, nada de varrer `client.incoming` por quadro.

**Tela aberta com o jogador CONECTADO tem que servir a rede - o prazo é de 18 segundos.** O NVGT
configura o peer com `enet_peer_timeout(128, 10000, 35000)`: sem `client.update()`, nenhum ACK sai,
e o servidor tira o jogador da sala. Medido em `tools/probes/probe_keepalive.nvgt`: 18 segundos com
a sala conversando - o tempo de mexer num controle de volume. Foi assim que abrir as configurações
de dentro da sala derrubava o jogador dela. Todo menu ganha `background_callback` e todo laço de
`audio_form` ganha um `update()`; quem chama passa o cliente (null no menu inicial, onde não há
conexão). E cuidado com o que a sonda mede: perguntar ao CLIENTE parado se ele ainda está conectado
responde sempre que sim, porque ele nem olhou a rede - o sinal honesto é o servidor mandando
`S_PLAYER_LEFT` a quem ficou.

**Tela de fora com laço fechado não dá para consertar - reescreva.** `tts_config` (speech.nvgt) não
tem gancho nenhum, então a configuração de voz não tinha como servir a rede. Foi reescrita aqui
(`run_voice_screen`), o que de quebra resolveu ela ser em inglês fixo; os valores continuam saindo
e entrando por `tts_dump_config`/`tts_load_config`, então nada do que estava salvo se perde.

## Rede, protocolo e sessão

**Versão de PROTOCOLO é separada da versão do jogo, e o corte se faz em UM número.**
`PROTOCOL_VERSION` (o que o cliente manda no `C_LOGIN`) e `MIN_PROTOCOL_VERSION` (o que o servidor
aceita), em `config/game_constants.nvgt`. Hoje os dois estão em **2**: quem está abaixo da 0.30.0
não entra. A recusa é um `S_LOGIN_RESULT` com `ok=false` e `message = login.update_required` - o
cliente avisa e abre a página de download -, e não uma desconexão: na desconexão ele só veria a
rede cair. O servidor guarda o protocolo de cada conexão (`game_player.protocol_version`) para
poder decidir, por sala, o que oferecer a quem.

**Subir o mínimo é o que PERMITE apagar compatibilidade**, e foi assim que o legado saiu na 0.31.0.
Duas cautelas, nessa ordem: só corte quando a base já estiver numa versão que saiba **explicar** a
recusa (um cliente que não tem a chave `login.update_required` fala a chave crua, e a pessoa vai
rever a senha em vez de atualizar); e decida pelos **dados**, não pela impressão - `read_feedback`
mostra a versão de cada recado, e foi ele que mostrou que a base migra no mesmo dia em que a versão
sai. Sonda: `tools/probes/probe_protocol.nvgt` (testa a recusa mandando -1, então vale contra
produção seja qual for o mínimo).

**Uma conta, uma sessão - e a entrada NOVA derruba a velha, não o contrário.** Recusar a segunda
entrada parece mais educado e é pior: uma conexão que caiu feio continua de pé para o servidor até o
ENet desistir dela, e a pessoa ficaria sem conseguir voltar ao próprio jogo. `drop_other_sessions`
usa `disconnect_peer_softly` (o ENet entrega o que está na fila antes de desligar, então o aviso do
porquê chega) e chama `on_disconnect` na mão: os três `disconnect_peer*` já tiram o peer do mapa da
rede, então esperar o evento deixaria a sessão velha pendurada na sala. Sonda:
`tools/probes/probe_uma_sessao.nvgt`, que cobre também o caso que o jogador vive - ser derrubado
estando dentro de uma sala, com o anfitrião passando para quem ficou.


## Sala de espera e saguão

**O saguão é derivado, não declarado.** "Estar no saguão" é `autenticado && lobby_id == ""`
(`in_hall` no servidor) - não existe um "entrar no saguão" que o cliente peça, justamente para não
haver um segundo estado capaz de discordar do primeiro. O preço é lembrar de chamar
`send_hall_state()` em TODO caminho que muda essa condição: login, criar sala, entrar, sair, queda,
e sala fechada. Criar sala foi o esquecido - a sonda pegou (a lista continuava com quem já tinha
saído, embora a conversa já o excluísse, porque o envio recalcula na hora). Sonda:
`tools/probes/probe_hall.nvgt`. No cliente, os pacotes do saguão são retirados da fila por
`drain_hall_packets`, chamado TAMBÉM de dentro das esperas por resposta do servidor: elas descartam
o que não esperam, e sem isso uma mensagem que chegasse durante a busca da lista de partidas sumia.

**A sala SOBREVIVE à partida - o que morre é o estado dela.** No fim do jogo a lobby volta a
"waiting" com a mesma gente dentro (`reopen_lobby` no servidor, `reopen_for_next_match` no
game_state), em vez de fechar e jogar todo mundo no navegador de partidas. Duas coisas têm que
acontecer juntas, e esquecer qualquer uma vira bug sem causa visível na partida seguinte: **o
estado de partida da SALA** sai em `reopen_for_next_match` (corpos, votos, reunião, sabotagem,
câmera, portas, vencedor) e o **estado do JOGADOR** em `reset_match_state`. Ao acrescentar campo de
partida em qualquer um dos dois, acrescente a limpeza no mesmo commit. Quem saiu no meio é
REMOVIDO da lista ao reabrir (o registro dele só existia para a contagem de vivos não quebrar), e
o anfitrião é reatribuído se tiver caído. O cliente reconhece a volta pelo `S_LOBBY_STATE` que vem
logo depois do `S_GAME_OVER`: `run_game` devolve esse retrato e `run_session` entra direto na sala
de espera com ele (null = navegador, como antes). Sondas: `tools/probes/probe_rematch.nvgt` (duas
partidas seguidas na mesma sala) e `probe_leavemid.nvgt` (anfitrião sai no meio).

**Quem sai da sala de espera precisa de um S_LOBBY_STATE novo para os que ficaram.** A lista de
quem está na sala (tecla P) sai do último retrato; só o S_PLAYER_LEFT não a atualiza, e quem saiu
continuava listado. `remove_from_lobby` manda o retrato quando não há partida em curso.


