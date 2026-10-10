# Among Us Audiogame — guia do projeto

Jogo de dedução social **jogado inteiramente por som**, escrito em [NVGT](https://nvgt.gg)
(AngelScript). Cliente Windows distribuído por um site estático; servidor dedicado rodando como
contêiner Docker numa VPS da Hostinger (até 2026-10-09, num cluster AKS do Azure). Está em **beta**,
com jogadores reais usando.

Tudo que o jogador percebe passa por leitor de tela e áudio posicionado — **não existe informação
visual**. Ao decidir qualquer coisa de interface, a pergunta certa é "como isso soa?", não "como isso
aparece".

**Como este guia está organizado.** Aqui fica o que vale em TODA sessão: estrutura, como compilar,
como publicar, as armadilhas da linguagem e as pendências. O conhecimento de cada área mora em
`notes/`, e a regra é simples: **antes de mexer numa dessas áreas, leia o arquivo dela.** Não é
sugestão — cada parágrafo daqueles arquivos custou horas a alguém, e a única forma de uma lição se
perder é não ser lida.

| Vou mexer em… | Leia ANTES |
|---|---|
| som novo, volume, arquivo de áudio, bip | [notes/som.md](notes/som.md) |
| papel/profissão, habilidade | [notes/papeis.md](notes/papeis.md) |
| mapa, sala, duto, sorteio de tarefas | [notes/mapa-e-tarefas.md](notes/mapa-e-tarefas.md) |
| **inventar ou escrever um minijogo** | [notes/tarefas-novas.md](notes/tarefas-novas.md) |
| começo/fim de partida, reunião, sabotagem, morte | [notes/ciclo-da-partida.md](notes/ciclo-da-partida.md) |
| protocolo, login, sessão, tela, menu, fila de pacotes, saguão | [notes/rede-e-telas.md](notes/rede-e-telas.md) |
| chat de voz | [notes/voz.md](notes/voz.md) |
| escrever uma sonda | [notes/sondas.md](notes/sondas.md) |
| publicar, servidor, VPS, Docker, banco | [notes/infra-e-deploy.md](notes/infra-e-deploy.md) |
| idioma, `lang/`, envio de tradução | [notes/traducoes.md](notes/traducoes.md) |

As **Armadilhas do NVGT**, abaixo, valem para tudo: são coisas da linguagem e da engine que falham em
silêncio, e é onde procurar quando algo "compilou e não funcionou".

## Mantenha estas notas vivas

**Aprendeu algo que teria economizado tempo se estivesse escrito? Escreva na mesma sessão.** Isto não
é opcional nem "se sobrar tempo": é parte de terminar o trabalho, junto com o commit. Cada armadilha
registrada custou horas a alguém; o que não for escrito será redescoberto do mesmo jeito caro.

**Onde escrever:** no arquivo de `notes/` da área, se a lição é sobre som, papel, mapa, partida,
rede, voz, sonda, infra ou tradução — que é o caso da grande maioria. **Aqui** só entra o que vale
para toda sessão, independentemente do que se esteja mexendo: como compilar, como publicar,
armadilha da linguagem, princípio do projeto, pendência. Na dúvida, vai para `notes/`: este arquivo é
carregado inteiro em toda sessão, e cada linha que não serve àquela sessão é custo em toda outra.

Vale registrar:

- **Armadilha** que fez algo falhar de forma enganosa — sempre com o **sintoma**, não só a causa. O
  sintoma é o que se vê primeiro, e é por ele que a pessoa vai procurar.
- **Decisão** de projeto e a alternativa descartada, quando alguém possa querer desfazê-la sem saber
  o que ela evitava.
- **Passo que se esquece** e falha em silêncio.
- **Pendência conhecida**, para não ser redescoberta como se fosse bug novo.

E, com o mesmo peso: **se algo aqui deixou de ser verdade, corrija na hora**. Estes textos são
tratados como verdade — uma instrução errada é pior do que instrução nenhuma, porque leva a decisões
erradas com confiança. Ao mudar caminho de arquivo, comando de deploy, nome de recurso ou padrão,
confira se este documento e o de `notes/` ainda descrevem a realidade.

Antes de afirmar algo, **verifique contra o código**, não contra a memória da conversa.

## Estrutura

| Caminho | O que é |
|---|---|
| `AmongUs.nvgt` | ponto de entrada do cliente (no Windows vira `AmongUsGame.exe`) |
| `launcher.nvgt` | o iniciador: no Windows é o `AmongUs.exe` que o jogador abre - ver "O iniciador" em [notes/infra-e-deploy.md](notes/infra-e-deploy.md) |
| `server_main.nvgt` | ponto de entrada do servidor |
| `src/` | **todo** o código: `config/`, `core/`, `game/`, `network/`, `ui/`, `database/`, `audio/`, `i18n.nvgt` |
| `lang/` | **só dados** de tradução (`pt_BR.json`, `en_US.json`) — o motor de i18n fica em `src/` |
| `sounds/` | áudio fonte; vira `sounds.dat` no build |
| `tools/` | `build_pack` (gera o `sounds.dat`), `check_sounds`, `bots`, `build_clients.ps1`, `sync_translations.ps1`, `check_translation.py`, ferramentas de administração, `probes/` (sondas que conversam com um servidor de verdade) |
| `docs/` | manuais e histórico de versões, distribuídos com o jogo numa pasta `docs/` |
| `notes/` | as notas de projeto por área (som, papéis, mapa, partida, rede, voz, sondas, infra, traduções). **Não** vai para o jogador — é `docs/` que vai |
| `infra/` | Dockerfile, `deploy.ps1`, `admin.ps1` (recados, avisos, traduções, status - tudo da administração), `vps.ps1` (o acesso à VPS); Terraform e manifests do Kubernetes são da época do Azure |

`lang/` fica fora de `src/` de propósito: é lido por caminho em tempo de execução, e esse caminho
precisa ser o mesmo rodando do fonte ou do build compilado.

Os manuais e o histórico moram em `docs/` e chegam ao jogador numa pasta `docs/` dentro da pasta
do jogo (`LEIAME.md`, `README.md`, `NOVIDADES.md`, `CHANGELOG.md`) — o `#pragma document` aceita
`origem;destino`, e o destino pode ter subpasta. Não vão na raiz de propósito: quatro textos soltos
ao lado do executável poluíam a pasta. O menu "Novidades" lê esses arquivos (ver
`src/ui/changelog_screen.nvgt`), procurando primeiro os nomes do jogo compilado e depois os do
repositório, para funcionar rodando do fonte.

## Compilar

```
nvgt tools/build_pack.nvgt          # regera sounds.dat a partir de sounds/
nvgt -c AmongUs.nvgt                # cliente Windows -> AmongUs.zip
nvgt -c -plinux server_main.nvgt    # servidor (Linux, para o contêiner)
tools\build_clients.ps1             # Windows + Linux (+ Mac se houver stub), com os nomes certos
```

**O NVGT grava TODO build do cliente como `AmongUs.zip`, seja qual for a plataforma.** Compilar
`-plinux` depois do Windows sobrescreve o zip do Windows em silêncio - e o deploy publicaria um
binário Linux como se fosse Windows. Use `tools\build_clients.ps1`, que renomeia cada um
(`AmongUs-linux.tar.gz`, `AmongUs-mac.iso`) e deixa o Windows por último. A extensão do Linux
mudou de `.zip` para `.tar.gz` entre duas builds do NVGT 0.90.0-dev - o script aceita as duas e
mantém a que saiu; se um dia mudar de novo, é ali que se acrescenta o nome novo.

**O produto do Mac fora de um Mac é `AmongUs.iso`, não zip nem dmg.** O NVGT só gera `.dmg` no
macOS (precisa do `hdiutil`); em outros sistemas grava um ISO 9660 com Rock Ridge, que preserva os
bits de execução e o macOS monta com dois cliques (`src/bundling.cpp` do NVGT). Sai grande (~73 MB)
porque ISO não comprime.

**Mac precisa do stub `stub/nvgt_mac.bin` e de `lib_mac/`** - já instalados nesta máquina (vieram
do instalador oficial de nvgt.gg, componente "MacOS binary stub"). Não estão no repositório
`D:\git\nvgt`: o stub é produto de compilar o NVGT num Mac. Se sumirem numa reinstalação, o
`-pmac` falha com "File not found: stub/nvgt_mac.bin" e o `build_clients.ps1` pula o Mac avisando.
Os builds de Linux e Mac são experimentais: compilam, mas ninguém os rodou; fora do Windows o
updater só avisa e abre o site.

**Mexeu em qualquer arquivo de `sounds/`? Rode o `build_pack` antes de compilar.** O jogo empacota o
`sounds.dat`, não a pasta — sem regerar, o build sai com o som antigo e nada avisa.

### Android

```
tools\build_android.ps1        # -> AmongUs-android.apk, assinado, ~21 MB
```

O APK sai pronto e **assinado**, sem instalar nada: a instalação do NVGT traz o `aapt2`, o
`apksigner`, o `zipalign`, um Java e até o `adb` em `android-tools/` — as variáveis `ANDROID_HOME` e
`JAVA_HOME` podem estar vazias. Dura cerca de cinco segundos. Fora do `build_clients.ps1` de
propósito: o site ainda serve Windows, e o updater do jogo não sabe instalar APK.

**O build "trava para sempre" em `signing APK...` = o Java do NVGT CAIU.** O Java 17.0.8 que vem em
`android-tools/java17` morre ao compilar código (JIT) neste Windows (build 26300): `EXCEPTION_ACCESS_VIOLATION`
em `jvm.dll`, um `hs_err_pid*.log` na pasta, e a janela de erro do Windows segurando o processo - sem
nada na saída. O `build_android.ps1` roda o build com `JAVA_TOOL_OPTIONS=-Xint` (só interpretador),
que assina em ~10 s; `-XX:TieredStopAtLevel=1` NÃO basta. Apareceu na 0.50.0, dois dias depois de a
0.46.0 compilar normal - foi o Windows que mudou, não o projeto.

**O build "trava por dez minutos" = a pergunta de instalar no aparelho.** `build.android_install`
vale 1 por padrão, que significa "perguntar", e a pergunta é um diálogo esperando resposta que nada
anuncia. O script passa `0`.

**A chave de assinatura mora em `%USERPROFILE%\.nvgt_android.keystore`, FORA do repositório, e o
NVGT cria uma nova em silêncio se não achar** (a linha "creating signature keystore..." no build). APK
assinado com outra chave não atualiza o instalado: o Android recusa, e o jogador tem que desinstalar
e perder preferências e conta lembrada. Descoberto ao compilar num segundo computador. Ao trocar de
máquina, COPIE esse arquivo antes do primeiro build de Android - e guarde uma cópia fora do disco.

**O identificador `com.amongusaudiogame.game` se escolhe UMA vez.** No Android ele é o caminho da
pasta de dados do aplicativo: trocá-lo depois de alguém instalar não atualiza nada, cria um segundo
aplicativo e o jogador perde preferências e conta lembrada.

**Toda a configuração do bundle vai por `-s chave=valor` na linha de comando, e não num `.nvgtrc`.**
A versão é LIDA de `GAME_VERSION`; gravada num arquivo de configuração, ela seria um segundo lugar
para lembrar de subir junto — a armadilha que o `version.json` já ensinou.

**O microfone precisa de um manifesto nosso.** O template do NVGT vem com a permissão comentada, e
sem ela o chat de voz fica mudo sem dizer por quê. `tools/android/AndroidManifest.xml` é a cópia do
template do stub com `RECORD_AUDIO` ligado, passada por `build.android_manifest`; o script confere
que ela chegou no APK. Ao atualizar o NVGT, compare com a entrada `AndroidManifest.xml` de
`stub/nvgt_android.bin` — um template novo não avisa que o nosso ficou para trás. A permissão ainda
tem que ser pedida ao jogador em tempo de execução (`request_voice_permission`, na abertura).

**Dentro do APK, "o arquivo existe?" e "dá para ler o arquivo?" DISCORDAM.** Os assets ficam
empacotados: `file_exists` (que usa `stat`) responde **false**, e `file_get_contents` (que passa pelo
SDL) devolve o conteúdo. Foi por isso que o jogo no celular não carregaria idioma nenhum e falaria o
nome cru de toda chave. Regra: sobre arquivo que veio de `#pragma asset`/`#pragma document`, **nunca
pergunte se existe — tente ler** e trate vazio como ausente. Vale para `directory_exists` também. O
`sounds.dat` escapou por sorte: o `pack_file` já abre por SDL.

**E `find_files` não enumera nada dentro do APK** — não há pasta para listar, então a lista de
idiomas voltava vazia e o jogador ficava preso no padrão. Para isso existe `lang/index.list`
(gerado pelo script, a partir da própria pasta, em todo build de Android). A pasta continua tendo
precedência: rodando do fonte um idioma novo aparece na hora, e um índice velho nunca esconde um
arquivo que está ali. Sonda: `tools/probes/probe_android_assets.nvgt`, que roda **no PC** justamente
porque esse caminho só rodaria num celular — um defeito nele apareceria como "a tela de idiomas está
vazia", com um build e uma instalação por tentativa.

**`PLATFORM` diz "Linux" no Android** (sai de `Environment::osName()`), então `PLATFORM == "android"`
é false para sempre, sem erro. Quem responde é `is_android()` em `src/core/platform.nvgt`, sobre
`ANDROID_SDK_VERSION` (-1 fora do Android). O updater não precisou de nada: `system_is_unix` é
verdadeiro lá, então ele já cai no caminho "avise e abra a página" em vez de procurar PowerShell.

O APK entra no deploy junto dos outros pacotes (`AmongUs-android.apk`, na lista de extras do
`deploy.ps1`), mas **não é gerado pelo `build_clients.ps1`** - rode `tools\build_android.ps1` antes
de publicar uma versão que deva levá-lo, senão o site fica com o APK da versão anterior sem nada
avisar.

**Gestos: `src/core/touch_input.nvgt`, ligado só no Android.** Cada gesto vira a MESMA tecla que o
teclado apertaria, então menus, formulários e minijogos não têm segunda interface. NÃO usa o
`touch_keyboard_interface` do NVGT de propósito: aquele só enxerga toques se o `monitor()` dele for
chamado a cada quadro em TODO laço - dezenas de lugares para esquecer, cada um uma tela surda. Aqui os
gestos são resolvidos dentro dos próprios eventos de toque do motor, que chegam em qualquer `wait()`;
o preço é não usar nada que dependa de relógio (toque longo). O único aviso é o do laço da partida
(`touch_match_frame()`, também no Conhecer o mapa): enquanto ele chama, a metade esquerda vira o manche
que SEGURA as teclas de andar e a direita faz as ações; um menu aberto para de chamar e a tela volta
sozinha aos gestos de menu. Tecla com Shift (radar para trás) deixa o Shift seguro até o quadro
seguinte - soltado junto, o jogo já o leria solto. O mapa dos gestos está nas funções
`touch_gesture_*` e no texto `touch.help`: mudou um, mude o outro. Sonda (no PC, sem aparelho):
`tools/probes/probe_touch.nvgt`. O deslize dispara assim que o dedo passa do limiar (depois de
`TOUCH_EARLY_SWIPE_MS`, o tempo de os outros dedos pousarem), e não quando ele sai da tela - esperar a
saída somava atraso (recados #161 e #165). Os números dos painéis são um MODO (três dedos abrem, um
dedo escolhe, toque duplo digita). Escrever abre o teclado da tela sozinho: todo laço de `audio_form`
tem um `screen_keyboard_follower` (`src/core/screen_keyboard.nvgt`) que chama `start_text_input()` com
o foco num campo de texto - funções que a documentação do NVGT NÃO lista (estão no `src/input.cpp` dele;
por meses achamos que não existiam). Formulário novo: ponha o seguidor, senão no celular o campo fica
surdo. Sonda: `probe_screen_keyboard.nvgt`. Nenhum de nós tem
aparelho: tudo aqui foi feito às cegas e é validado pelos jogadores.

**O leitor de tela TOMA os toques, e o jogo não tem como evitar sozinho.** O
`dev.nvgt.capability.DIRECT_TOUCH` do manifesto do NVGT não é pedido ao Android: só o lê o **NVGT
Bridge** (github.com/aryanchoudharypro/NVGTBridge), um app à parte, instalado por APK, que desliga a
exploração por toque enquanto o jogo está na frente - com TalkBack ou outro leitor que use exploração
por toque. Sem ele, o jogador desliga a exploração por toque à mão (recados #222 e #231). E o
manifesto é também onde se passa variável ao SDL antes de ele iniciar: `<meta-data
android:name="SDL_ENV.<HINT>">`. É por aí que vão três decisões nossas em
`tools/android/AndroidManifest.xml`, e o `build_android.ps1` confere que cada uma chegou no APK:
`SDL_ANDROID_BLOCK_ON_PAUSE=0` (por padrão o SDL CONGELA o jogo quando ele sai do primeiro plano - a
pergunta do microfone, a aba de notificações - e congelado a rede não é atendida: em 18 s o servidor
derruba; é a suspeita do "caio do servidor quando a partida começa", recado #161); a horizontal, por
causa do estéreo dos alto-falantes do celular (recado #165); e, para a horizontal valer,
`SDL_ORIENTATIONS=LandscapeLeft LandscapeRight`. **O `sensorLandscape` da atividade sozinho NÃO
basta:** ao abrir a janela o SDL chama `setOrientation` e, sem a dica e com a janela redimensionável,
pede `FULL_USER`, que segue a trava de rotação do celular. Sintoma: o jogo abre em pé, sem erro - foi
assim da 0.43.1 à 0.51.5 (recado #233), com o manifesto dizendo horizontal o tempo todo.

**O jogo anota no `crash.log` quando a rede fica 3 s ou mais sem ser atendida, e quando a conexão
cai** (`log_diagnostic`, com o contexto em `g_net_stall_context`). O `crash.log` segue junto de todo
recado enviado pelo jogo, então o próximo "caí do servidor" chega com a causa - é o substituto de ter o
aparelho. Sonda: `probe_net_stall.nvgt`. Três cuidados que ele já pediu: "a rede ficou N s sem ser
atendida" é o JOGO parado, não a internet (queda de rede de verdade é "conexão perdida por TEMPO
esgotado"); só cliente CONECTADO mede pausa (o treino usa um que nunca conecta, e anotava pausa a cada
volta ao menu); e o recado leva só o trecho NOVO desde o último recado confirmado (`unsent_crash_log`,
sonda `probe_crash_log_unsent`) - inteiro, uma linha velha ia em todo recado para sempre, e todo recado
chegava "com crash.log". O contexto na lista de partidas é por passo e por tela (`lobby_browser.nvgt`).

## Publicar uma versão

Sempre, e nesta ordem:

1. **Suba `GAME_VERSION`** em `src/config/game_constants.nvgt`.
2. **Escreva a entrada no changelog**, nos DOIS idiomas: `docs/CHANGELOG_ptBR.md` e
   `docs/CHANGELOG_enUS.md`, no topo, como `## x.y.z`. A entrada diz **o que a versão traz para quem
   joga** — nunca detalhe interno (nome de arquivo, refatoração, como o bug era no código). O texto é
   FALADO ao jogador na atualização; "os papéis passaram a declarar o que podem fazer" não significa
   nada para ele e expõe o interno à toa. Se uma versão não muda nada visível, diga só isso.
3. **Gere o version.json**: `python tools/make_version_json.py`. Ele sai do changelog — não edite o
   `version.json` à mão, ou as duas descrições da mesma versão vão divergir. Essa relação fica
   registrada AQUI, e não no cabeçalho do changelog: o changelog vai para o jogador (é o
   `NOVIDADES.md` da pasta do jogo), e nome de script é informação interna. O script recusa se a
   versão do changelog não bater com `GAME_VERSION`, ou se um dos idiomas estiver faltando.
4. **Commite**, e rode `tools\release.ps1` (sem redirecionar a saída). Ele confere versão, árvore
   limpa e site; regera o `sounds.dat` se um som mudou; compila clientes, Android e - só se o código
   dele mudou desde o commit no ar - o servidor; faz o push, o deploy com aviso aos jogadores, e confere
   no ar o que ficou publicado. `-Conferir` faz só as conferências. Os passos à mão continuam
   possíveis (`build_clients.ps1`, `build_android.ps1`, `infra\deploy.ps1 [-SkipServer]`), mas cada
   um deles já escapou calado uma vez.

As duas versões **têm que bater**. O `version.json` é o que os clientes instalados comparam contra si
mesmos: se ele ficar para trás, ninguém é avisado da atualização.

**Deploy do servidor derruba quem está jogando - por isso ele AVISA antes.** O estado das partidas
vive na memória do processo; trocar o servidor no meio de uma partida derrubou todo mundo sem aviso (e
foi assim que se descobriu). O `deploy.ps1` agora, com a imagem nova pronta e conferida, manda
`C_ADMIN_DRAIN` ao servidor atual: todo jogador conectado ouve "o servidor vai reiniciar em N
minutos", nenhuma partida nova começa, e o script espera as partidas em andamento acabarem (ou o
prazo, `-DrainSeconds`, padrão 300; 0 = trocar na hora). `infra\admin.ps1 status|drenar`
também serve à mão. O cliente, por sua vez, DETECTA conexão perdida (antes o laço rodava para
sempre numa nave vazia, sem aviso) e volta ao menu inicial avisando.

**O servidor só precisa de deploy quando o código dele muda** (`src/network/server.nvgt` e
`src/network/server/`, `src/core/game_state.nvgt` e `src/core/game_state/`, protocolo, banco). Som, UI e textos são só cliente.
Quem decide no release é `tools/server_sources.py`: a lista exata do que o servidor compila, seguindo os
`#include` a partir do `server_main.nvgt`. Era uma lista de pastas do cliente para excluir, e um arquivo
de fora dela (o `game_settings.nvgt`, na 0.51.7) reiniciava o servidor à toa.

Para o servidor, a imagem é etiquetada com o **commit** (`git rev-parse --short HEAD`, e não a versão
do jogo - é o que diz exatamente qual código está no ar: `Vps-ServerCommit`, de `infra/vps.ps1`, ou
`docker inspect amongus-server` na VPS) e o `deploy.ps1` **sobe a imagem e confere que o
servidor fica de pé antes de publicar** — ver "compilação que sai defeituosa", abaixo. Todo acesso à
VPS (token, banco, commit no ar) passa por `infra/vps.ps1`, usado pelo deploy, pelo admin e pelo release.

## Ao terminar uma feature

1. **Commite.** O usuário autorizou commits ao fim de cada trabalho concluído. A mensagem deve dizer
   **por que**, não só o quê — o histórico é onde as decisões ficam explicadas.
2. **Registre o que a feature ensinou** (ver "Mantenha este arquivo vivo", no começo). Se apareceu
   uma armadilha, um passo que falha calado ou uma decisão que alguém possa querer desfazer sem
   saber o motivo, isso entra aqui — no mesmo commit.

Uma lição só descoberta e não escrita será paga de novo, inteira, na próxima vez.

## Armadilhas do NVGT (todas custaram caro)

**Caminho relativo resolve pelo diretório do EXECUTÁVEL, não pelo diretório de trabalho.** Um `cd`
antes de rodar não muda nada. Foi isso que fez o servidor tentar criar o banco de contas dentro da
imagem em vez do volume.

**Caminho absoluto precisa de barra normal (`/`).** Com barra invertida,
`file_exists("C:\Windows\System32\cmd.exe")` responde `false` para um arquivo que existe, e `run()`
falha sem dizer por quê.

**`run()` não procura no `PATH`.** `run("powershell.exe", ...)` devolve `false` sempre; é preciso o
caminho completo. Isso deixou a atualização automática quebrada por versões seguidas, caindo no plano
B silenciosamente.

**`run(exe, "linha de argumentos")` quebrou na build do NVGT de 15/09/2026 - use a forma de LISTA.**
A build nova reescreveu `run()` sobre `SDL_CreateProcess` e a forma antiga passou a partir a linha
por espaço em branco, mantendo as aspas dentro dos tokens: `-File "C:/.../x.ps1"` chega ao
PowerShell com aspas literais no nome e ele não acha o script. O pior é o sintoma: `run()` devolve
`true`, o jogo diz "instalando" e fecha, e nunca volta - foi a 0.22.3/0.22.4 no ar com o updater
morto. Sempre `process@ run(array<string> args, flags)` (`src/core/updater.nvgt`), que entrega cada
argumento intacto. Isso prende o projeto à build nova do NVGT (a de dezembro não tem essa forma).

**A ferramenta Bash do assistente corrompe barra invertida dupla em heredocs** (uma string NVGT
escrita como barra-barra-probe vira barra-probe, que é um escape inválido e engole o `p`).
Sonda escrita por heredoc com caminhos Windows dentro produz conclusões falsas - custou uma hora
"investigando" um `run()` que na verdade recebia um caminho errado. Escreva sondas com a ferramenta
de escrita de arquivo, não por heredoc; o código do projeto nunca passou por isso. **Vale para
QUALQUER texto com barra invertida, inclusive um script Python mandado por heredoc** que edita
nota ou doc: `infra\read_feedback` virou `infra` + CR + `ead_feedback` no CLAUDE.md, e
`notes/traducoes.md` carregava dois caminhos assim corrompidos (um `\r` e um `\a`) sem ninguém ver.

**`clipboard_set_text()` devolve `false` mesmo quando COPIOU** (build de 15/09/2026, Windows). Um
`if` no retorno faz o jogo dizer "não consegui copiar" com o texto já na área de transferência.
Confira lendo de volta com `clipboard_get_text()` (ver `src/ui/support_screen.nvgt`).

**Nunca meça o quadro com `timer.elapsed` + `restart()`.** `elapsed` é INTEIRO (ms): o restart joga a
fração fora a cada quadro. No PC, 95% do tempo real chegava ao jogo; no Android, com relógio mais
grosseiro, o personagem levava 70 s num trajeto de 40 (recado #151, sintoma "o jogo é lento/laggado no
celular", embora offline). Use `frame_timer.tick()` (`src/core/frame_timer.nvgt`), que nunca reinicia e
soma a diferença entre leituras. Sonda: `tools/probes/probe_frame_timer.nvgt`.

**`DIRECTORY_TEMP` já termina com barra.** Concatenar outra gera caminhos com `\\` no meio que o
PowerShell tolera e o NVGT não enxerga de volta.

**A build do NVGT de 06/10/2026 vem com a `menu.nvgt` QUEBRADA - corrigida à mão na instalação.**
Sintoma: nada que inclua menu compila, nem o jogo ("No matching symbol 'setup_menu'", em
`c:/nvgt/include/menu.nvgt`, e o `nvgt -c` sai com código 70); na bateria de sondas, as que usam menu
aparecem com `ok=0 falhas=2`. O `choose_number` novo (commit 143ca55 do NVGT) chama um `setup_menu` que
não existe. A linha foi trocada por `menu@ m = menu(); m.intro_text = intro;` em `C:\nvgt\include\menu.nvgt`
(a original ficou ao lado, `.orig_2026-10-06`). **Reinstalou o NVGT? Confira se ainda precisa** - até o
NVGT corrigir, todo reinstalar desfaz o conserto.

**A `menu.nvgt` instalada pode ser mais velha que a documentação.** A daqui **não** suporta a forma
`"som{1...6}.wav"` — ela entrega esse texto direto ao carregador, não acha o arquivo e fica muda, sem
erro. Use lista separada por vírgula, que funciona nas duas versões (ver `sound_variants_list`).

**Declare os `#include` de que o arquivo precisa.** Já houve código compilando por ordem de inclusão,
não por dependência declarada — bastava alguém incluir só aquele arquivo para quebrar.

**`find_files()` NÃO é recursivo.** Com os sons em subpastas, `find_files("sounds/*")` devolve
**zero** arquivos. Isso derrubou de uma vez o gerador do pacote e o conferidor de sons — e o modo
como falhou é o pior: o conferidor passou a acusar que *todos* os sons sumiram, com eles ali do lado.
Para varrer subpasta, use `all_sound_files_on_disk()` (em `src/audio/sound_catalog.nvgt`), que desce
com `find_directories`.

**Em script de `tools/`, não use `chdir("..")` fixo.** O diretório de trabalho depende de como o
script foi invocado, então subir um nível às vezes cai fora do projeto. Os dois utilitários agora
PROCURAM a raiz (`if (!directory_exists("sounds")) chdir("..")`) em vez de supor onde estão.

**Leia cada tecla UMA vez por quadro e guarde o resultado.** `key_pressed()` é consumo de evento, não
consulta de estado: perguntar duas vezes no mesmo quadro pode dar respostas diferentes. Foi assim que
as setas da câmera andavam as duas para o mesmo lado — a condição perguntava `key_pressed(KEY_LEFT)`
e o corpo perguntava de novo para escolher a direção. Guardar em `bool` antes de decidir vale
independentemente da semântica exata.

**`audio_form.is_pressed()` CONSOME o aperto, como o `key_pressed`.** Perguntado duas vezes no mesmo
quadro, o segundo dá false. Sintoma (0.50.1): o botão "Papéis especiais" abria a tela de MODIFICADORES -
um `if (is_pressed(a) || is_pressed(b))` seguido de `bool qual = is_pressed(a)` lia `a` duas vezes. Leia
cada botão uma vez e guarde.

**Cuidado com dois blocos disputando a mesma tecla.** A tecla do radar era lida pelo bloco normal, que
roda ANTES do da câmera e pede a varredura ao servidor; a resposta chegava depois e falava por cima da
que a câmera já tinha dito. Sintoma: a câmera "funciona", mas responde pela sala errada. Quando um
modo novo reaproveita uma tecla, o bloco antigo precisa ser desligado explicitamente nele.

**`KEY_*` é a POSIÇÃO da tecla no teclado americano, não o símbolo impresso nela.** Atalho que depende
do símbolo (`+`, `-`, `=`, pontuação) some em outra disposição: no teclado espanhol o "+" e o "-" ficam
em outras teclas. Sintoma (0.50.6, recado #209): "mais e menos não fazem nada" no Conhecer o mapa - só
para quem joga com teclado de outro idioma, e por isso passou no teste daqui. Para símbolo, leia o
caractere com `get_characters()` (o texto digitado desde a última chamada; ela o zera); letras e teclas
como Tab e setas continuam por `KEY_*`. O mais e o menos do numérico são comandos do NVDA e do JAWS e
muitas vezes nem chegam ao jogo.

**`string.length()` conta BYTES, não letras, e `substr` corta no meio de uma letra.** O texto é UTF-8:
"ação é" tem 6 letras e length 9 (acento vale 2, emoji 4). Um teto escrito com `length()` é, em
português, bem menor do que diz - o chat "de 300" cortava frases de 150 letras -, e o corte deixava um
byte solto que o leitor de tela lê como lixo. Para tamanho ou corte de texto que alguém vai ler, use
`utf8_length`/`utf8_truncate` (`src/core/utf8.nvgt`). E `s[i]` devolve um texto, não um byte: o byte é
`character_to_ascii(s.substr(i, 1))`. Sonda: `probe_text_limits.nvgt`.

**`sound.stream_pcm` BLOQUEIA quando o buffer do stream enche - e para sempre se o som parou.** É o
"o jogo não responde" sem `crash.log`: não é exceção, é o jogo preso numa chamada nativa. Nunca
escreva num stream sem saber quanto ainda cabe (a voz descarta acima de 1 s esperando e refaz o som
que parou - ver [notes/voz.md](notes/voz.md)). Sonda: `tools/probes/probe_stream_block.nvgt`.

**Global se inicializa na ordem do ARQUIVO - uma constante declarada depois da global que a usa está
vazia.** Sintoma: o jogo fecha ao abrir (código 65; do fonte, "Failed to initialize global variable
'g_settings'" com "Null pointer access" no construtor). Compila limpo e passa em toda sonda que não
abre o cliente inteiro - foi a 0.45.0 no ar, e quem atualizou ficou sem updater para sair dela. Constante
que um construtor de global lê vem ANTES da global. O `build_clients.ps1` agora abre o exe compilado
e recusa o build se ele fechar sozinho.

**`audio_form` tem teto de 50 controles - e o que passa disso não é criado, em silêncio.** O
`create_*` devolve -1 e a tela segue sem os campos do fim. Sintoma (0.50.0, com os papéis novos no
formulário de sala): "só consigo mexer nos campos, nunca criar a sala" - o botão de confirmar era o
51º, não existia, e o Enter não tinha o que apertar. Papéis e modificadores foram para telas próprias,
abertas por botão (`run_role_choice_form`), e os formulários de sala registram no `crash.log` se
ficarem sem o confirmar. Formulário que cresce com uma lista (papéis, idiomas): conte. Sonda:
`probe_lobby_form.nvgt`, que aperta Enter de dentro do laço da tela.

**Desenhar na janela: use `window.renderer`, nunca o `graphics_renderer()` solto.** O NVGT tem gráficos
(SDL3: imagens, fontes, retângulos, linhas, texturas - `src/graphics.cpp` do NVGT, quase sem
documentação), e o construtor solto só se prende à janela que tem o FOCO do teclado. Sintoma: o
renderizador vem inválido e nada aparece, sem erro - sempre que o jogo abre sem foco (aberto por outro
programa, ou o Windows segurando o foco). A janela devolvida por `show_window` tem o dela, sempre válido.
**E NÃO chame `present()` em quadro que se redesenha a cada volta: todo `wait()` já apresenta**
(`refresh_window()` do NVGT, sem condição). Apresentando também, cada quadro saía duas vezes e a segunda
mostrava o outro buffer da placa, com um quadro antigo. Sintoma (0.51.3, relato do usuário): andando,
"as telas anteriores ficam embaixo, meio duplicado". Tela desenhada UMA vez e deixada parada é o caso
inverso: os `wait()` seguintes alternam entre os dois buffers, então ela é desenhada duas vezes com um
`present()` no meio, para os dois ficarem iguais (`present_static` em `visual_map.nvgt`). Captura de tela
não pega essa alternância (ela dura uma fração de quadro) - a prova é o código do NVGT.
Para conferir um desenho sem ver, capture a janela (PowerShell + `CopyFromScreen`) e leia a imagem. É o
que faz o mapa na tela (`src/ui/visual_map.nvgt`).

**Uma compilação do NVGT pode sair defeituosa.** Aconteceu: mesmo código-fonte, um build gerou binário
com segfault na inicialização e o seguinte saiu bom. Compilar com sucesso **não** é o mesmo que o
binário funcionar. Por isso o `deploy.ps1` testa a imagem antes de publicar; se algo assim aparecer de
novo, **recompile antes de investigar o código**.

## Padrões do projeto

**Comentário explica o porquê, não o quê.** O código já diz o que faz; os comentários existem para a
decisão, a alternativa descartada e o problema que aquilo evita. É o padrão em todo o projeto —
mantenha.

**Classe grande se divide por assunto com `mixin class`, não com classe nova.** O AngelScript não tem
classe parcial; o `mixin class nome_part { ... }` num arquivo à parte, mais `class x : nome_part`, põe
os métodos de volta na mesma classe - eles enxergam os campos e métodos de todas as partes, privados
inclusive. É assim que moram o `game_server` (`src/network/server/`) e o `game_state`
(`src/core/game_state/`): os campos e o laço no arquivo principal, um arquivo por assunto. Método novo
vai na parte do assunto dele; campo novo, no arquivo principal. Funções soltas se dividem só movendo
para outro arquivo incluído - mantendo a ORDEM das globais (ver a armadilha das globais, acima): as
telas de sala estão em `src/ui/lobby/`, incluídas pelo `lobby_screens.nvgt` nessa ordem.

**O laço da partida do cliente é a classe `match_session`** (`src/game/game_loop.nvgt`): o estado da
partida são os campos (o porquê de cada um, no construtor), `frame()` é um quadro - uma sequência de
chamadas, na ordem em que as coisas têm que acontecer - e cada mensagem do servidor tem o seu
`on_<mensagem>` em `src/game/match/match_net_*.nvgt`, chamado por `handle_packet`. Mensagem nova: um
`on_` no arquivo do assunto e uma linha no `handle_packet`. Tecla nova: no trecho de teclas certo de
`match_frame_keys.nvgt`. E a sonda `probe_match_packets.nvgt` entrega mensagens a uma partida de
mentira, sem servidor nem teclado, e confere o estado - o caso novo entra lá.

**Mudança que só move código se prova por comparação:** `tools\run_probes.ps1 -Saida antes.txt` antes,
de novo depois, e as duas listas têm que bater (`-Raiz` roda numa cópia - um `git worktree` - para não
travar a pasta de trabalho durante os ~40 minutos da rodada).

**Todo menu é `screen_menu`, nunca `menu`** (`src/ui/visual_map.nvgt`). É o `menu` do NVGT com uma
diferença só: a cada `monitor()` ele desenha a lista na tela para quem enxerga, se a opção estiver
ligada. Um `menu` puro funciona igual pelo som, mas fica em branco na tela - e nada avisa.

**Valor vindo do cliente é validado no servidor.** As configurações de sala passam por
`lobby_config.validate()` depois de aplicadas: elas vêm da máquina do jogador.

**Estado que existe nos dois lados tem que ser DESFEITO nos dois lados.** Um estado desfeito só de um
lado deixa o jogador preso sem mensagem nenhuma — aconteceu com a câmera de segurança e, por outra
porta, com o duto. Quando o sintoma reaparecer por uma terceira, o lugar de consertar continua sendo
a origem, não o sintoma. Os dois casos estão em [notes/rede-e-telas.md](notes/rede-e-telas.md) e
[notes/ciclo-da-partida.md](notes/ciclo-da-partida.md).

**Regra que os DOIS lados aplicam mora numa função só.** Escritas separadas, elas divergem e vence a
mais restritiva — e a correção feita de um lado parece não surtir efeito nenhum, porque quem recusa é
o outro. Foi assim com a voz na reunião e com o radar do fantasma, que sobreviveu a três tentativas
de conserto no cliente enquanto o servidor era quem dizia não.

O resto — som, papéis, mapa, ciclo da partida, rede, telas, voz — está em `notes/`, um arquivo por
área. A tabela no topo diz qual ler.

## Testar

**Minigame (task ou reparo) se testa no menu "Praticar tarefas e reparos"** do próprio jogo, sem
servidor: `src/ui/practice_screen.nvgt` roda o mesmo `run_task_minigame`/`run_sabotage_panel` da
partida com um `game_client` criado mas não conectado (`make_offline_client`). Serve para o jogador
como modo de treino e para quem desenvolve como bancada - antes, testar uma task era montar uma
partida de três.

Há um servidor de verdade no ar — **use-o**. O padrão que funcionou a sessão inteira: escrever um
`.nvgt` curto que conecta, faz a coisa e imprime o resultado, rodar com `nvgt arquivo.nvgt`, apagar
depois. Foi assim que se validou feedback, i18n, configurações de sala e limite de jogadores.

As armadilhas de escrever sonda - todas descobertas do jeito caro - estão em
[notes/sondas.md](notes/sondas.md). Leia antes de escrever uma: sonda que passa pelo motivo errado é
pior do que sonda nenhuma.

Para coisas que só falham no build compilado (o menu de sons, a atualização, caminhos), compile uma
sonda com `nvgt -c`, rode o `.exe` e grave o resultado num arquivo — o app compilado não tem console.

**Não confie em "compilou".** Compilar não prova que o som toca, que o pacote tem o arquivo novo, nem
que o binário sobe.

## Pendências conhecidas

- **Recados do beta:** a caixa atual é o `infra\admin.ps1 recados`, e não esta lista — uma lista
  de recados copiada aqui envelhece e passa a ser lida como pendência depois de atendida (foi o que
  aconteceu: ela listava como abertos pedidos já resolvidos). Aqui só entra o que foi DECIDIDO e
  ainda não foi feito.
- **Sons sem uso, de propósito** (o `check_sounds` os lista a cada execução; não são lixo):
  `steps/SnowTile*` é piso que o mapa ainda não tem - quando houver uma sala de neve, ele entra em
  `FOOTSTEP_FLOOR_PREFIXES` e na tabela de variantes
  (ver "piso novo entra em DUAS tabelas"). É a única sobra hoje - o `nearbeep.wav` virou o
  `ui/target_in_range.ogg` e está em uso.
- **O som por COR nunca vai ser encontrado no jogo compilado.** `color_death_sound_path` e
  `color_kill_sound_path` (`config/colors.nvgt`) escolhem o arquivo específico da cor com
  `file_exists("sounds/colors/...")` - e no build distribuído os sons vivem dentro do `sounds.dat`,
  onde `file_exists` responde false. Hoje não vaza nada porque nenhum desses arquivos existe e o
  fallback genérico é o certo; no dia em que alguém gravar um som de kill por cor, ele vai funcionar
  rodando do fonte e ficar mudo no jogo dos jogadores. O conserto é perguntar ao pacote
  (`sound_default_pack`), não ao sistema de arquivos. Achado ao portar para o Android, que tem
  exatamente a mesma discordância entre "existe" e "dá para ler".
