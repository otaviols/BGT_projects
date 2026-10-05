# Chat de voz

Captura, codec, reprodução e quem ouve quem. Leia antes de mexer em voz.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## Chat de voz

**Chat de voz: Opus NÃO decodifica ao vivo nesta build do NVGT; o codec é μ-law em script.**
`src/audio/voice_chat.nvgt` captura com `microphone.read()`, comprime em G.711 μ-law a 16 kHz
(128 kbps por pessoa falando, qualidade de telefone) e toca com `sound.stream_pcm()`. Não é Opus
porque o `audio_decoder` (opusfile) só abre fluxo COMPLETO - num fluxo que cresce responde
"Invalid file" sempre, com ou sem taxa/canais. Trocar por Opus um dia é trocar `voice_encode`/
`voice_decode` e nada mais. Armadilhas achadas pela sonda (`tools/probes/voice_probe.nvgt`):
`spatialization_enabled`/`set_position_3d` definidos ANTES do primeiro `stream_pcm` se perdem (som
centralizado, sem erro) - reaplique depois de cada quadro; microfone estéreo não espacializa
(converta para mono); `mic.read(n)` devolve n quadros × canais amostras; e as listas de
`get_sound_input_devices()`/`get_sound_output_devices()` NÃO têm o item 0 "sem som" que a
documentação descreve - o índice é o da própria lista (`sound_output_device = 1` era o segundo
aparelho). Por isso os dispositivos são guardados pelo NOME e resolvidos na hora.

**`microphone.read(n)` devolve SEMPRE n quadros, tenha ou não n quadros novos; `read(0)` devolve
só o que há.** Medido: `read(960)` a cada 5 ms rendia 169 mil quadros/s de um microfone de 48 mil -
3,5x mais dados que fala, e a voz chegava embaralhada ("qualidade terrível" da 0.26.0). Capture
sempre com `read(0)` e acumule até dar um pacote.

**Reprodução de voz ao vivo precisa de folga E de detectar que ela secou.** `stream_pcm` toca o que
tem e, quando falta, toca silêncio sem avisar - cada pacote 30 ms atrasado era um estalo, e uma
cadência 5% mais lenta que o relógio do áudio (o que qualquer `wait(20)` produz, porque dura 21-22
ms) esvaziava a fila aos poucos e o som "tremia da metade para o final". O cliente junta 120 ms
antes de começar cada fala, contabiliza quanto entregou versus quanto tempo passou, e quando a folga
chega a zero refaz a folga (um buraco de 120 ms uma vez, em vez de tremer sem parar). Buffer de
2 s explícito: o automático é 2x o primeiro pedaço. Quem GERA áudio sintético para teste
(`probe_talker`) tem que marcar a cadência pelo tempo total, não por "20 ms desde o último envio":
reiniciar o relógio perde o resto e a entrega fica lenta - foi um defeito da sonda que pareceu
defeito do jogo. Sondas (em `tools/probes/`): `probe_sender` (mede a captura real: esperado 50 pacotes/s),
`probe_playback` (de ouvido, cinco caminhos de reprodução), `probe_stream` (stream_pcm sob tremor).

**...e precisa também de se proteger de ela ENCHER: com o buffer cheio, `stream_pcm` BLOQUEIA.** O
lado de secar estava coberto; o de encher, não. Medido em `probe_stream_block`: com o buffer de 2 s
cheio, cada quadro de 20 ms segura a chamada uns 50 ms esperando o som consumir (escrever 4 s de voz
levou 10 s de relógio), e com o som PARADO a chamada nunca volta. Sintoma: "o jogo não responde", sem
`crash.log` (não é exceção de script - o jogo está preso numa chamada nativa). É o que explicava
recados antigos e seguidos - #49, #66, #76 ("no responde" no meio da partida), #88/#111 ("quando eu
falo, o jogo dos outros trava") - e o travamento que o usuário viveu na barriga do lobo, onde a voz
chega sem parar. O buffer enche quando a voz chega em RAJADA: qualquer tela ou `wait` que deixe a rede
um tempo sem atender (o `wait(1500)` do resultado da votação, uma tarefa) acumula pacotes, e eles
chegam todos de uma vez. Conserto (`on_voice_frame`): com mais de `VOICE_MAX_BUFFERED_MS` (1 s)
esperando, o quadro é DESCARTADO (`frames_dropped`); e se o som da pessoa não está mais tocando, a voz
é refeita do zero (`renew_stream`) em vez de receber dados que ninguém consome. Sonda:
`probe_voice_no_freeze` (uma rajada de 5 s, e o som parado à força no meio da fala).

**A taxa da captura NÃO é sempre 48 kHz: é a da saída de som de cada computador.** O motor padrão do
NVGT nasce sem taxa (`new_audio_engine` no `sound.cpp` dele) e adota a nativa do aparelho de saída -
48 000 aqui, 44 100 em muitos fones e placas -, e o microfone é aberto na taxa do MOTOR. A conversão
para 16 kHz era "média de 3", certa só a 48 000: a 44 100, quem FALAVA saía 8,8% mais agudo e mais
rápido para todos (relato do usuário, "algumas vozes um pouco mais finas"; por depender de quem fala,
não era em todos). Hoje a taxa vem de `sound_default_engine.sample_rate` ao abrir o microfone
(`mic.sample_rate` responde 0, não sirva-se dele), o pacote tem 20 ms daquela taxa e
`voice_encode_from` converte por janela fracionária; a 48 000 o caminho é byte a byte o antigo. Sonda:
`probe_voice_rate.nvgt` (tom de 440 Hz em quatro taxas, sem microfone). Para simular um computador
desses: `@sound_default_engine = audio_engine(24, 44100)` antes de abrir o microfone.

**A tecla de falar é uma letra: toda caixa de texto liga `g_voice.typing`** (chat, regras da sala)
enquanto está aberta, senão digitar a letra abre o microfone no meio da mensagem.

**A voz viaja num canal próprio, binário, e fora da fila de pacotes.** `CHANNEL_VOICE` (2) não
carrega JSON (`voice_wire`/`voice_unwire` em `protocol.nvgt`); cliente antigo tenta ler como JSON,
falha e descarta. No cliente ela não entra em `incoming`: é entregue a `game_client.voice` dentro de
`update()`, que é a única coisa que TODA tela bombeia - na fila, quem abrisse uma task ficava surdo
até fechá-la. Quem ouve quem é decidido no SERVIDOR (`game_state.can_hear_voice`): sala de espera
e reunião, todos; na nave, `VOICE_RANGE` (8); dentro de duto, ninguém é ouvido; fantasma ouve todos
e só é ouvido por fantasma. O servidor só manda voz a quem mandou `C_VOICE_STATE` ligado - sem
isso um cliente antigo receberia 128 kbps que não sabe tocar. Regra da sala `voice_chat` (padrão
ligado). Testes: `tools/probes/probe_relay.nvgt` (relay e distância, contra servidor local) e
`tools/probes/probe_talker.nvgt` (um "jogador" que manda um tom de 440 Hz, para ouvir a voz
posicionada no jogo de verdade com `AMONGUS_SERVER_HOST=127.0.0.1`).


