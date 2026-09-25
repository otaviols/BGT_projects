# Among Us Audiogame — guia do projeto

Jogo de dedução social **jogado inteiramente por som**, escrito em [NVGT](https://nvgt.gg)
(AngelScript). Cliente Windows distribuído por um site estático; servidor dedicado rodando como
contêiner num cluster AKS. Está em **beta**, com jogadores reais usando.

Tudo que o jogador percebe passa por leitor de tela e áudio posicionado — **não existe informação
visual**. Ao decidir qualquer coisa de interface, a pergunta certa é "como isso soa?", não "como isso
aparece".

**Onde procurar aqui.** Este arquivo é longo porque cada parágrafo custou horas a alguém. Não é para
ser lido inteiro toda vez — é para ser consultado pela seção que interessa:

| Vou mexer em… | Vá para |
|---|---|
| qualquer coisa, antes de começar | [Estrutura](#estrutura), [Compilar](#compilar) |
| som novo, volume, arquivo de áudio | [Som](#som-o-que-toca-quando-e-para-quem), [Arquivos de som](#arquivos-de-som), [Sinais sonoros](#sinais-sonoros-e-preferências-de-audição) |
| papel/profissão, habilidade | [Papéis](#papéis-profissões) |
| tela, menu, formulário, fila de pacotes | [Telas e laço do cliente](#telas-laço-do-cliente-e-fila-de-pacotes) |
| protocolo, login, sessão | [Rede, protocolo e sessão](#rede-protocolo-e-sessão) |
| começo/fim de partida, reunião | [Ciclo da partida](#ciclo-da-partida) |
| mapa, sala, tarefa | [Mapa e tarefas](#mapa-e-tarefas) |
| chat de voz | [Chat de voz](#chat-de-voz) |
| escrever uma sonda | [Testar](#testar) e [o que as sondas ensinaram](#o-que-as-sondas-ensinaram-custou-caro-descobrir) |
| publicar, servidor, Docker | [Publicar uma versão](#publicar-uma-versão), [Deploy](#deploy-infraestrutura-e-ferramentas) |

O resto das **Armadilhas do NVGT** vale para tudo: são coisas da linguagem e da engine que falham em
silêncio, e é onde procurar quando algo "compilou e não funcionou".

## Mantenha este arquivo vivo

**Aprendeu algo que teria economizado tempo se estivesse escrito aqui? Escreva aqui, na mesma
sessão.** Isto não é opcional nem "se sobrar tempo": é parte de terminar o trabalho, junto com o
commit. Cada armadilha listada abaixo custou horas a alguém; o que não for registrado será
redescoberto do mesmo jeito caro.

Vale registrar:

- **Armadilha** que fez algo falhar de forma enganosa — sempre com o **sintoma**, não só a causa. O
  sintoma é o que se vê primeiro, e é por ele que a pessoa vai procurar.
- **Decisão** de projeto e a alternativa descartada, quando alguém possa querer desfazê-la sem saber
  o que ela evitava.
- **Passo que se esquece** e falha em silêncio.
- **Pendência conhecida**, para não ser redescoberta como se fosse bug novo.

E, com o mesmo peso: **se algo aqui deixou de ser verdade, corrija na hora**. Este arquivo é lido no
começo de toda sessão e tratado como verdade — uma instrução errada aqui é pior do que instrução
nenhuma, porque leva a decisões erradas com confiança. Ao mudar caminho de arquivo, comando de
deploy, nome de recurso ou padrão, confira se este documento ainda descreve a realidade.

Antes de afirmar algo aqui, **verifique contra o código**, não contra a memória da conversa.

## Estrutura

| Caminho | O que é |
|---|---|
| `AmongUs.nvgt` | ponto de entrada do cliente |
| `server_main.nvgt` | ponto de entrada do servidor |
| `src/` | **todo** o código: `config/`, `core/`, `game/`, `network/`, `ui/`, `database/`, `audio/`, `i18n.nvgt` |
| `lang/` | **só dados** de tradução (`pt_BR.json`, `en_US.json`) — o motor de i18n fica em `src/` |
| `sounds/` | áudio fonte; vira `sounds.dat` no build |
| `tools/` | `build_pack` (gera o `sounds.dat`), `check_sounds`, `bots`, `build_clients.ps1`, `sync_translations.ps1`, `check_translation.py`, ferramentas de administração, `probes/` (sondas que conversam com um servidor de verdade) |
| `docs/` | manuais e histórico de versões, distribuídos com o jogo numa pasta `docs/` |
| `infra/` | Terraform, Dockerfile, manifests do Kubernetes, `deploy.ps1`, `read_feedback.ps1` |

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

**O build "trava por dez minutos" = a pergunta de instalar no aparelho.** `build.android_install`
vale 1 por padrão, que significa "perguntar", e a pergunta é um diálogo esperando resposta que nada
anuncia. O script passa `0`.

**O identificador `com.otaviols.amongusaudiogame` se escolhe UMA vez.** No Android ele é o caminho da
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

**O que ainda não foi feito:** ninguém rodou o APK num aparelho. E o toque não existe — esta versão
espera **teclado** (Bluetooth ou USB), que é o combinado: gestos vêm depois, e o `touch_keyboard_interface`
do NVGT mapeia gesto para tecla simulada, então menus, formulários e minigames não precisam de uma
segunda interface. O que precisa de código de verdade é **andar**, que lê tecla SEGURADA
(`action_down`) e nenhum gesto expressa isso — é um manche virtual por `on_hold`. E `monitor()` teria
que ser chamado em todo laço bloqueante, com a mesma disciplina (e a mesma falha silenciosa) do
`client.update()`.

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
4. Compile o que mudou.
5. `infra\deploy.ps1 -StorageAccount amongusaudiogame` (use `-SkipServer` quando só o cliente mudou).

As duas versões **têm que bater**. O `version.json` é o que os clientes instalados comparam contra si
mesmos: se ele ficar para trás, ninguém é avisado da atualização.

**Deploy do servidor derruba quem está jogando - por isso ele AVISA antes.** O estado das partidas
vive na memória do processo; trocar o pod no meio de uma partida derrubou todo mundo sem aviso (e
foi assim que se descobriu). O `deploy.ps1` agora, com a imagem nova pronta e conferida, manda
`C_ADMIN_DRAIN` ao servidor atual: todo jogador conectado ouve "o servidor vai reiniciar em N
minutos", nenhuma partida nova começa, e o script espera as partidas em andamento acabarem (ou o
prazo, `-DrainSeconds`, padrão 300; 0 = trocar na hora). `tools/server_admin.nvgt status|drain`
também serve à mão. O cliente, por sua vez, DETECTA conexão perdida (antes o laço rodava para
sempre numa nave vazia, sem aviso) e volta ao menu inicial avisando.

**O servidor só precisa de deploy quando o código dele muda** (`src/network/server.nvgt`,
`src/core/game_state.nvgt`, protocolo, banco). Som, UI e textos são só cliente.

Para o servidor, a imagem é etiquetada `v<versão>` e o `deploy.ps1` **sobe a imagem e confere que o
servidor fica de pé antes de publicar** — ver "compilação que sai defeituosa", abaixo.

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
de escrita de arquivo, não por heredoc; o código do projeto nunca passou por isso.

**`DIRECTORY_TEMP` já termina com barra.** Concatenar outra gera caminhos com `\\` no meio que o
PowerShell tolera e o NVGT não enxerga de volta.

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

**Cuidado com dois blocos disputando a mesma tecla.** A tecla do radar era lida pelo bloco normal, que
roda ANTES do da câmera e pede a varredura ao servidor; a resposta chegava depois e falava por cima da
que a câmera já tinha dito. Sintoma: a câmera "funciona", mas responde pela sala errada. Quando um
modo novo reaproveita uma tecla, o bloco antigo precisa ser desligado explicitamente nele.

**Uma compilação do NVGT pode sair defeituosa.** Aconteceu: mesmo código-fonte, um build gerou binário
com segfault na inicialização e o seguinte saiu bom. Compilar com sucesso **não** é o mesmo que o
binário funcionar. Por isso o `deploy.ps1` testa a imagem antes de publicar; se algo assim aparecer de
novo, **recompile antes de investigar o código**.

## Padrões do projeto

**Comentário explica o porquê, não o quê.** O código já diz o que faz; os comentários existem para a
decisão, a alternativa descartada e o problema que aquilo evita. É o padrão em todo o projeto —
mantenha.

### Tradução e identidade

**Texto do servidor viaja como chave de tradução, nunca como frase pronta.** Os jogadores de uma
partida podem estar em idiomas diferentes, e quem sabe o idioma de cada um é o cliente dele. Ver
`MSG_KEY_FIELD` e `tr_server_message` em `src/network/protocol.nvgt`.

**Idiomas são plugáveis.** Basta pôr um `.json` em `lang/` para o idioma aparecer no jogo; o nome dele
sai da chave `language.name`, no próprio idioma. O que faltar cai no inglês. **Ao acrescentar
qualquer texto novo, acrescente a chave nos dois idiomas.**

**Nada de identidade é traduzido** — nome de jogador, nome de bot. Traduzir faria duas pessoas na
mesma partida acusarem "nomes" diferentes pela mesma pessoa.

### Quem decide o quê: servidor e cliente

**Valor vindo do cliente é validado no servidor.** As configurações de sala passam por
`lobby_config.validate()` depois de aplicadas: elas vêm da máquina do jogador.

**Estado que existe nos dois lados tem que ser DESFEITO nos dois lados.** O servidor derrubava o posto
de câmeras (sabotagem, reunião, morte) e avisava a partida inteira qual sala estava sob observação -
menos a única pessoa que precisava saber, a que estava observando. O cliente dela continuava na
câmera: parado, ouvindo outra sala, apertando ESC sem efeito porque para o servidor ele já tinha
saído. Sintoma: jogador travado sem mensagem nenhuma. Por isso `release_cameras_any()` devolve QUEM
foi liberado, e todo lugar que fecha o posto passa por `close_cameras()`.

### Som: o que toca, quando e para quem

**Caminho de arquivo montado à mão é bomba-relógio.** Depois que os sons foram para subpastas, a task
de rever a gravação continuou pedindo `sounds/Tile3.wav` - arquivo que não existe mais - e rodava
MUDA: o NVGT não reclama de som que não existe, e o `check_sounds` não pega, porque só compara o
catálogo com o disco e esse caminho era construído dentro da task. Todo caminho de som sai de uma
função do catálogo (`footstep_sound_path()`, por exemplo), nunca de concatenação local.

**Quem percebe QUEM se decide no servidor, não no cliente.** O cliente recebe a posição de todo mundo
(é o que faz o áudio posicionado funcionar), então é tentador montar "quem está nesta sala" ali mesmo
- e a câmera de segurança nasceu assim. Só que o cliente não sabe quem está dentro de um duto: isso é
privado de quem entrou, e tem que continuar sendo. A câmera então contava o impostor escondido,
entregando a dedução que o duto existe para negar; mandar o estado de duto para todos os clientes
consertaria a lista criando um vazamento pior. A regra mora em `game_state.players_in_room()`, e o
radar e a câmera são duas perguntas para a MESMA função.

**Regra de percepção fica em `src/core/sabotage_rules.nvgt`, nunca espalhada.** Quem é atrapalhado
por qual sabotagem se decide num lugar só. Espalhada como `if` em cada ponto (marcadores, radar, som
de corpo, alcance de audição), bastava esquecer um para o jogador ficar cego pela metade sem que nada
denunciasse. É também onde papéis novos vão declarar o que enxergam.

**`audio_form` nasce SEM foco em controle nenhum** (`control_focus = -1` em `form.nvgt`) — chame
`f.focus(<primeiro campo>)` depois de criar os controles, em todo form. Sintoma: o jogador abre a
tela, digita, aperta Enter e nada acontece; funciona depois que ele aperta Tab. Foi assim com a caixa
de mensagem do chat, o usuário no primeiro login, o nome da partida e os volumes.

**Espera que consome um pacote não pode jogá-lo fora se ele for o único.** As esperas por resposta do
servidor tiram pacotes da fila com `poll_message()`, e o que não for o pacote esperado **some**. A
confirmação da criação da sala é o próprio `S_LOBBY_STATE` - o único retrato que o anfitrião sozinho
recebe, já que o próximo só vem quando alguém entra. Descartá-lo deixava P e C mudos até chegar a
segunda pessoa. Quem espera devolve o pacote a quem chamou.

**Não espere um som acabar com `wait` de duração fixa.** O número é um chute da duração do arquivo, e
quem trocar o som depois não tem como saber que havia um `wait` casado com ele. Falha em silêncio:
dois sons por cima um do outro, sem erro. A fita da sala de segurança dura 3457 ms e o código
esperava 1200 — os primeiros passos, que a task pede para CONTAR, tocavam por cima do chiado. Use
`wait_for_task_sound()` (em `src/game/tasks/task_common.nvgt`), que espera `sound.playing` virar
false sem parar de bombear `client.update()`. Som que não existe devolve handle nulo, e a espera
passa direto.

**`play_wait()` e `wait(n)` seco não têm lugar dentro de uma minigame.** `play_wait` prende a
thread inteira até o som acabar: nada de rede, nada de ESC, e - o sintoma que chegou no recado #97
como "o jogo não responde na tarefa dos coletores" - nenhuma tecla é vista. Quem decorou a
sequência digita rápido, e a tecla apertada durante o eco da anterior simplesmente sumia; o
jogador ficava esperando uma resposta que não vinha. Som que precisa acabar antes do próximo
passo: `wait_for_task_sound(task_sound(..., pitch), client)`; pausa: `task_wait(ms, client)`; eco
de tecla: toca e NÃO espera. Medido em `tools/probes/probe_play_wait.nvgt`: o `play_wait` em si
volta direito (504 ms no tom grave), o problema é o que não acontece enquanto ele não volta.

**Task visível (o scan) é prova porque o SERVIDOR a anuncia, não o cliente.** O cliente manda
`C_START_TASK` ao abrir qualquer minigame; só para o scan o servidor confere que aquele jogador tem a
task pendente e está vivo, e então manda `S_TASK_VISIBLE` a todos menos a ele. Impostor não tem a
task, então nada é anunciado por mais que o cliente dele finja - é isso que faz o som do scanner na
enfermaria valer como álibi. O fim vem pelo `C_TASK_INPUT` (sucesso ou cancelamento), e reunião,
morte e saída também encerram (`end_visible_task`), senão o scanner soaria numa sala vazia.

**"Task concluída" é UM som, tocado num lugar só.** `SND_TASK_COMPLETE` sai do `task_manager` ao
fechar a task com sucesso (e de `run_sabotage_panel` para reparos). Nenhuma task toca um som de
"acabei" por conta própria: `BeepComplet` e `CardComplet` são feedback de ETAPA (lixo puxado, cartão
aceito) e só entram onde a etapa existe. Antes cada task tocava um som diferente no fim, por cima do
som certo.

**Cada som tem UM ouvinte pretendido — decida quem antes de tocar.** Num jogo em que a informação é o
som, tocar para todo mundo entrega de graça o que devia custar. O assassinato são dois sons com
públicos diferentes: a **morte** é pessoal e só a vítima ouve; o **kill** é espacial e é o único
sinal do crime para quem está por perto. Tocar os dois para todos, como era antes, dava o crime de
graça aos vizinhos e fazia a vítima ouvir a própria morte como se fosse de outra pessoa.

**Fantasma não é atingido por sabotagem.** Ele já perdeu o que tinha a perder, e continua fazendo
tarefas pelo time — cegá-lo não cria tensão, só torna tedioso o que ainda ajuda.

### Papéis (profissões)

**O papel do jogador tem UMA fonte, e `is_impostor` é derivado dela.** `game_player.role_id` é o
que vale; `set_role()` recalcula `is_impostor` a partir do time do papel. Os dois nunca são
escritos separados - é o que impede um papel de impostor (shapeshifter) de contar como tripulante
na vitória, ou um papel de tripulação de virar alvo proibido de kill. Nada deve atribuir
`is_impostor` direto; quem fizer isso deixa os dois discordando sem que nada acuse (a sonda
`probe_ghost_sabotage` já foi corrigida por causa disso). A sala guarda os papéis em
`lobby_config.role_counts` (id -> quantos, **vazio por padrão em todo preset**) e
`role_chances` (chance de CADA vaga sair, 100 por padrão), o sorteio inteiro mora em
`draw_special_roles` - chamada pelo servidor E pela sonda, porque reimplementá-la na sonda passaria
com o servidor quebrado -, e a habilidade ativa é um pacote só
(`C_USE_ABILITY` -> `run_ability`, um caso por id) em vez de um pacote por papel. Sala com papel
ligado exige `ROLES_MIN_PROTOCOL` e recusa cliente velho na PORTA (entrar) e no INÍCIO (quem já
estava dentro), dizendo quem precisa atualizar. Sonda: `tools/probes/probe_roles.nvgt`.

**Número de papel que a sala ajusta: o PAPEL declara, e o servidor pergunta.** `role_traits.tunables`
lista recarga, duração e janela de gravação com o padrão e os LIMITES (que moram no papel porque só
ele sabe o que é absurdo no caso dele - recarga do engenheiro abaixo da recarga da sabotagem faz
sabotar deixar de ser jogada). A sala guarda em `role_values` **só o que foi mexido**, então mudar
um padrão no código vale para todas as salas antigas em vez de ficar congelado na criação. Nada
disso toca o protocolo: um papel novo com um número novo custa uma linha na declaração e uma chave
de texto - o formulário, a validação, a leitura das regras e o `to_json` percorrem a lista.

O servidor lê por `config.role_value(papel, chave)`, **nunca a constante**; a constante virou só o
padrão. E o cliente recebe a duração de volta no `S_ABILITY_RESULT` (`duration_seconds`), porque ele
acompanha a fantasia e a invisibilidade localmente e não conhece a configuração da sala.

**Preset de sala salvo = `lobby_overrides` + o preset-base, no MESMO formato do protocolo.** Os
ajustes são serializados por `write_to_packet`/`read_from_packet` (`config/lobby_presets.nvgt`), e
não por uma serialização própria: assim um campo novo na sala entra no preset sozinho, sem uma
segunda lista para esquecer de atualizar. Ficam na máquina do jogador (`game_settings.lobby_presets`,
junto das teclas e volumes) - não dependem de conta nem de banco, e o preço assumido é que trocar de
computador os perde. **Nome da sala e código de acesso não entram**: o nome se quer variar, e um
código salvo seria um código que nunca muda. A última usada é gravada sozinha sob o nome VAZIO, que
o formulário recusa - por isso ela convive com as nomeadas sem poder ser sobrescrita. E a leitura
fica em try/catch: o arquivo é do jogador, pode estar truncado, e `parse_json` LANÇA. Sonda:
`tools/probes/probe_presets.nvgt` (a ida e volta é o que falha calado - um campo que não sobrevive
vira uma sala com regra diferente da escolhida, sem nada acusando).

**A sala guarda a INTENÇÃO; quem lida com a realidade é o sorteio.** `validate()` NÃO recorta as
vagas por time nem por quanta gente veio - só pelo absurdo (mais vagas do que a lotação). Recortar
ali reescrevia em silêncio o que o anfitrião digitou, e de um jeito que não voltava: baixar a
lotação apagava os papéis, e subi-la de volta não os trazia. Quem sabe quantos vieram é
`draw_special_roles`, que já deixa de fora o que não couber.

**Papel ligado NÃO é papel garantido, e isso é de propósito.** Cada vaga rola a própria chance
(`role_chance`, 100 = sempre), como no original: com chance abaixo de 100 a tripulação não pode
assumir que o xerife existe, e é essa dúvida que faz o papel valer. Três consequências que
precisam continuar verdadeiras juntas: o `S_GAME_START` anuncia o que a sala **configurou**, nunca
o que saiu (anunciar o resultado entrega de graça a dedução que a chance cria); uma vaga sorteada
para fora **não gasta jogador**, senão "2 xerifes a 50%" tiraria duas pessoas do sorteio dos
outros papéis sem xerife nenhum aparecer; e quando não há gente para todos os papéis (sala pequena,
ou cheia de bots, que nunca recebem papel especial) o que não couber simplesmente não sai, calado -
do lado de quem joga isso é indistinguível de uma chance que não saiu, e é isso que o torna
seguro.

**Engenheiro: o duto dele custa uma dedução, e isso foi pago de propósito.** "A tampa abriu na
navegação, então ele saiu em armas ou no reator" deixa de provar impostor. O som da tampa continua
IGUAL para os dois - fazer um som diferente devolveria a certeza e mataria o papel. O conserto
remoto (`ABILITY_REMOTE_FIX`) acaba com a sabotagem inteira de qualquer lugar, com recarga de
120 s (`ENGINEER_FIX_COOLDOWN_SECONDS`, maior que a recarga da sabotagem de propósito: consertar
toda sabotagem que aparece transformaria a habilidade num botão de desfazer). Ele sai pelo MESMO
`S_SABOTAGE_FIXED` do conserto normal - o resto do jogo não precisa saber que há um segundo jeito
de a sabotagem acabar - e **sem dizer quem consertou**: o sinal é a sabotagem terminar sem ninguém
ter chegado ao painel. Sonda: `tools/probes/probe_engineer.nvgt`.

**Xerife: quem morre no tiro é DECIDIDO, não presumido - e o erro é segredo dele.** `try_kill`
escolhe a vítima (atirador da tripulação + alvo que não é impostor = morre o atirador), então todo
lugar que tratava "o alvo morreu" passou a ler o `peer_id` do PACOTE - foi assim que o `on_kill`
soltava a câmera da pessoa errada. O `misfire` **não vai no `S_PLAYER_KILLED`**: ele é transmitido
à partida inteira, e o campo entregaria de graça que houve um xerife e que ele errou. O aviso vai
em particular, por `S_ERROR`, só para quem atirou; para todos os outros - inclusive para quem ele
mirou, que nem sabe que foi mirado - aquilo é um assassinato qualquer. E `you_were_killed_text`
trata o caso de o assassino ser a própria vítima, senão a fala é "você foi eliminado por <seu
nome>". Sonda: `tools/probes/probe_sheriff.nvgt`.

**Alarmista: o aviso sai da CAPACIDADE (`announces_own_death`) e depois do pacote da morte.** A
ordem importa - invertida, o texto contaria o que aconteceu antes de o som do assassinato tocar. O
`S_DEATH_ALARM` diz nome e SALA, nunca quem matou nem a posição exata: quem quiser mais que isso
tem que ir até lá. E ele entra em `background_event_text`, senão quem estivesse numa task - que é
exatamente onde a pessoa está quando o impostor escolhe matar longe de todos - não ouviria o único
aviso que o papel existe para dar. Sonda: `tools/probes/probe_noisemaker.nvgt`.

**Anjo da guarda: o escudo é conferido no TOPO do `try_kill`, e o lugar dele ali é a regra.** Antes
de consumir a recarga do atacante, porque gastá-la seria um aviso indireto ("apertei, não saiu som
de kill e meu cooldown zerou" só pode significar escudo); e antes do cálculo do tiro errado do
xerife, o que faz o escudo valer contra QUALQUER ataque - uma regra só, sem exceção para explicar.
Nada é transmitido ao lançar: nem ao protegido, nem à partida. O anjo ouve só o nome de quem
protegeu, e **nunca** fica sabendo se o escudo serviu de algo - saber que ele barrou um ataque
seria saber que havia um impostor ali, e fantasma não pode ter essa informação (ele conversa com
outros fantasmas por voz). `ability_usable_alive`/`ability_usable_dead` são DOIS campos porque há
três casos: só vivo, vivo e morto, e só morto - este. Sonda: `tools/probes/probe_guardian.nvgt`.

**Detetive: o rastro é frio, sai em SALAS, e a janela veio da geometria do mapa.** Ele grava por
onde o assassino passou nos `KILLER_TRAIL_SECONDS` seguintes à morte e para - é testemunho (uma
frase que ainda vale na reunião), não caçada (uma direção ao vivo, que apodrece em dez segundos e
faria o detetive perseguir sozinho). **Corredor não entra**: o rastro é para ser DITO, e
`room_id_at` devolve corredor também, então o filtro é explícito (`zone.is_corridor`). **Dentro do
duto nada é gravado**, pela mesma regra que esconde quem está ventilado do radar e da câmera - e
isso faz de ventilar a contra-jogada natural contra o papel. O `killer_peer_id` fica no corpo só
para gravar e **nunca** sai dali.

Os 25 segundos saíram da GEOMETRIA: da borda da cafeteria à borda da armaria são 22 unidades,
exatamente dez segundos a `PLAYER_MOVE_SPEED`. Com a janela de dez que eu tinha posto, o rastro
saía vazio em quase toda morte e o papel nascia morto - a sonda pegou de primeira. **Mexeu no
tamanho do mapa, revise `KILLER_TRAIL_SECONDS`.**

**Lista traduzível vem como CHAVES separadas por "|", e quem junta é o cliente.** O
`tr_server_message` substitui parâmetro como texto cru, então uma lista de salas chegaria como
`room.electrical|room.storage` dentro da frase. Ver `examine_result_text` em `event_speech.nvgt`.

**O ELENCO do cliente é a fonte de todo nome que ele fala sozinho** - quem está por perto, quem
votou, quem está falando no chat de voz. Ele é carregado uma vez, no `S_GAME_START`, e por isso o
disfarce do metamorfo tinha um buraco que só um jogador achou: **chegar perto dele anunciava o nome
verdadeiro**, justamente no momento em que o disfarce mais importa. Hoje o servidor transmite
`S_PLAYER_RENAMED` ao transformar, ao expirar e na reunião, e o cliente renomeia a entrada - o que
fecha todos esses caminhos de uma vez, inclusive os que alguém acrescentar depois. Quem for
acrescentar um lugar que fale nome: use o elenco, não invente outra fonte.

**Metamorfo: todo nome que chega a outro jogador passa por `display_name()`.** Num jogo sem imagem,
"parecer com alguém" é aparecer com o nome dele, e o nome só sai do SERVIDOR - radar, câmera, quem
está na sala. Um ponto esquecido é um buraco no disfarce, sem nada acusando no código.

**A vítima é a exceção, e é deliberada: ela ouve o nome VERDADEIRO.** Desde que o elenco passou a
carregar o nome disfarçado, isso exige uma CÓPIA privada: o `S_PLAYER_KILLED` vai à partida sem nome
nenhum, e só a vítima recebe uma versão com `killer_real_name`. Quem morreu já pagou o preço e não tem como contar a
ninguém (não vota, não fala com vivo, e fantasma só é ouvido por fantasma), então mentir para ele
não compra nada ao impostor e só piora o jogo de quem já perdeu - e no caso normal a vítima também
sabe quem a matou, então disfarçar aqui tornaria morrer para o metamorfo pior do que morrer para um
impostor comum. Eu tinha feito o contrário, e o usuário corrigiu. Cuidado ao mexer: o nome
verdadeiro **não pode** entrar no `S_PLAYER_KILLED`, que é transmitido à partida inteira - seria o
mesmo erro do `misfire` do xerife.

**Pendência do disfarce: ele cobre o NOME, não a cor.** Hoje isso não vaza porque todas as cores
usam o mesmo som de kill (os sons por cor têm fallback genérico e nenhum foi criado). No dia em que
existir som de kill por cor, quem estiver perto vai ouvir a cor REAL do metamorfo - e o disfarce
precisa passar a copiar a cor junto (ver `killer_color` no `S_PLAYER_KILLED`).

Três coisas seguram o papel: o SOM da transformação é posicionado e **não leva nome nenhum** (quem
ouve sabe que algo aconteceu ali, não quem nem em quem); a **voz cala** enquanto durar (ela chega
identificada pelo peer, e uma frase o entregaria - a decisão fica na linha do `g_voice.allowed`,
recalculada por quadro, e não numa transição, senão o quadro seguinte a desfaz); e a **reunião
desfaz a fantasia**, porque lá se vota por NOME e dois nomes iguais na lista não são blefe, são
uma tela quebrada. Sonda: `tools/probes/probe_shapeshifter.nvgt`.

**Fantasma: a invisibilidade reaproveita a semântica do DUTO, nos mesmos três pontos.** "Presente
mas imperceptível" já existia, e pendurar `invisible()` ao lado de `vented` em `players_in_room`
(radar E câmera de uma vez), `on_move` (a posição é o que os outros clientes viram PASSOS - sem
isto ele seria invisível no radar e continuaria pisando alto) e `on_voice_frame` cobre tudo sem
inventar condição nova. Um quarto ponto esquecido seria um caminho que continua denunciando ele, e
o silêncio do código não acusaria nada.

**Sumir é silencioso para todo mundo menos para ele**, e isso é decisão de jogo, não esquecimento:
não existe pacote de "som de sumir" para os vizinhos. Eu tinha feito o contrário (o som vazava,
como a tampa do duto) e o usuário corrigiu - o fantasma some de verdade, sem rastro. Quem estava
perto simplesmente para de ouvir os passos daquela pessoa. **Se um dia isso soar forte demais, o
conserto é devolver o som aos vizinhos**, e não enfraquecer a invisibilidade por outro caminho.

Duas sutilezas: **matar devolve ele na hora** (senão não há jogo do outro lado - e o som do
assassinato, esse sim, é público), e a **reunião também** - invisível não é ouvido, e ficar mudo na
mesa sem explicação é um sinal luminoso de quem é o fantasma. E `was_invisible` existe porque `invisible_remaining == 0` não distingue "acabou de
voltar" de "nunca sumiu": sem a marca, o som da volta tocaria para todo mundo, o tempo todo. Sonda:
`tools/probes/probe_phantom.nvgt`.

**Papel declara o que pode; o código pergunta por capacidade.** `src/game/roles/role_traits.nvgt`
define cada papel (pode matar, ventilar, sabotar, o que percebe) e `player_abilities` combina isso
com estar vivo. Nada deve voltar a perguntar "é impostor?" para decidir uma ação — era assim que a
resposta virava um booleano em mais de sessenta pontos, e cada papel novo obrigava a revisitar todos.
Um papel novo entra em três passos, descritos no topo daquele arquivo.

### Arquivos de som

**Todo som do jogo é `.ogg`** (Vorbis, qualidade 3), fora dois `.mp3` antigos. Foram 45 MB de wav
virando 3,4 MB sem perda audível, e o download caiu de 60 MB para 29 MB. Som novo entra convertido:
`D:\Program\winvox\ffmpeg.exe` (existe na máquina, mas **não está no PATH**) com
`-c:a libvorbis -q:a 3`. Depois da conversão, o maior peso do zip passou a ser `lib/phonon.dll`
(17,9 MB comprimido), que é o motor de áudio posicionado e não sai.

**Volume de som se mede, não se chuta - e depois se confere de ouvido.** Os arquivos de passo vinham
com até 15 dB de diferença entre pisos, o que num jogo onde o passo alheio é a pista principal deixa
uma sala segura por acidente. A correção é por piso, em dB, em `footstep_volume_db()` - no código, e
não nos arquivos, para ajustar sem reconverter. Duas armadilhas na medição: RMS do arquivo inteiro
inclui o silêncio do fim e subestima som curto (medir só o que está acima de 1% do pico), e mesmo a
medida certa erra para mais - o Tile pedia +17,6 pela conta e ficou alto demais, voltando para +15.

**Família de volume nova entra em QUATRO lugares.** `SOUND_FAMILY_<NOME>` (índice), `..._COUNT`,
`SOUND_FAMILY_IDS` (o id vai para o disco como `volume_<id>`, então acrescente sempre no FIM para
não mexer no que o jogador já ajustou), `SOUND_FAMILY_NAME_KEYS`, e `FAMILY_PREVIEW_SOUNDS` em
`ui/settings_screens.nvgt` - as três listas são indexadas pelo mesmo número, e uma menor que
`COUNT` é leitura fora do array na tela de volumes. Mais as chaves de texto nos dois idiomas.

**A medição de piso virou ferramenta: `python tools/measure_footsteps.py <prefixo>`.** Ela dá o corpo
em dBFS, a correção sugerida contra o MetalTile (a referência) e - o número que decide - o **ruído de
fundo** depois da correção. Um piso muito baixo só pode ser levantado se o silêncio entre as batidas
for silêncio de verdade: o vidro pediu +19,2 dB e isso só passou porque o ruído dele está em -70,5
dBFS, indo para -53 com o ganho, praticamente o mesmo -54,8 do próprio MetalTile. Ela reproduz os
números que já estavam no código (EarthTile, O2Tile) dentro de ~1,5 dB, e a diferença é sempre a
mesma: **a conta exagera e o ouvido puxa de volta** (o Tile pedia +17,6 e ficou em +15; o vidro pede
+19,2 e ficou em +17). Aplique a conta, depois confira de ouvido.

**Piso novo entra em DUAS tabelas, e esquecer uma falha calado.** `footstep_variant_count` tem um
fallback de 2 variantes: um piso com oito arquivos que não esteja na tabela toca sempre os dois
mesmos passos, e nada acusa - o som existe, só soa repetitivo. `FOOTSTEP_FLOOR_PREFIXES` é a outra:
sem ela os arquivos não entram no `sounds.dat` e o piso fica mudo no jogo compilado (funciona
rodando do fonte, que é o pior jeito de descobrir). E o ganho do piso se MEDE: o O2Tile veio 9 dB
acima da referência, com picos a -1,7 dB - sem correção, uma sala de seis por seis viraria a mais
barulhenta do mapa por acidente de gravação. Sonda: `tools/probes/probe_floor.nvgt`.

**Tela que ABRE com som tem que FECHAR com som.** Os painéis de luzes e de oxigênio abriam com a
tampa (`SND_OPEN_PANEL`, o som de interagir com o objeto no mapa) e terminavam em silêncio - nada
marcava o fim, nem ao concluir nem ao desistir. O `panel_close.ogg` existia no repositório e nunca
tinha sido tocado; hoje sai em `run_sabotage_panel`, nos dois fins, porque o som é da TAMPA e não do
resultado. O de comunicações fica de fora: ele tem o par próprio (`SND_COMMS_PANEL_*`) e tocar os
dois seria a mesma tampa fechando duas vezes. As tasks já tinham o par (`SND_TASK_INPROGRESS` ao
abrir, `SND_TASK_COMPLETE` ao terminar) - a assimetria era só dos painéis.

**Nome de arquivo de som é `snake_case`, e a PASTA diz o que o som É** - não onde ele toca:
`events/` é acontecimento do mundo (morte, alarme, transformação, vitória), `beacons/` é marcador
posicionado contínuo (o corpo no chão está aqui, não em events), `ui/` é interface (menu, votação,
chat), `world/` é objeto do cenário (porta, painel, esbarrão), `ambience/`, `steps/`, `tasks/`. Os
nomes foram uniformizados de uma vez (`bodyReport` -> `body_reported`, `emergence` ->
`sabotage_alarm`, `killed<N>` -> `death<N>`, que se confundia com `kill`); só `steps/` ficou como
estava, porque os nomes ali são PREFIXOS lidos pelo código (`FOOTSTEP_FLOOR_PREFIXES`).

Renomear som é seguro **desde que** o `check_sounds` rode depois: ele compara catálogo e disco nos
dois sentidos e é a única coisa que pega um caminho errado - som que não carrega falha em silêncio.
Cuidado com as referências CONCATENADAS (`"sounds/events/death" + i + ".ogg"`, o prefixo em
`sound_variants_list`): elas não aparecem numa busca pelo caminho inteiro.

**Som novo vai na subpasta certa de `sounds/`** (`steps`, `ambience`, `beacons`, `tasks`, `events`,
`ui`, `world`) e o caminho no catálogo inclui a pasta. Depois de mexer em som, rode
`nvgt tools/check_sounds.nvgt`: ele compara o catálogo com o disco **nos dois sentidos** e é a única
coisa que pega um caminho errado — som que não carrega falha em silêncio, sem erro nenhum.

### Chat de voz

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

### Telas, laço do cliente e fila de pacotes

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

### Rede, protocolo e sessão

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

### Ciclo da partida

**Quando a partida acaba, o laço de pacotes PARA de drenar a fila.** O que vem logo atrás do
`S_GAME_OVER` já é da próxima rodada - o retrato da sala reaberta e, se o anfitrião não esperou, o
`S_GAME_START`. Como nenhuma condição do laço de partida trata um início de partida, continuar
drenando o jogava fora em silêncio, e quem tinha ficado na tela de resultado ficava de fora da
rodada. São TRÊS peças para esse caminho funcionar, e ele só funciona com as três: a tela fecha
sozinha (`run_menu_until_next_match`), o laço para de drenar (o `break` dentro do while), e a sala
de espera pega o `S_GAME_START` que estava na fila. Uma sonda por peça:
`probe_limbo`, `probe_fim_para_proxima` e `probe_entra_na_partida` - as três conferidas contra a
versão sem a correção, onde falham.

**Tela que o jogador pode deixar aberta precisa saber fechar sozinha.** A de resultado é a única
sem pressa - dá para sair para atender o telefone com ela aberta -, e `menu.run()` só sai por tecla.
Quando o anfitrião começava a rodada seguinte, o `S_GAME_START` chegava e ficava na fila sem ninguém
para lê-lo: a pessoa ouvia as vozes da partida nova acontecendo sem ela e não conseguia jogar.
`run_menu_until_next_match` abre o laço do menu e devolve quando esse pacote aparece. Duas coisas
aprendidas junto: a fila é examinada a partir de um CURSOR que só cresce (ninguém a consome com o
menu aberto), porque varrê-la inteira por quadro é o que já travou o jogo; e **uma sonda que
reimplementa a lógica que testa não testa nada** - a primeira versão desta passava com o código
quebrado, e só virou teste de verdade quando passou a chamar a função real e a ser conferida contra
a versão sem a correção (onde ela trava).

**A reunião é uma PAUSA, e o que não funciona nela não pode recarregar dentro dela.** Ninguém mata,
ninguém usa habilidade e ninguém anda durante a discussão - então todo relógio que continue correndo
ali é tempo ganho de graça, e a reunião vira a melhor jogada de quem tem o relógio mais caro. O
cooldown de kill já era rearmado no fim da votação; a habilidade do papel não era, e o metamorfo saía
da mesa pronto para virar alguém no meio de todo mundo. Hoje quem decide é o PAPEL
(`ability_cooldown_resets_on_meeting`, padrão `true`), e não um `if (é impostor)`: as habilidades de
DEFESA se recusam, com o motivo escrito em cada uma - a sabotagem atravessa a reunião intacta, então
rearmar o engenheiro junto entregaria ao impostor, pelo outro lado, exatamente o que a regra queria
tirar dele. Efeito ATIVO é outra coisa: disfarce, invisibilidade e escudo **caem** ao começar a
reunião, e cada um por um motivo próprio (dois nomes iguais na lista de votação são uma tela
quebrada; invisível não é ouvido e ficar mudo na mesa denuncia o fantasma; um escudo mantido só
queima o relógio numa fase em que ninguém ataca, e quanto sobra dependia do tamanho da discussão).
Regra prática ao acrescentar qualquer timer de partida: decida as DUAS coisas separadamente - a
reunião apaga o efeito? a reunião rearma a recarga? - e escreva as duas no mesmo commit. Sonda:
`tools/probes/probe_meeting_reset.nvgt`.

**A DISCUSSÃO é contada no cliente, não no servidor.** O `S_VOTING_STARTED` sai colado no
`S_MEETING_STARTED`; o servidor manda `discussion_time` dentro do pacote da reunião e é o cliente que
segura o voto até o tempo passar (o `voting_timer` do servidor é discussão + votação juntas, só como
prazo máximo). Consequência para quem escreve sonda: votar assim que a votação abre produz uma
reunião de UM segundo, e aí "a recarga não andou" e "a recarga foi rearmada" dão exatamente o mesmo
número - a primeira versão da `probe_meeting_reset` "reprovou" a correção certa por isso. Quem quiser
medir o efeito de uma reunião precisa esperar a discussão como um jogador esperaria.

**Regra que o cliente também aplica tem que morar numa função só.** O cliente decide se ABRE o
microfone e o servidor decide QUEM ouve; quando as duas decisões são escritas separadas, elas
divergem e vence a mais restritiva - foi assim que a regra "comunicações calam a reunião", mesmo
DESLIGADA, continuava calando a reunião: o servidor deixava a voz passar e o cliente nem capturava.
Hoje as duas perguntam a `voice_down_for` (em `core/sabotage_rules.nvgt`), que recebe se está em
reunião e o que a sala escolheu; a regra da sala viaja no `S_GAME_START` justamente para o cliente
poder responder igual. Sonda: `tools/probes/probe_voice_meeting.nvgt`.

**Som contínuo precisa de UM dono, e o dono é quem reavalia o estado.** O alarme de sabotagem era
ligado em três lugares (início da sabotagem, volta da reunião) e desligado em outros, e cada caminho
esquecia um: quem morria no meio da sabotagem continuava ouvindo, e a volta da reunião religava para
todo mundo, vivo ou não - foi o recado #96, chegado JÁ com a correção parcial no ar. Hoje quem manda
no alarme é `apply_sabotage_effects`, o mesmo lugar que decide os outros efeitos, e que roda no
começo, no conserto, na MORTE e na volta da reunião. O `meeting_ui` não toca mais nele. Sonda:
`tools/probes/probe_alarme_fantasma.nvgt`, que chama a função de verdade e olha o slot do som.

**Recurso de UM de cada vez precisa ser recusado no servidor E anunciado ao cliente.** O scanner da
enfermaria guarda um `visible_task_peer` só; com três pessoas entrando juntas, elas se sobrescreviam
ali e o fim do scan de uma não avisava ninguém - o som ficava tocando sem relação com quem estava
nele. Hoje `try_start_visible_task` recusa o segundo, e o cliente sabe que está ocupado pelo mesmo
`S_TASK_VISIBLE` que já recebia, então nem abre a minigame. A mesma forma do posto de câmeras.

**Trabalho que o servidor vai recusar não pode ser oferecido ao jogador.** Duas versões disso no
mesmo lugar: o fantasma abria o painel de sabotagem, resolvia a minigame inteira e só então ouvia
que não valia (o servidor exige estar vivo), e quem chegasse num painel de oxigênio já resolvido
digitava o código para nada. O cliente agora sabe dos dois - pelo `alive` que já tinha e pelo
`S_SABOTAGE_PROGRESS`, que diz QUAL painel saiu do caminho - e recusa na hora, explicando. E o
painel aberto fecha sozinho quando alguém resolve antes (`sabotage_ended` no game_client, marcado na
chegada como o `interrupt_pending`, porque varrer a fila por quadro já travou o jogo uma vez).
Sonda: `tools/probes/probe_reparo.nvgt`.

**Sabotar é a única capacidade que sobrevive à morte.** O impostor morto continua sabotando (como
no original) - matar, ventilar e fechar portas acabam com ele. A regra está em dois lugares que
precisam concordar: `player_abilities.can_sabotage()` (que oferece a tecla) e
`game_state.trigger_sabotage` (que decide). Cuidado ao editar: `close_doors_of_room` tem uma linha
de guarda IDÊNTICA à da sabotagem, e uma substituição descuidada troca a função errada sem que nada
acuse - a sonda `tools/probes/probe_ghost_sabotage.nvgt` cobre as duas justamente por isso.

**Voz posicionada é só para quem está VIVO e andando.** `g_voice.spatial = alive && !movement_frozen`
é decidido a cada quadro, e não em cada transição: são três (morrer, começar a reunião, acabar a
votação) e esquecer uma deixa o jogador num estado que nada mais corrige. Na reunião todo mundo está
na mesma mesa; o fantasma fica onde morreu, então com posição ele ouvia a discussão e os outros
fantasmas - espalhados pelo mapa - de tão longe que virava silêncio. Morto não tem lugar.

### Mapa e tarefas

**O grafo de navegação sai do NOME do corredor - e só sabe ligar sala a sala.** `zone_neighbors`
parte `corridor_<sala_a>_<sala_b>` em três e é assim que os bots acham caminho. Uma sala pendurada
num CORREDOR (a do oxigênio, que se abre para o corredor de carga) não cabe nessa forma: o id teria
quatro partes, o grafo a ignoraria e os bots nunca chegariam lá - sem erro nenhum, só bots que não
vão àquela sala. Para esses casos existe `zone_link`/`extra_links`, uma ligação declarada à mão.
Ao acrescentar zona nova, confira as duas coisas separadamente: a GEOMETRIA decide por onde o
jogador anda (`zone_at`/`can_move` são puramente retangulares, e zonas não podem se sobrepor, senão
o jogo diz a sala errada conforme qual for encontrada primeiro), e o GRAFO decide por onde o bot
anda. Sonda: `tools/probes/probe_o2room.nvgt`.

**Ninguém mata de dentro do duto, e ninguém morre dentro dele - a regra vale para as duas pontas
e mora no topo do `try_kill`.** A razão é uma só: a posição de quem está num duto PARA de ser
transmitida (ver `on_move`), então o atacante está mirando o último lugar conhecido, e não onde a
pessoa está - matar por posição congelada é matar através da parede.

Do lado do ATACANTE foi um recado de jogador: escondido, fora do radar e da câmera, ele alcançava
quem passasse ao lado da tampa - assassinato sem contra-jogada, porque não havia como saber que ele
estava ali.

**Invisibilidade não entra nesta regra, em ponta nenhuma.** O fantasma mata e fica visível, que é o
desenho combinado do papel; e proibir matá-lo enquanto sumido seria uma mudança de equilíbrio por
tabela. Eu tinha posto `target.invisible()` junto do duto ao consertar isto, e o usuário cortou: um
conserto de duto mexe em duto. Vale a regra geral - quando uma correção "de graça" cair num papel
que não estava em discussão, ela não é de graça.

Do lado do ALVO era pior do que parece, e é o "bugando" do recado: o morto continuava com `vented`
ligado, e **fantasma ventilado não anda** (`try_move_player` recusa) **nem sai** (o menu do duto
exige estar vivo) - travava no lugar pelo resto da partida, sem mensagem nenhuma. É a MESMA armadilha
que a reunião já tinha tido, aparecendo por outra porta; quando aparecer uma terceira, o lugar de
consertar continua sendo a origem, não o sintoma. Sonda: `tools/probes/probe_vent_kill.nvgt`, que
confere também que o mesmo kill funciona um instante depois com os dois fora do duto - sem isso, uma
recusa por recarga ou alcance passaria por "o duto protegeu".

**Duas redes de dutos, e elas não podem se tocar.** A rede se declara nos `linked_object_id` de
cada duto (lista separada por vírgula), e é o que permite ao jogador deduzir para onde alguém pode
ter ido: "a tampa abriu na navegação, então ele saiu em armas ou no reator". Ligar todos os dutos
numa rede só mataria essa dedução - por isso a rede nova (corredor jogos-estufa ↔ corredor da
segurança) é SEPARADA da antiga, e a sonda `tools/probes/probe_vents.nvgt` confere justamente que
uma não alcança a outra. Duto em corredor tem um custo que duto em sala não tem: o som da tampa é
ouvido por quem está passando. E rede de dois dutos é determinística de propósito (quem ouve sabe
onde ele vai sair) - é a troca de velocidade por previsibilidade.

**Um tipo pode ter VÁRIAS correntes, e a escolhida mora no JOGADOR.** O destino saía de
`map.chain_object_at(tipo, fase)`, que é global: todo mundo com "abastecer os motores" ia ao mesmo
lugar, e rota única é informação de graça para quem deduz - sabia-se de antemão por onde quem estava
abastecendo teria que passar. Hoje o mapa declara duas correntes para `fuel_engines` (depósito ->
reator e depósito -> motor leste), `map.chains_starting_at()` devolve as que começam no ponto
sorteado, o servidor escolhe UMA no sorteio e ela fica em `player_task.chain`. **O cliente não mudou
nada**: ele já só seguia o `object_id` da fase atual.

Duas coisas para lembrar ao acrescentar uma corrente alternativa: as alternativas têm que começar no
MESMO ponto (senão o sorteio entrega uma tarefa que começa noutro lugar - por isso o filtro é por
ponto de início, e não só por tipo), e **sonda que fixa o destino quebra**. A `probe_phases` mandava
`task_fuel_reactor` na segunda fase; com dois destinos ela acertava metade das vezes e, na outra
metade, o servidor não achava a tarefa, não respondia, e a sonda ficava esperando até o prazo -
sintoma "a sonda trava de vez em quando". Hoje ela usa o `next_object_id` que a resposta traz.

E uma propriedade que a sonda do motor descobriu: **o servidor NÃO confere distância para concluir
tarefa** - ele confia no cliente, que só abre a minigame quando está perto. Isso torna a sonda barata
(dá para avançar fases sem caminhar), e é bom saber que está assim de propósito ou não.

**Tarefa de VÁRIAS FASES: a sequência mora no mapa, a fase mora no servidor.** `task_chain`
(`game/map.nvgt`) lista os pontos de uma tarefa na ordem, junto dos objetos - separada deles, um id
renomeado quebraria a corrente em silêncio. `player_task` carrega `phase`/`phase_count` e o
`object_id` da fase ATUAL, então marcador, lista de tarefas e `find_interactable` seguem a fase sem
saber que ela existe. Quem avança é `game_state.advance_task`, no servidor: quem for interrompido
por uma reunião no meio da travessia volta com o galão na mão, e não do começo - punir quem reporta
um corpo seria punir o comportamento que o jogo quer. Três armadilhas que já custaram: a contagem
da equipe conta FASES (`task_count` soma `phase_count`), senão a barra congela durante a travessia
inteira; só o PRIMEIRO ponto da corrente entra no sorteio (`is_chain_start`), senão alguém recebe a
segunda metade de uma tarefa que nunca começou; e o modo de treino roda todas as fases em sequência,
senão "treinar" a tarefa é só a etapa que não tem o que treinar. Sonda:
`tools/probes/probe_phases.nvgt`.

**O sorteio de tarefas tem DOIS tetos, e os dois são do sorteio, não da sala.** Antes ele era
independente por jogador, e a variância mandava.

A **tarefa visível** (hoje só o scan) sai para no máximo `visible_task_cap(jogadores)` pessoas -
20% arredondado para cima, 1 numa sala de 5 e 2 numa de 10. Ela é a única prova de inocência do
jogo, e sem teto uma rodada em cada tantas entregava álibi a quase todo mundo (medido: 4 dos 5
tripulantes numa sala de 6). Há um motivo mecânico junto do de equilíbrio: o scanner é recurso de
UM DE CADA VEZ no servidor, então muita gente com essa tarefa vira fila e "ocupado". 10% foi
considerado e descartado - daria exatamente 1 em qualquer sala até 10 pessoas, o que faz do álibi
um bilhete de loteria em vez de uma economia. **"Visível" é uma LISTA** (`VISIBLE_TASK_TYPES`), e
não um `task_type == "submit_scan"` solto: a segunda tarefa visível precisa nascer com teto e com
anúncio, e a lista é o que obriga a lembrar dos dois.

E **no máximo `MAX_TASKS_PER_ROOM` (2) tarefas do mesmo jogador na mesma sala do mapa** - para
ninguém resolver a lista inteira num canto, num jogo em que andar é fazer barulho. É
**preferência, não parede**: se respeitá-la impedisse fechar o total pedido, o total ganha (a mesma
escolha que o sorteio já faz entre longa e curta). Só morde em navegação (4 pontos) e admin (3); as
outras salas já não têm mais que 2. Quem escreve o sorteio: `pick_task_spots` recebe `want` como
QUANTO ACRESCENTAR, e não como tamanho final - as duas chamadas (longas e curtas) escrevem no mesmo
array, e comparar com `destino.length()` cru fazia a segunda achar a cota cumprida e devolver um
jogador só com as tarefas longas, sem erro nenhum. Sonda: `tools/probes/probe_task_limits.nvgt`
(`probe_task_mix` não pegou porque só IMPRIME o que saiu, sem cobrar o total).

**Tarefa NOVA entra em OITO lugares, e a maioria falha em silêncio.** Na ordem em que se esquece:
o minigame (`game/tasks/<tipo>.nvgt`), o `#include` **e** o `else if` do `task_manager` (só o
include compila e a task nunca abre), o ponto no `map.nvgt` com beacon, `PRACTICE_TASKS` em
`ui/practice_screen.nvgt` (sem isso não dá para testá-la sem montar partida), o beacon em
`ui/onboarding_screens.nvgt` ("Conhecer o mapa"), as constantes **e a lista do build_pack** em
`audio/sound_catalog.nvgt` (sem a lista, o som não entra no `sounds.dat` e a task fica muda só no
jogo compilado), as chaves nos DOIS idiomas, e a seção de tarefas dos dois `docs/README_*`. Decida
também se ela é longa (`LONG_TASK_TYPES`) - o padrão é curta.

**O jeito que funciona de inventar tarefa: pegar a ORIGINAL e traduzi-la para o ouvido.** Foi a
correção do usuário depois de eu propor três levas ruins (tarefas de mundo, mais painéis, e a
mistura revertida). A pergunta certa não é "que mecânica de ouvido falta?" - é **"o que o jogador
faz com as mãos no original, e como isso vira um gesto de ouvido?"**. A fiação já era isso e é a
melhor tarefa do jogo. A biblioteca de sons do usuário (fora do repositório,
`D:\documents\sons among us\sounds among`) tem o conjunto COMPLETO de várias originais que ainda não
existem aqui: calibrar o distribuidor, inserir as chaves, rodar diagnóstico, traçar a rota, ativar
escudos, desviar energia, nó do clima, separar amostras.

`clean_o2_filter` é a primeira feita assim. No original as folhas estão à vista e você arrasta cada
uma até a abertura; aqui elas estão escondidas em seis posições, o aspirador anda entre elas com as
setas, e o som responde: **tique seco = vazio, tique + farfalho = folha**. A versão anterior da
maquete tocava tudo sozinha e o veredito foi "ficou automático" - **o jogador tem que decidir, não
assistir**, e é o mesmo erro que a primeira versão do `water_plants` teve. Três decisões que a
seguram: seis posições para três folhas (com uma em cada, bastava apertar Enter seis vezes sem ouvir
nada); as folhas **se mexem** a cada aspirada, senão dava para mapear a câmara numa varredura e o
resto virava digitação; e o beacon é o MESMO arquivo do ar que toca dentro da task, então de longe se
ouve o filtro puxando e de perto aquele puxar vira o norte fixo.

**MAQUETE ANTES DE CÓDIGO.** Monte a ideia com `ffmpeg` (um `.wav` com os sons nas posições e nos
tempos certos) e OUÇA antes de escrever a task. Custa dez minutos; a task revertida custou uma
sessão. Duas armadilhas da maquete em si: `adelay=0|0` é recusado (o evento em zero não leva o
filtro), e **make-up gain demais + `alimiter` achatam tudo no mesmo nível** - foi assim que o "tique
vazio" e o "tique com folha" saíram idênticos numa medição, que é justamente a informação que a task
existe para carregar. Meça janelas do arquivo com `volumedetect` em vez de confiar no ouvido para o
balanço: no filtro, o farfalho ficou ~9 dB acima do tique, e esses números viraram as constantes em
dB no topo da task.

**SOM CONTÍNUO NÃO SE LOCALIZA, e vários ao mesmo tempo viram parede.** Isto custou uma tarefa
inteira, escrita e revertida (`stabilize_lines`, commit 53341d0 e a reversão logo atrás - dá para
recuperar o código de lá se um dia servir). A ideia era "ouvir para dentro de uma mistura": quatro
linhas zumbindo juntas, cada uma numa posição, e o jogador diria qual estava oscilando. O veredito
de quem ouviu foi "uma miscelânea de sons todos juntos, não está localizado, ficou horrível" - e
estava certo.

O erro é de acústica, não de ajuste: **o ouvido localiza por ATAQUE**. Um zumbido constante quase
não tem transiente, então mesmo panoramizado ele vira um borrão largo em vez de um ponto; quatro
deles se mascaram e o resultado é ruído. Mistura de verdade - um passo por cima da ambiência da sala,
que é o que o jogo faz o tempo todo - funciona porque há **um fundo estável e UMA coisa intermitente
dentro dele**. Quatro primeiros planos não são uma mistura, são uma parede.

Regra que sai daí, para qualquer tarefa futura: som que precisa ser localizado tem que ter ataque, e
no máximo um elemento contínuo por vez. Antes de escrever a task, monte a ideia com `ffmpeg` e ouça
- misturar quatro arquivos e escutar custa um minuto, escrever a task custou uma sessão.

**Tarefa longa vale mais aqui do que no jogo original.** `LONG_TASK_TYPES` (em
`config/game_constants.nvgt`) marca as tarefas que fazem atravessar a nave ou ficar parado um bom
tempo; o que não é comum nem longo é CURTO, sem terceira lista para sair de sincronia. O motivo não
é paridade com o original: nesta versão andar é fazer barulho, e barulho é a moeda do jogo - uma
tarefa curta feita num canto não produz informação para ninguém, uma longa é álibi para quem a faz
e janela para o impostor. O sorteio é por categoria (`on_start_game`), e quando o mapa não tem
pontos suficientes de uma delas o resto vem da outra: entregar o total pedido importa mais do que a
proporção exata. A configuração é **total + quantas longas** (não uma contagem por categoria): as
curtas são o resto, ninguém faz conta, e as versões anteriores, que só mandavam o total, continuam
entendidas. Sonda: `tools/probes/probe_task_mix.nvgt`.

### Sinais sonoros e preferências de audição

**O bip de alvo ao alcance é SÓ de quem pode agir, e só na TRANSIÇÃO.** Impostor, xerife e anjo
ouvem quando alguém entra no alcance da ação deles - é o equivalente sonoro do botão de matar
acendendo no original. Tocar no ALVO avisaria a vítima de que o impostor está do lado dela, então
ele nunca sai para mais ninguém. Na transição, e não contínuo, pela regra de sempre: num jogo em que
se está tentando ouvir passos, um bip repetido por cima é ruído. E ele usa a MESMA
`roster.nearest_alive` (com as mesmas exclusões) da tecla que vai agir, senão prometeria um alvo que
a tecla depois recusa. Pode ser preferência pessoal, e não regra da sala, porque **não cria
informação nova**: a posição de todos já chega ao cliente, isto é só apresentação.

**O quanto o tom cai para o sul é do JOGADOR** (`spatial_pitch_percent`, padrão 6%, teto 25%): há
quem não distinga norte de sul com o padrão e queira reforçar, e quem ache que variar o tom
descaracteriza os passos - o piso se reconhece pelo timbre - e prefira desligar. O `sound_pool`
GUARDA o valor, então mexer na preferência exige `apply_pitch_setting()` no pool que está tocando
(`g_active_spatial`); sem isso o ajuste só valeria na partida seguinte e o jogador testaria achando
que não funcionou. Os dois ajustes ficam numa tela PRÓPRIA ("Como eu ouço o jogo"), separada dos
volumes: eles mudam a INFORMAÇÃO que o som carrega, não o quão alto ele é.

**Um sinal sonoro, um significado.** O tom do bip do radar dizia a DISTÂNCIA (130 perto, 80 longe)
enquanto o tom de todo o resto do jogo diz NORTE/SUL (`SPATIAL_SOUTH_PITCH_DECREASE`, 6%). Os 50%
da distância engoliam os 6% da direção, e o radar deixava de responder à única pergunta que o
estéreo não responde sozinho: "está acima ou abaixo de mim?". Hoje o tom do radar é só direção, e a
distância fica por conta da atenuação do som posicionado. Antes de usar tom, volume ou repetição
para uma informação nova, veja o que aquele canal já significa em outro lugar do jogo.

### Sala de espera e saguão

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

### Deploy, infraestrutura e ferramentas

**"Não consegui extrair server_main.zip" no deploy = servidor compilado SEM `-plinux`.** Para
conferir que o servidor compila, é tentador rodar `nvgt -c server_main.nvgt` - isso gera um
`server_main.zip` WINDOWS, mais novo que o `server_main.tar.gz` Linux, e o `deploy.ps1` escolhe
o pacote mais novo. Ele falha na extração por sorte (o zip não tem o binário `server_main`); se
não falhasse, publicaria um executável Windows no contêiner. Apague o zip e gere com
`nvgt -c -plinux server_main.nvgt`. Para só conferir que compila, use `-plinux` também.

**"docker build falhou" logo no começo do deploy = Docker Desktop parado** (a máquina reiniciou).
`Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"` e esperar `docker info`
responder; o deploy não inicia o Docker sozinho.

**Nunca `latest` como tag de imagem.** Com tag fixa o Kubernetes não vê diferença e não reinicia nada.

**Commite ANTES de publicar o servidor.** A imagem leva o nome do commit atual, e isso só é verdade
se a árvore estiver limpa: a 0.19.0 subiu etiquetada `6b40082` com código que o `6b40082` não tinha.
O `deploy.ps1` agora recusa árvore suja quando o servidor entra no deploy. O cliente não tem esse
problema - o zip não carrega nome de commit.

**Quem saiu da partida continua na lista de jogadores dela** (como registro, para a contagem de vivos
não quebrar) **com o mesmo peer_id da conexão** - e a conexão pode estar em outra sala. Todo envio em
massa passa por `reaches_client()`, que pula bot e quem saiu. Sintoma quando isso faltou: o jogador
recebia o fim da partida ANTIGA dentro da nova e o cliente, obediente, saía dela "como se tivesse
vencido".

**`bind_text` corta dados BINÁRIOS no primeiro byte nulo - hash/binário em coluna TEXT vai como
hex.** O token de sessão guardava `string_hash_sha256(token)` direto: no Windows essa chamada volta
como texto hexadecimal, mas no LINUX volta binário, e os 32 bytes com um nulo no meio eram cortados
para 6 na hora do bind_text - o login por token nunca casava na produção, embora passasse no teste
local (que roda no Windows). Pior: `string_to_hex` TAMBÉM não produz hex na build Linux, então não
adianta embrulhar a chamada nele. A saída que funciona nos dois é hexar os bytes À MÃO
(`character_to_ascii` + tabela de dígitos, ver `token_hash` em user_db). A mesma armadilha vale para
qualquer binário que for para uma coluna TEXT, e o teste local no Windows NÃO pega - confira contra
a produção (sonda + `kubectl exec ... base64 among_users.db`, olhando o comprimento da coluna).

**"Lembrar de mim" guarda um token de sessão, nunca a senha.** O NVGT não alcança o Cofre de
Credenciais do Windows nem DPAPI, então senha em disco seria texto puro. O servidor emite um
token aleatório no login com `remember` (tabela `sessions`, só o SHA-256 dele; vence depois de
`SESSION_TOKEN_DAYS` sem uso), o cliente guarda o token nas preferências e entra com `C_LOGIN
{token}`; "Sair da conta" manda `C_LOGOUT` e apaga. Login com senha SEM lembrar apaga o token
local, para computador emprestado não ficar com a sessão de alguém. Sonda: `tools/probe_session`.

**Na reunião a voz toca SEM posição** (`g_voice.spatial = false` entre S_MEETING_STARTED e
S_VOTE_RESULT): os vivos são teleportados para a cafeteria e o fantasma fica onde morreu - com
posição, ele ouvia a discussão de longe, quase inaudível.

**Respostas aos recados vão pelo jogo, autorizadas por token de ambiente.** `C_ADMIN_REPLY` só passa
se o token bater com `AMONGUS_ADMIN_TOKEN` no servidor (segredo `amongus-admin` do cluster; sem ele
o servidor recusa tudo). Não há conta de administrador de propósito: seria mais uma senha dentro do
banco que ela protege. Para testar local: `$env:AMONGUS_ADMIN_TOKEN` antes de subir o servidor e
`reply_feedback.ps1 -Local`. A variável `AMONGUS_SERVER_HOST` faz qualquer ferramenta ou sonda
apontar para outro servidor sem criar `server.txt` (que o jogo de verdade também leria).

**Ferramenta que inclui só o cliente precisa dos `#include` certos.** `tools/reply_feedback` não
compilava: `protocol.nvgt` usa `tr()` e `game_constants.nvgt` usa `DEFAULT_LANGUAGE`, e os dois
vinham por ordem de inclusão. Agora os dois incluem `i18n.nvgt`. Sintoma: "No matching symbol 'tr'"
numa ferramenta nova, com o jogo compilando normalmente.

**`read_feedback.ps1` mostra os recados PENDENTES; `-All` traz os arquivados junto.** O padrão já
foi "os 20 mais recentes", e isso pareceu truncamento - os recados chegam em dezenas por dia; hoje o
filtro é por estado, não por quantidade. `-After <id>` continua servindo para retomar de onde parou.

**Recado tratado se ARQUIVA, não se apaga** (`infra\resolve_feedback.ps1 -Id 42` ou `-Until 96`, e
`-Undo` desfaz). O contexto de um recado - versão, papel, sala, crash.log - é justamente o que falta
quando o mesmo problema volta meses depois. Quem escreve no banco é o SERVIDOR, por comando de rede
(`C_ADMIN_RESOLVE_FEEDBACK`, mesmo token do `reply_feedback`), e nunca uma ferramenta mexendo no
arquivo por fora: ele mantém o SQLite aberto o tempo todo, e duas mãos no mesmo arquivo é pedir
corrupção. A coluna entra por `ALTER TABLE` ao abrir o banco, porque o `CREATE TABLE IF NOT EXISTS`
não roda num banco que já existe - e o `read_feedback` tolera os dois formatos, já que é usado antes
e depois do deploy que acrescenta a coluna.

**Fora do git:** `terraform.tfvars`, `*.tfstate`, `sounds.dat`, `*.zip`, `*.exe`, `crash.log`,
`among_users.db`, `server.txt`.

## Traduções

**Fonte da verdade: github.com/otaviols/game-translations** (clone em `D:\git\game-translations`),
pasta `among-us/lang/`. `en_US` e `pt_BR` são EMBUTIDOS e mantidos aqui, junto do código (as chaves
novas nascem aqui); os outros idiomas são da comunidade e vivem lá. `tools\sync_translations.ps1`
faz os dois sentidos - traz os da comunidade para `lang/`, manda os embutidos para lá como referência
(commit + push automático) - e roda no começo do `build_clients.ps1`. Não edite um idioma da
comunidade em `lang/`: o próximo sync sobrescreve; edite no repositório de traduções.

**Como uma tradução chega:** o jogador manda pelo jogo ("Enviar uma tradução", na lista de partidas)
-> fica no banco do servidor, UMA por (usuário, idioma), reenvio substitui -> `infra
ead_translations.ps1`
traz para `translations_inbox/` e APAGA do servidor -> `python tools/check_translation.py <arquivo>`
diz o que falta/sobra -> copiar para `D:\git\game-translationsmong-us\lang\<código>.json`,
commit, push -> responder ao jogador com `reply_feedback.ps1` se ele mandou recado -> o próximo build
traz. O `build_clients.ps1` recolhe a caixa de entrada sozinho e PARA se houver algo para revisar
(`-SkipInbox` pula). Pull request no repositório também serve para quem sabe usar GitHub.

**O deploy CONFERE as traduções antes de qualquer outra coisa, e recusa.**
`python tools/check_translations_all.py` (exit 1 = não sobe). A regra de o que é erro vem de quem
mantém cada idioma: `pt_BR` fora de sincronia com `en_US` - em qualquer direção, faltando ou
sobrando - é defeito NOSSO e barra o deploy; idioma da comunidade atrasado é só aviso com a
contagem, porque chave faltando cai no inglês de propósito; e **envio de jogador parado em
`translations_inbox/` barra**. Existe `-SkipTranslations`, para emergência, e usar é escolher subir
no escuro.

Isso nasceu de um erro: a **0.32.0 subiu com um espanhol COMPLETO parado na caixa** (614 chaves,
zero faltando) enquanto o jogo distribuía um de 518 com 96 buracos. O `build_clients` já conferia -
mas tem `-SkipInbox`, e eu pulei. Por isso o portão de verdade fica no DEPLOY, que é o que vai ao
ar, e por isso ele **recolhe do servidor junto** (`read_translations.ps1`): uma tradução que o
jogador mandou ontem e ninguém baixou está tão atrasada quanto uma ignorada.

**Envio de jogador NÃO é necessariamente melhoria - compare a contagem de chaves antes de promover.**
Aconteceu na primeira vez que o portão barrou um deploy de verdade: chegou um `es_LATAM` novo, de
outra pessoa, com **518 chaves e 100 faltando**, enquanto o que estava no ar tinha 614 e nenhuma - e
o campo de tradutor dentro dele era de um TERCEIRO. Era o arquivo antigo, embutido num cliente
desatualizado, reenviado de volta. Promover teria desfeito uma tradução completa, e o portão teria
"passado" porque a caixa esvaziou. Rode `python tools/check_translation.py` nos DOIS (o envio e o que
está em `lang/`) e só promova o que tiver mais chaves.

Outras duas do mesmo lote: o envio pode vir com `language.translator` **vazio** (o turco veio), e
gravar por cima assim apaga o crédito de quem traduziu de graça - reponha do arquivo em uso; e dois
envios do mesmo idioma podem ser o mesmo arquivo com nomes diferentes. Processado - promovido OU
descartado -, o arquivo vai para `translations_inbox/processadas/`: a subpasta não dispara o portão e
guarda quem mandou o quê.

**`parse_json` LANÇA exceção em JSON malformado** (não devolve null). Já derrubou o servidor inteiro
num teste - um envio com `{` solto matou o processo. Todo `parse_json` de conteúdo que vem de fora
(rede, arquivo de idioma, version.json) fica em try/catch; ver `validate_translation_json`.

## Testar

**Minigame (task ou reparo) se testa no menu "Praticar tarefas e reparos"** do próprio jogo, sem
servidor: `src/ui/practice_screen.nvgt` roda o mesmo `run_task_minigame`/`run_sabotage_panel` da
partida com um `game_client` criado mas não conectado (`make_offline_client`). Serve para o jogador
como modo de treino e para quem desenvolve como bancada - antes, testar uma task era montar uma
partida de três.

Há um servidor de verdade no ar — **use-o**. O padrão que funcionou a sessão inteira: escrever um
`.nvgt` curto que conecta, faz a coisa e imprime o resultado, rodar com `nvgt arquivo.nvgt`, apagar
depois. Foi assim que se validou feedback, i18n, configurações de sala e limite de jogadores.

### O que as sondas ensinaram (custou caro descobrir)

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

## Infra — o que não é óbvio

**Não há VM.** Esta assinatura Azure não consegue criar nenhuma SKU barata
(`NotAvailableForSubscription` em todas as regiões), e as sem restrição têm **cota zero** — o que não
aparece em `az vm list-skus` e fazia o `terraform apply` falhar sempre no mesmo ponto. O servidor roda
no cluster AKS `aks-fallenrealms-alpha`, compartilhado com outro jogo, a custo marginal ~zero.

**O IP `20.206.112.223` é estático e mora fora do cluster**, porque vai compilado dentro do cliente
(`DEFAULT_SERVER_HOST`). Se mudar, todo mundo precisa de build novo.

**O segredo `ghcr-pull` tem prazo de validade.** O pacote da imagem é privado, então o cluster precisa
de credencial. Quando o token do GitHub expirar, o servidor para de subir com um `ImagePullBackOff`
genérico que **não menciona token**. Diagnóstico e cura em `infra/README.md`.

**Uma réplica só, e não é para aumentar** — o estado das partidas vive na memória do processo.

**O `among_users.db` (contas + feedback) vive no volume `amongus-data`.** `kubectl delete -f` apaga o
volume junto.

### Operar

```
kubectl get pods -n amongus
kubectl logs -n amongus deploy/amongus-server -f
infra\read_feedback.ps1 [-After <id>] [-WithCrashLog] [-Out arquivo]   # recados dos jogadores
infra\reply_feedback.ps1 -Id <id> -Text "..."   # responder; o jogador ouve dentro do jogo
infra\read_translations.ps1                    # traduções enviadas pelo jogo -> translations_inbox/ (e apaga do servidor)
```

## Pendências conhecidas

- **Recados do beta ainda sem resposta** (ver `infra\read_feedback.ps1`): pedidos repetidos de
  personagem/passos mais rápidos (#30, #32, #40, #43, #54 - decidido: virar configuração da sala,
  validada no servidor, com o padrão um pouco maior; ainda não feito); avisos que interrompem a
  fala do leitor (#52 pede um buffer de anúncios); bots impostores eficazes demais (#51); "não
  responde" no meio da partida (#49, #10 - sem log); jogo fechou ao apertar Tab logo depois de a
  câmera cair por sabotagem (#35 - sem log, não reproduzido); tradução completa para espanhol
  oferecida por Bauti (#31) - já existe um `es_LATAM` em uso; responder pelo `reply_feedback.ps1`.
  Já atendidos: mapa/orientação (Conhecer o mapa), regras na sala (O), radar travado (Q), partidas
  privadas.
- **Sons sem uso, de propósito** (o `check_sounds` os lista a cada execução; não são lixo):
  `steps/CarpetTile*` e `steps/SnowTile*` são pisos que o mapa ainda não tem - quando houver uma
  sala de carpete ou de neve, eles entram em `FOOTSTEP_FLOOR_PREFIXES` e na tabela de variantes
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
