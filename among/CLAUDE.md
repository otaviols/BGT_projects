# Among Us Audiogame — guia do projeto

Jogo de dedução social **jogado inteiramente por som**, escrito em [NVGT](https://nvgt.gg)
(AngelScript). Cliente Windows distribuído por um site estático; servidor dedicado rodando como
contêiner num cluster AKS. Está em **beta**, com jogadores reais usando.

Tudo que o jogador percebe passa por leitor de tela e áudio posicionado — **não existe informação
visual**. Ao decidir qualquer coisa de interface, a pergunta certa é "como isso soa?", não "como isso
aparece".

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
| `tools/` | `build_pack` (gera o `sounds.dat`), `check_sounds`, `bots` |
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

O `version.json` carrega as notas em dois formatos: `notes` (texto único, em inglês) para os clientes
até a 0.17.0, que esperam uma string e quebrariam com um objeto, e `notes_by_language` para os novos.
O campo antigo pode sair quando não houver mais ninguém nessas versões.

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

**Texto do servidor viaja como chave de tradução, nunca como frase pronta.** Os jogadores de uma
partida podem estar em idiomas diferentes, e quem sabe o idioma de cada um é o cliente dele. Ver
`MSG_KEY_FIELD` e `tr_server_message` em `src/network/protocol.nvgt`. Os pacotes ainda levam o texto
pronto num campo à parte só para clientes até a 0.11.1 — removível quando não houver mais ninguém
nessas versões.

**Idiomas são plugáveis.** Basta pôr um `.json` em `lang/` para o idioma aparecer no jogo; o nome dele
sai da chave `language.name`, no próprio idioma. O que faltar cai no inglês. **Ao acrescentar
qualquer texto novo, acrescente a chave nos dois idiomas.**

**Nada de identidade é traduzido** — nome de jogador, nome de bot. Traduzir faria duas pessoas na
mesma partida acusarem "nomes" diferentes pela mesma pessoa.

**Valor vindo do cliente é validado no servidor.** As configurações de sala passam por
`lobby_config.validate()` depois de aplicadas: elas vêm da máquina do jogador.

**Estado que existe nos dois lados tem que ser DESFEITO nos dois lados.** O servidor derrubava o posto
de câmeras (sabotagem, reunião, morte) e avisava a partida inteira qual sala estava sob observação -
menos a única pessoa que precisava saber, a que estava observando. O cliente dela continuava na
câmera: parado, ouvindo outra sala, apertando ESC sem efeito porque para o servidor ele já tinha
saído. Sintoma: jogador travado sem mensagem nenhuma. Por isso `release_cameras_any()` devolve QUEM
foi liberado, e todo lugar que fecha o posto passa por `close_cameras()`.

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

**Cada som tem UM ouvinte pretendido — decida quem antes de tocar.** Num jogo em que a informação é o
som, tocar para todo mundo entrega de graça o que devia custar. O assassinato são dois sons com
públicos diferentes: a **morte** é pessoal e só a vítima ouve; o **kill** é espacial e é o único
sinal do crime para quem está por perto. Tocar os dois para todos, como era antes, dava o crime de
graça aos vizinhos e fazia a vítima ouvir a própria morte como se fosse de outra pessoa.

**Fantasma não é atingido por sabotagem.** Ele já perdeu o que tinha a perder, e continua fazendo
tarefas pelo time — cegá-lo não cria tensão, só torna tedioso o que ainda ajuda.

**Papel declara o que pode; o código pergunta por capacidade.** `src/game/roles/role_traits.nvgt`
define cada papel (pode matar, ventilar, sabotar, o que percebe) e `player_abilities` combina isso
com estar vivo. Nada deve voltar a perguntar "é impostor?" para decidir uma ação — era assim que a
resposta virava um booleano em mais de sessenta pontos, e cada papel novo obrigava a revisitar todos.
Um papel novo entra em três passos, descritos no topo daquele arquivo.

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

**Som novo vai na subpasta certa de `sounds/`** (`steps`, `ambience`, `beacons`, `tasks`, `events`,
`ui`, `world`) e o caminho no catálogo inclui a pasta. Depois de mexer em som, rode
`nvgt tools/check_sounds.nvgt`: ele compara o catálogo com o disco **nos dois sentidos** e é a única
coisa que pega um caminho errado — som que não carrega falha em silêncio, sem erro nenhum.

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

**`read_feedback.ps1` mostra tudo por padrão; use `-After <id>` para retomar.** O padrão era "os 20
mais recentes", e isso pareceu truncamento: os recados chegam em dezenas por dia.

**Fora do git:** `terraform.tfvars`, `*.tfstate`, `sounds.dat`, `*.zip`, `*.exe`, `crash.log`,
`among_users.db`, `server.txt`.

## Testar

Há um servidor de verdade no ar — **use-o**. O padrão que funcionou a sessão inteira: escrever um
`.nvgt` curto que conecta, faz a coisa e imprime o resultado, rodar com `nvgt arquivo.nvgt`, apagar
depois. Foi assim que se validou feedback, i18n, configurações de sala e limite de jogadores.

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
- **`sounds/ejected.ogg` não existe.** Está no catálogo, o `build_pack` avisa a cada build, e o jogo
  compilado sai sem o som de alguém ser expulso na votação.
- **O campo legado `message`** nos pacotes do servidor pode sair quando ninguém mais estiver em
  versões até a 0.11.1.
- **Jogadores em versões anteriores à 0.9.x** precisam baixar manualmente uma vez: a build deles é
  anterior ao updater e nunca vai perguntar nada.
