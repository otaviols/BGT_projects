# Among Us Audiogame — guia do projeto

Jogo de dedução social **jogado inteiramente por som**, escrito em [NVGT](https://nvgt.gg)
(AngelScript). Cliente Windows distribuído por um site estático; servidor dedicado rodando como
contêiner num cluster AKS. Está em **beta**, com jogadores reais usando.

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
| mapa, sala, duto, tarefa, minijogo | [notes/mapa-e-tarefas.md](notes/mapa-e-tarefas.md) |
| começo/fim de partida, reunião, sabotagem, morte | [notes/ciclo-da-partida.md](notes/ciclo-da-partida.md) |
| protocolo, login, sessão, tela, menu, fila de pacotes, saguão | [notes/rede-e-telas.md](notes/rede-e-telas.md) |
| chat de voz | [notes/voz.md](notes/voz.md) |
| escrever uma sonda | [notes/sondas.md](notes/sondas.md) |
| publicar, servidor, Docker, Kubernetes, banco | [notes/infra-e-deploy.md](notes/infra-e-deploy.md) |
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
| `AmongUs.nvgt` | ponto de entrada do cliente |
| `server_main.nvgt` | ponto de entrada do servidor |
| `src/` | **todo** o código: `config/`, `core/`, `game/`, `network/`, `ui/`, `database/`, `audio/`, `i18n.nvgt` |
| `lang/` | **só dados** de tradução (`pt_BR.json`, `en_US.json`) — o motor de i18n fica em `src/` |
| `sounds/` | áudio fonte; vira `sounds.dat` no build |
| `tools/` | `build_pack` (gera o `sounds.dat`), `check_sounds`, `bots`, `build_clients.ps1`, `sync_translations.ps1`, `check_translation.py`, ferramentas de administração, `probes/` (sondas que conversam com um servidor de verdade) |
| `docs/` | manuais e histórico de versões, distribuídos com o jogo numa pasta `docs/` |
| `notes/` | as notas de projeto por área (som, papéis, mapa, partida, rede, voz, sondas, infra, traduções). **Não** vai para o jogador — é `docs/` que vai |
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
