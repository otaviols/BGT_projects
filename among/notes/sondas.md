# O que as sondas ensinaram

Armadilhas de escrever sonda, todas descobertas do jeito caro. Leia antes de escrever uma: sonda que passa pelo motivo errado é pior do que sonda nenhuma.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## O que as sondas ensinaram (custou caro descobrir)

**Sonda que espera uma RECARGA tem que insistir, não chutar o instante.** As recargas correm no
relógio do SERVIDOR, que avança por tick e fica para trás do relógio de parede quando ele está
ocupado - com cinco clientes de sonda martelando, uma margem de 2 s sobre os 30 s da sabotagem
falhava numa tentativa a cada três. Pior: o sintoma é "a ação não aconteceu", sem dizer que foi a
recarga. Repita a ação até ela ser aceita, com um prazo generoso (ver a sabotagem em
`probe_engineer.nvgt`), e capture o `S_ERROR` junto do sucesso - esperar só o sucesso esconde a
explicação. **Sonda intermitente é pior que sonda que falha**: ela ensina a ignorar o vermelho.

**Sonda com VÁRIOS clientes precisa de uma CAIXA por cliente, e de esperas que bombeiam todos.** O
`wf(cliente, tipo, prazo)` das sondas de um cliente só tem dois defeitos quando há seis: ele
**descarta** tudo que não é o pacote esperado - e o `S_GAME_OVER` vem no mesmo lote do
`S_VOTE_RESULT`, então esperar um apagava o outro, com o sintoma "a partida não acabou" numa partida
acabada -, e ele só dá `update()` no cliente da vez, deixando os outros sem ACK: 40 s esperando uma
apuração derrubava os cinco restantes da sala, com o sintoma "a sala não reabriu". A forma que
funciona está em `probe_task_limits.nvgt`: um array de pacotes por cliente, uma drenagem que bombeia
todo mundo e não joga nada fora, e um `esvazia_caixa()` num lugar só por rodada (esvaziar no fim
apaga o retrato da sala reaberta, que chega junto do fim; não esvaziar nunca faz a espera achar o
retrato da ENTRADA e passar sem esperar nada).

**Teto estatístico ("afrouxa em menos de X% dos casos") é sonda intermitente disfarçada.** Duas
execuções seguidas do mesmo servidor sem teto nenhum deram 30% e 13%: a segunda teria passado.
Quando os números permitem, prefira a garantia DURA ("ninguém passou de 2") e escreva ao lado por
que ela é legítima - em `probe_task_limits` é porque 11 salas × 2 vagas cabem folgadamente nas 8
tarefas pedidas, e quem mudar esse número precisa afrouxar a linha junto.

**Sonda que testa que algo NÃO acontece precisa limpar as filas ANTES de provocar.** O
`S_PLAYER_KILLED` vai para a partida inteira, e o `wf` que esperou uma morte num cliente esvaziou só
a fila DELE: a morte anterior ficava parada nas dos outros e era lida como se fosse a morte que a
sonda estava tentando impedir - a `probe_vent_kill` reprovou um servidor correto por isso. E a
limpeza vai antes do `send`, nunca dentro da função que mede: ali ela já apagaria a resposta de
verdade se ela chegasse depressa.

**Sonda que mede um relógio precisa medir o RELÓGIO DE PAREDE junto.** "O anjo tem 53 s de 60" não
diz nada sozinho: pode ser "não rearmou" (certo) ou "não passou tempo nenhum" (sonda quebrada). Uma
linha imprimindo quanto tempo de verdade correu desde o evento separa as duas na hora - sem ela, a
`probe_meeting_reset` acusou de errado um código que estava certo, e o caminho até descobrir passou
por reler três arquivos do servidor.

**Sonda que monta texto traduzido precisa CARREGAR o idioma** (`g_i18n.load_language`), senão
`tr()` devolve a própria chave e a sonda "passa" mostrando `role.noisemaker_alarm` como se fosse a
frase. Aconteceu na primeira versão da sonda do alarmista.

**Sonda que precisa mover alguém tem que CAMINHAR, não teleportar.** `send_move` com o destino
final é recusado pelo anti-cheat de velocidade (`PLAYER_MOVE_SPEED`, 2,2/s) e o jogador não sai do
lugar - repetir o pacote não adianta. O sintoma é o pior possível: o teste passa pelo motivo
errado, porque os dois continuam colados. Interpole o trajeto (ver `anda_ate` em
`probe_guardian.nvgt`) partindo do `your_x`/`your_y` que o `S_GAME_START` manda. E cuidado com o
tamanho do elenco: uma sonda que mata duas pessoas numa partida de quatro ACABA a partida no meio
dela (um impostor contra um tripulante é vitória do impostor), e o kill seguinte é recusado sem
dizer por quê.

Para coisas que só falham no build compilado (o menu de sons, a atualização, caminhos), compile uma
sonda com `nvgt -c`, rode o `.exe` e grave o resultado num arquivo — o app compilado não tem console.

**Não confie em "compilou".** Compilar não prova que o som toca, que o pacote tem o arquivo novo, nem
que o binário sobe.


