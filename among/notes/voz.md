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


