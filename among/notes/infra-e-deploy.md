# Infra e deploy

Publicar, Docker, Kubernetes, banco e ferramentas de administração. Leia antes de publicar ou mexer na infra.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## Deploy, infraestrutura e ferramentas

**"Não consegui extrair server_main.zip" no deploy = servidor compilado SEM `-plinux`.** Para
conferir que o servidor compila, é tentador rodar `nvgt -c server_main.nvgt` - isso gera um
`server_main.zip` WINDOWS, mais novo que o `server_main.tar.gz` Linux, e o `deploy.ps1` escolhe
o pacote mais novo. Ele falha na extração por sorte (o zip não tem o binário `server_main`); se
não falhasse, publicaria um executável Windows no contêiner. Apague o zip e gere com
`nvgt -c -plinux server_main.nvgt`. Para só conferir que compila, use `-plinux` também.

**"docker build falhou" logo no começo do deploy = Docker Desktop parado** (a máquina reiniciou).
`Start-Process "C:\Program Files\Docker\Docker\Docker Desktop.exe"` e esperar `docker info`
responder; o deploy não inicia o Docker sozinho.

**Mesmo sintoma com o Docker DE PÉ (`NativeCommandError` em `#0 building with "desktop-linux"`) =
o deploy foi chamado com a saída redirecionada** (`2>&1`, `*>&1`, `| Tee-Object`). No PowerShell 5.1
isso transforma o progresso que o `docker build` escreve na saída de erro em registro de erro, e o
`$ErrorActionPreference = "Stop"` do script o trata como falha. Rode `infra\deploy.ps1` sem
redirecionar nada. Nada é publicado nesse caso - ele morre antes do push.

**Terceiro jeito do mesmo "docker build falhou": `unable to resolve docker endpoint: context
"desktop-linux": open ...\.docker\contexts\meta\...\meta.json: The process cannot access the file`.**
Um arquivo de configuração do Docker travado por outro processo, passageiro (0.50.7). Confira `docker
version` e rode o `tools\release.ps1` de novo: ele refaz as conferências e as compilações, e o push, que
já tinha saído, não tem o que mandar. Nesse caso o GitHub fica à frente do que está no ar por alguns
minutos, o que não afeta jogador nenhum.

**O que o `tools\release.ps1` decide sozinho, e por quê** (cada um já custou uma publicação):
- **servidor só quando o código dele muda** desde o commit no ar. Ficam de fora: `src/ui`, `src/audio`,
  `src/game/match`, `game_loop`/`spectate_loop`, `lang/` (o servidor manda CHAVES; quem traduz é o
  cliente - incluí-lo reiniciou o servidor à toa na 0.50.6) e um `game_constants` que só mudou o
  `GAME_VERSION`. Se o servidor voltar a montar texto com `tr()`, `lang/` volta para o filtro;
- **o `sounds.dat` é conferido em TODA publicação** (`tools/check_pack.nvgt`), e não só regerado quando
  um som muda: o publicado da 0.50.0 à 0.50.2 tinha um byte trocado que deixava um passo mudo (ver
  notes/som.md);
- **as traduções enviadas pelo jogo param a publicação** logo nas conferências (ver notes/traducoes.md).

**"O servidor NÃO fica de pé nesta imagem" com o log `exec /usr/local/bin/docker-entrypoint.sh: no
such file or directory` = o script shell saiu com CRLF.** O arquivo está lá; quem não existe é o
`/bin/sh\r` do shebang. Acontece em máquina com `core.autocrlf=true` (padrão do Git para Windows) — foi
descoberto ao preparar um segundo computador para publicar. O `.gitattributes` da pasta força `eol=lf`
em `*.sh`; se aparecer outro script que vai para o contêiner com outra extensão, ele entra lá também.

**Máquina nova para publicar** (o que cada ferramenta precisa, na ordem em que falha): NVGT da release
`latest` do GitHub `samtupy/nvgt` (instalador Inno Setup em `C:\nvgt`, `/VERYSILENT` já traz os stubs
de Linux, Mac e Android) e `C:\nvgt` no PATH; `az login` (só para o site antigo, no Azure); Docker
Desktop; a chave `~/.ssh/amongus_vps` (ver "VPS na Hostinger") e a VPS no `known_hosts` pelo NOME;
Python 3 no PATH; identidade
do git (`user.name`/`user.email`), sem a qual nenhum commit sai; a chave de assinatura do Android
(`%USERPROFILE%\.nvgt_android.keystore`, copiada da outra máquina - ver CLAUDE.md, Android).
Com `core.autocrlf=true`, o git escreve avisos de fim de linha na saída de erro, e script PowerShell
em modo `Stop` morre neles achando que o git falhou - foi o `sync_translations.ps1`. O nvgt.gg
passou a redirecionar para outro domínio - o GitHub é a fonte segura do instalador.

**Atualização que falha com "arquivo em uso por outro processo" numa DLL de `lib/`** (recado #152:
`nvdaControllerClient64.dll`). O script esperava só o EXECUTÁVEL ficar livre, e seguia calado depois
de 60 s; uma segunda janela do jogo, um antivírus ou o jogo ainda fechando seguravam a DLL. Hoje ele
espera TODOS os arquivos de `lib/` (abertos sem compartilhamento) e tenta a cópia até 8 vezes; se
ainda falhar, a mensagem pede para fechar todas as janelas do jogo - no IDIOMA do jogador, o que exige
gravar o `.ps1` com BOM UTF-8 (sem ele o PowerShell 5.1 embaralha os acentos; era por isso que a
mensagem antiga não tinha acento, e era sempre em português). Quem está numa versão ANTERIOR ao
conserto continua com o script velho na primeira atualização - o conserto vale da seguinte em diante.

**O iniciador (`launcher.nvgt`): no Windows, o `AmongUs.exe` NÃO é o jogo.** O jogo vira
`AmongUsGame.exe` na mesma pasta, e o `AmongUs.exe` é um programa pequeno que confere a atualização,
abre o jogo com `--launcher` e espera o arquivo `.jogo_abriu`, que o jogo grava logo depois de abrir a
janela. Se o jogo fechar ANTES do sinal, o iniciador oferece voltar para a versão anterior. Existe por
causa da 0.45.0: o erro estava na inicialização das globais, antes da primeira linha do jogo, e o
atualizador morria junto - quem atualizou ficou sem saída. Nenhuma proteção dentro do jogo pega isso.
- **Pelo sinal, não pelo código de saída:** o `process` do NVGT não expõe o código (`wait()` devolve 0
  para quem saiu com 7), e "fechou depressa" não distingue defeito de quem saiu do menu na hora.
- **A cópia da versão anterior** fica em `%LOCALAPPDATA%\AmongUsAudiogame\versao_anterior`, gravada
  pelo script de atualização antes de copiar por cima; o `versao.txt` vai por último e é o sinal de
  cópia completa. A volta apaga a cópia (sem laço) e grava a versão em `pular_versao.txt`, que o
  atualizador respeita até sair uma mais nova. Arquivo de texto, e não as preferências: o iniciador e
  o jogo abririam o mesmo arquivo de preferências e um sobrescreveria o outro.
- **O iniciador inclui só o atualizador** (e por ele as constantes, o i18n e a fala). Tudo que ele
  inclui pode derrubá-lo do mesmo jeito - não ponha preferências, rede ou som ali.
- **O zip do Windows é remontado pelo `build_clients.ps1`** (o NVGT compila um script por pacote): o
  `AmongUs.exe` do jogo vira `AmongUsGame.exe` e o `launcher.exe` vira `AmongUs.exe`, com o mesmo
  `lib/`. O teste de abertura do build abre pelo iniciador e exige o jogo de pé E o iniciador fechado.
- **Linux, Mac e Android não têm iniciador** - lá o jogo segue abrindo direto, como antes. E quem abrir
  o `AmongUsGame.exe` direto também: o jogo só pula a própria conferência quando vem do iniciador.
- **A rede só vale a partir da segunda atualização:** a primeira para a versão com iniciador ainda é
  feita pelo script antigo, que não guarda cópia.
- Testar a falha sem esperar um defeito de verdade: compile um `.nvgt` com
  `int x = quebra();` numa global que lança, ponha o exe como `AmongUsGame.exe` numa cópia do pacote e
  abra o `AmongUs.exe` com `LOCALAPPDATA` apontando para uma pasta de mentira - o iniciador tem que
  ficar aberto mostrando o aviso.

**Sonda do atualizador:** `tools/probes/probe_updater_script.nvgt` roda o script de verdade numa
instalação de MENTIRA (sandbox em `%TEMP%` com um `hostname.exe` fazendo o papel do jogo, pacote
servido por `python -m http.server`, e outro processo segurando a DLL por 8 s). Montagem: pasta
`install\` (hostname.exe como `AmongUs.exe` + `lib\nvdaControllerClient64.dll` com o texto "velho"),
o mesmo com "novo" compactado em `srv\update.zip`, `python -m http.server 8765 --directory srv`, um
`powershell -Command` que abre a DLL com `[IO.File]::Open(..., 'Open', 'Read', 'None')` e dorme 8 s,
e então `nvgt tools/probes/probe_updater_script.nvgt <sandbox> http://127.0.0.1:8765/update.zip` -
a DLL tem que terminar "novo", a cópia em `<sandbox>\backup` com "velho", e depois da volta a DLL
"velho" de novo. Ponha também um `AmongUsGame.exe` na instalação e no pacote. **Entre duas rodadas,
pare o servidor pela linha de comando** (`Get-CimInstance Win32_Process` com `http.server`):
`Stop-Process` no `python` que o `Start-Process` devolveu pode deixar o servidor de verdade vivo, e a
rodada seguinte esperou 90 s por arquivo e falhou sem motivo aparente. NUNCA chame `launch_updater_script` rodando do fonte: o
"executável do jogo" é o `nvgt.exe`, e a cópia iria para dentro da instalação do NVGT - por isso o
texto do script saiu para `build_updater_script`, que recebe os caminhos.

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

**`bind_text(i, texto, false)` NÃO copia o texto: passar uma string montada na chamada é passar lixo.**
O `false` diz ao SQLite que a string continua viva até o `step()`, e uma temporária
(`bind_text(1, "-" + days + " days", false)`) já morreu. Sintoma: a consulta roda sem erro e volta
VAZIA - foi a contagem de plataformas, com três logins gravados na tabela. Monte o texto numa variável
local antes do bind (todos os outros binds do arquivo já passavam variáveis, e por isso nunca tinham
mordido).

**De onde se joga: `infra\admin.ps1 plataformas [dias]`.** Jogadores DISTINTOS por
plataforma e por versão no período (padrão 30 dias), da tabela `client_logins` - uma linha por
jogador por dia, a última do dia vale. O `status` diz também quem está online agora por plataforma.
"unknown" é cliente anterior à 0.43.0, que não dizia de onde vinha, ou um cliente que mandou algo
fora da lista (o servidor não grava o texto que chega). Apagar a conta apaga as linhas dela. Sonda:
`probe_platforms.nvgt`.

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
`admin.ps1 <comando> -Local`. A variável `AMONGUS_SERVER_HOST` faz qualquer ferramenta ou sonda
apontar para outro servidor sem criar `server.txt` (que o jogo de verdade também leria).

**Ferramenta que inclui só o cliente precisa dos `#include` certos.** `tools/reply_feedback` (hoje parte do `tools/server_admin`) não
compilava: `protocol.nvgt` usa `tr()` e `game_constants.nvgt` usa `DEFAULT_LANGUAGE`, e os dois
vinham por ordem de inclusão. Agora os dois incluem `i18n.nvgt`. Sintoma: "No matching symbol 'tr'"
numa ferramenta nova, com o jogo compilando normalmente.

**`admin.ps1 recados` mostra os PENDENTES; `-Todos` traz os arquivados junto.** O padrão já
foi "os 20 mais recentes", e isso pareceu truncamento - os recados chegam em dezenas por dia; hoje o
filtro é por estado, não por quantidade. `-Depois <id>` serve para retomar de onde parou.

**Recado tratado se ARQUIVA, não se apaga** (`infra\admin.ps1 arquivar 42`, ou `arquivar 96 -Ate`, e
`desarquivar 42` desfaz). O contexto de um recado - versão, papel, sala, crash.log - é justamente o que falta
quando o mesmo problema volta meses depois. Quem escreve no banco é o SERVIDOR, por comando de rede
(`C_ADMIN_RESOLVE_FEEDBACK`, mesmo token do responder), e nunca uma ferramenta mexendo no
arquivo por fora: ele mantém o SQLite aberto o tempo todo, e duas mãos no mesmo arquivo é pedir
corrupção. A coluna entra por `ALTER TABLE` ao abrir o banco, porque o `CREATE TABLE IF NOT EXISTS`
não roda num banco que já existe - e o leitor (`tools/read_feedback.py`) tolera os dois formatos, já que é usado antes
e depois do deploy que acrescenta a coluna.

**`infra\admin.ps1` é a ÚNICA porta da administração** (eram cinco scripts e quatro ferramentas
quase iguais). Comando novo: o lado de rede vai em `tools/server_admin.nvgt` (um `cmd_*`), o lado
PowerShell é um `case` no `switch` do `admin.ps1`. Leitura de recados vai por cópia do banco
(`tools/read_feedback.py`), e escrita SEMPRE por comando ao servidor. **Native com stderr sob
`$ErrorActionPreference = "Stop"` morre no PowerShell 5.1** (o aviso "Removing leading '/'" do
`kubectl cp` virava erro fatal) - é por isso que o leitor antigo não funcionava nesta máquina; o
`admin.ps1` baixa para `Continue` só em volta do `kubectl cp` e confere se o arquivo chegou.

**Script `.ps1` com acento PRECISA de BOM UTF-8.** Sem ele o PowerShell 5.1 lê o arquivo na página
de código do sistema e toda mensagem com acento sai "versÃ£o", "nÃ£o" - todos os nossos scripts
estavam assim, e ninguém lia as mensagens de erro direito. Todos ganharam o BOM; script novo,
também (o editor do VS Code: "Save with Encoding" > "UTF-8 with BOM").

**Negrito no MEIO de um parágrafo do changelog vai cru para o aviso de atualização.** O
`make_version_json.py` só tira o `**` que abre o parágrafo; um `**destaque**` no meio da frase chega ao
jogador como "asterisco asterisco" lido pelo leitor de tela. No changelog, negrito só no começo do
parágrafo (é o título dele); depois de gerar, confira que o `version.json` não tem `*`.

**Fora do git:** `terraform.tfvars`, `*.tfstate`, `sounds.dat`, `*.zip`, `*.exe`, `crash.log`,
`among_users.db`, `server.txt`.


## Infra — o que não é óbvio

**Servidor de TESTE na VPS (2026-10-19): `infra\test_server.ps1 subir|status|desligar`.** Um segundo
contêiner (`amongus-test`), porta UDP **8935**, banco PRÓPRIO em `/opt/amongus/test-data` - as contas de
lá não são as do jogo, e o oficial (`amongus-server`, 8934) não é tocado. Aceita os mapas em construção (o
`mapas_em_teste.txt` que o script põe lá) e manda e-mail com o `email.json` do servidor de teste local,
enviado direto à VPS com `"required": false`. O jogo de teste aponta para ele com um `server.txt`
("amongus.blindtabern.com:8935"); o pacote para os amigos do usuário fica em
`Documents\Among Us - teste com amigos.zip`, com o `server.txt` e o `mapas_em_teste.txt` dentro. Imagem
pelo commit, como no deploy, e com o mesmo teste de "sobe?". Armadilha que ele já pegou: o
`Set-Content -Encoding utf8` do PowerShell 5.1 grava o BOM, e o `parse_json` do NVGT recusa - o e-mail
subiu desligado ("nao e um JSON valido" no log). O script grava sem BOM, e o servidor ignora a marca.

**Desde 2026-10-09 o servidor e o site moram na VPS da Hostinger** (ver "VPS na Hostinger", abaixo), e
o que fala com ela está em `infra/vps.ps1`. **O Azure foi apagado pelo usuário no mesmo dia**: o
storage do site antigo e os grupos de recursos, cluster incluído (o activity log mostra os
`storageAccounts/delete`). O banco de lá ficou no backup `among_users_2026-10-09_105901_final_azure.db`.
Quem ficou numa versão até a 0.51.0 procurava atualização no site do Azure, então não é mais avisado:
precisa baixar o jogo de novo em amongus.blindtabern.com. Sintoma, se algum script ainda tentar o
Azure: "Failed to resolve 'amongusaudiogame.blob.core.windows.net'" e "Storage account not found" - foi
assim que a 0.51.7 descobriu. O que vem abaixo sobre AKS, `kubectl`, `ghcr-pull` e a assinatura é
histórico.

**Não há VM.** Esta assinatura Azure não consegue criar nenhuma SKU barata
(`NotAvailableForSubscription` em todas as regiões), e as sem restrição têm **cota zero** — o que não
aparece em `az vm list-skus` e fazia o `terraform apply` falhar sempre no mesmo ponto. O servidor roda
no cluster AKS `aks-fallenrealms-alpha`, compartilhado com outro jogo, a custo marginal ~zero.

**O cliente conecta pelo nome `amongus-server.blindtabern.com`** (desde a 0.51.1; `DEFAULT_SERVER_HOST`),
com o IP em `FALLBACK_SERVER_IP` só se o nome não resolver - o `connect_to` falha NA HORA com nome
inválido, então a reserva não custa espera. O domínio é do usuário, com o DNS na Hostinger, e o MCP
`hostinger` (escopo local do projeto) edita a zona: `dns_records_list` / `dns_records_update` com
`overwrite: false` para acrescentar. Mudar o servidor de lugar = trocar o registro A E o
`FALLBACK_SERVER_IP` na mesma versão. Até a 0.51.0 o IP `20.206.112.223` ia compilado no cliente:
mantenha-o respondendo até quase todos atualizarem. O redirecionamento de domínio da Hostinger NÃO
serve para o site: é do domínio inteiro, para um endereço fixo, sem manter o caminho.

**VPS na Hostinger (2026-10-08):** id 2045786, plano "Game Panel 2" (2 CPU, 8 GB, 100 GB), Campinas,
IP `179.199.151.166`, 6 ms daqui (o mesmo que o Azure). É o destino do servidor e do site, para sair
do Azure. O que custou descobrir:
- **O plano "Game Panel" NÃO deixa trocar o sistema** (no hPanel nem pela API): vem "Debian 13 with
  Game Panel" (o AMP, da CubeCoders). Serve igual - o Docker 29 já vem instalado - e o painel fica
  (decisão do usuário). Ele ocupa 2223/TCP, 12820/UDP, 8080 (só local) e um nginx em 80/443; a 8934
  está livre, e o site do jogo entra nesse mesmo nginx com um `server_name` próprio. Firewall: política
  de ENTRADA aceita tudo, e não há grupo de firewall da Hostinger na VM.
- **Chave SSH: `~/.ssh/amongus_vps`, sem senha, só para esta VPS** (`ssh -i ... -o IdentitiesOnly=yes
  root@179.199.151.166`). A `id_ed25519` desta máquina tem senha que ninguém lembra e o `ssh-agent`
  está desligado - o sintoma foi "Server accepts key" seguido de "Permission denied", que parece chave
  errada e não é. Chave cadastrada pela API (`vps_public-keys_attach`) com a VM já criada NÃO chegou
  ao `authorized_keys`; a que o usuário pôs pelo painel chegou.
- **Montagem:** `/opt/amongus/data` (dono 10001, o usuário da imagem) com o `among_users.db`,
  `/opt/amongus/admin.env` (modo 600, `AMONGUS_ADMIN_TOKEN=...`, o mesmo token do segredo do
  cluster - mandado pelo PowerShell ganha um `\r` no fim; o `sed` tira). A imagem vai sem registro:
  `docker save -o` aqui, `scp`, `docker load` lá - o `deploy.ps1` faz isso, com a imagem chamada
  `amongus-server:<commit>`. Contêiner:
  `docker run -d --name amongus-server --restart unless-stopped -p 8934:8934/udp -v
  /opt/amongus/data:/data --env-file /opt/amongus/admin.env <imagem>`. As imagens antigas ficam lá:
  voltar atrás é esse mesmo `docker run` com a etiqueta anterior (`docker images amongus-server`).
  Para falar com um servidor da VPS sem passar pelo DNS: `AMONGUS_SERVER_HOST=179.199.151.166`.
- **A troca (2026-10-09), na ordem que funcionou, com ~5 min fora do ar:** compilar e testar o servidor
  ANTES de tocar em nada; drenar o Azure; backup final (`kubectl cp`); `--replicas=0` no Azure; na VPS,
  tirar o contêiner de teste, guardar o banco de teste em `/opt/amongus/backups/` e pôr o backup final
  no lugar (dono 10001; o `sha256sum` de lá tem que bater com o daqui); `deploy.ps1 -SkipSite
  -DrainSeconds 0 -SkipTranslations` com `AMONGUS_SERVER_HOST` no IP (a conferência de traduções
  precisa de um servidor que ainda não existe); conferir com `admin.ps1 recados` que o banco é o de
  verdade (o último recado está lá); registro A (`overwrite: true`, só com aquele nome - o
  `dns_records_validate` antes); sonda pelo nome. O `release.ps1` viu o servidor já no commit dele e
  publicou só os clientes. O DNS da Hostinger propagou em segundos (TTL 300).
- **O site `amongus.blindtabern.com` mora na VPS** (desde a 0.51.1, que consulta o `version.json`
  lá): registro A na Hostinger, `/etc/nginx/conf.d/amongus.blindtabern.com.conf` servindo
  `/var/www/amongus`, certificado do Let's Encrypt pelo `certbot --nginx` (a renovação é o
  `certbot.timer` que o painel já tinha). É o único site desde que o do Azure foi apagado
  (2026-10-09), e o `release.ps1` confere no fim que ele anuncia a versão nova. Cada arquivo sobe como
  `.nome.novo` e é renomeado no fim, com o `version.json` por último.
- **Só a PÁGINA do site, sem publicar versão:** o `deploy.ps1` sobe junto o `AmongUs.zip` que estiver
  na pasta - e entre duas versões ele é o código ainda NÃO publicado. Para mexer só no `index.html`:
  `. .\infra\vps.ps1`, `scp` dele como `.index.html.novo` e `mv` no lugar (feito em 2026-10-16). A
  página lê as novidades de `notes_by_language` no `version.json`; até ali ela lia um `notes` que não
  existia mais, e a linha de novidades simplesmente não aparecia. Orientação na página (Android, onde
  fica o manual, o que tem no menu) envelhece calada: ao mudar algo que ela descreve, olhe a página.
- **"scp: Connection closed" no envio para a VPS = o NOME não está no `known_hosts`.** A chave da
  máquina foi aceita para o IP (primeiro SSH), e o deploy usa o nome `amongus.blindtabern.com`; com
  `BatchMode` o ssh recusa calado (pelo ssh direto aparece "Host key verification failed"). O
  `ssh-keyscan` deste Windows não devolve nada; a entrada do nome foi feita copiando a linha
  ed25519 do IP, depois de conferir que o nome resolve para ele. Ao trocar de VPS, isto volta.
  A 0.51.1 parou aqui: o site do Azure já tinha ido, o da VPS não, e o `release.ps1` não roda de
  novo com a versão já anunciada - o que falta sai com `infra\deploy.ps1 -SkipServer`.
- **Texto mandado pelo PowerShell por pipe chega com BOM e `\r`.** Sintoma: `nginx -t` recusa com
  `unknown directive "﻿#"` na linha 1 de um arquivo que parece certo. Tire com
  `sed -i '1s/^\xEF\xBB\xBF//; s/\r$//'`. O nginx recusar um `reload` NÃO o derruba - a configuração
  antiga continua rodando (o painel nem piscou).

**O segredo `ghcr-pull` tem prazo de validade.** O pacote da imagem é privado, então o cluster precisa
de credencial. Quando o token do GitHub expirar, o servidor para de subir com um `ImagePullBackOff`
genérico que **não menciona token**. Diagnóstico e cura em `infra/README.md`.

**Uma réplica só, e não é para aumentar** — o estado das partidas vive na memória do processo.

**O `among_users.db` (contas + feedback) vive no volume `amongus-data`.** `kubectl delete -f` apaga o
volume junto.

## Operar

**Aspas duplas somem dos textos do `admin.ps1`** (aviso, responder). O Windows PowerShell 5.1 as come ao
passar o argumento ao `nvgt`, e nem `\"` salva - o aviso da 1.0 (2026-10-20) saiu "the Support the game
menu", sem as aspas. Escreva os textos sem aspas duplas (ou com apóstrofo).

```
ssh -i ~/.ssh/amongus_vps -o IdentitiesOnly=yes root@amongus.blindtabern.com "docker ps; docker logs -f amongus-server"
infra\admin.ps1 recados [-Depois <id>] [-Crash] [-Saida arquivo]   # recados dos jogadores
infra\admin.ps1 responder <id> "..."     # o jogador ouve dentro do jogo
infra\admin.ps1 arquivar <id> [-Ate]     # tira da caixa (desarquivar <id> desfaz)
infra\admin.ps1 aviso "..." | -Apagar    # recado fixo do saguão
infra\admin.ps1 traducoes                # traduções enviadas pelo jogo -> translations_inbox/ (e apaga do servidor)
infra\admin.ps1 status | drenar <s> | plataformas <dias>
infra\admin.ps1 senha <conta> <senha nova>   # quem esqueceu a senha (o jogo não guarda e-mail); a conta sai de onde estiver
```

**`kubectl`: "lookup ...azmk8s.io: no such host" NÃO é a internet daqui - é o cluster PARADO.** O
endereço do AKS deixa de existir quando ele é desalocado. Em 2026-10-07 foi a assinatura do Visual
Studio inteira desativada pela Microsoft ("Disable subscription", caller "Microsoft operator", no
`az monitor activity-log list`): o crédito do mês acabou. Cai TUDO junto - servidor e site (o
`version.json` dá 404). Confira com `az account list --all` (estado `Disabled`) e `az aks list`
(`Deallocated`); o `az account show` comum ainda mostra `Enabled` do cache.

**Reativar a assinatura NÃO religa o cluster** - e ele fica em `Deallocated` + `Failed`. O `az aks
start` termina calado sem fazer nada (e, de novo, recusa: "only allowed on a stopped cluster"); o
que religa é `az aks update -g rg-fallenrealms-alpha -n aks-fallenrealms-alpha --yes` (sem o `--yes`
ele para numa pergunta e, sem terminal, morre com `EOFError`). ~5 min, e o pod sobe sozinho com o
mesmo IP e o volume intacto. O site volta sozinho com a assinatura.

**`admin.ps1 plataformas` conta desde a troca para a VPS (2026-10-09, ~14h UTC).** A tabela
`client_logins` (uma linha por conta, por dia) foi zerada ali para medir a adesão ao servidor novo; o
histórico do Azure está no backup `among_users_2026-10-09_105901_final_azure.db` (aqui e em
`/opt/amongus/backups/` na VPS). **Sonda rodada contra o servidor de verdade entra na contagem**: as
contas dela (`nb0x...`, `pc...`) logam como Windows na versão do código. Na troca, quatro delas eram
quase metade dos logins da 0.51.5 - foram tiradas à mão.

**Sala travada: `infra\admin.ps1 salas` lista (id, estado, modo, mapa, quem joga, bots, quem assiste) e
`infra\admin.ps1 fechar <id>` fecha sem derrubar o servidor** (2026-10-22). Fechar só vale SEM ninguém
jogando - recusa dizendo quem está lá: o cliente só sabe sair de uma sala fechada estando na sala de
espera, e quem estivesse no meio da partida ficaria preso numa partida que não existe mais. Quem só
assistia volta à lista (`S_KICKED`, tratado na tela de assistir). A causa de sala presa que motivou o
comando (partida só com bots depois de quem assistia sair por último) foi corrigida no mesmo dia: o
servidor confere a cada volta (`lobby_has_people`). Sonda: `probe_admin_lobbies`.

**Backup do banco: `infra\admin.ps1 backup`, para `%USERPROFILE%\amongus_backups\`**, fora do
repositório (tem hash de senha), guardando os 30 mais recentes; o `release.ps1` faz um a cada
publicação, antes do push. Até 2026-10-07 não havia NENHUM - a assinatura desativada mostrou que o
banco só sairia do disco do cluster com ela ativa. A pasta é local: uma cópia fora desta máquina é
com o usuário.

**"Caí do servidor": cruze o crash.log do recado com as linhas `[queda]` do log** (na VPS,
`docker logs --timestamps amongus-server 2>&1 | grep queda`, desde a 0.50.9). O jogador só sabe
dizer "o servidor encerrou a conexão"; o servidor diz se foi tempo esgotado, o cliente desligando ou
sessão substituída (a mesma conta entrando em outro aparelho - o único lugar em que o servidor
derruba alguém de propósito). O horário do crash.log é o relógio do JOGADOR, no fuso dele, e o do
`--timestamps` é UTC. O log só guarda desde o último contêiner: um deploy apaga o histórico.

**Até a 0.51.10, "o cliente desligou" e "o servidor encerrou a conexão" MENTIAM: quase sempre eram
tempo esgotado.** O NVGT entrega o `DISCONNECT_TIMEOUT` do ENet como um `DISCONNECT` comum, a menos que
`network.receive_timeout_event` esteja ligado (`src/network.cpp` do NVGT) - e não estava. Sintoma (levas
de 2026-10-12, sala inteira caindo junta): o servidor dizia que os clientes desligaram, e o crash.log de
cada um dizia que o servidor encerrou - cada lado culpando o outro. Hoje os dois lados o ligam, e a queda
por silêncio sai como "tempo esgotado" (o cliente leva ~31 s para desistir, o servidor também). Para
separar servidor de rede, o servidor registra `[pulso] conectados=N` a cada 5 min: numa leva de quedas,
cai TODO MUNDO = servidor ou VPS; cai um grupo = o caminho de rede deles... ou o buffer, abaixo.

**Salas inteiras caindo juntas na REUNIÃO = o kernel descartando pacotes do servidor (2026-10-14).** O
contador `RcvbufErrors` do `/proc/net/snmp` do contêiner passava de 16 mil: o buffer de recepção do
socket (~208 KB, `net.core.rmem_max`; menos de meio segundo de voz de uma sala cheia) transbordava
sempre que uma volta do laço do servidor demorava, e o kernel jogava fora voz, movimento E as
confirmações do ENet. Quem manda e recebe mais - a sala inteira falando, na reunião e na sala de espera,
onde todos ouvem todos - perde mais e cai por tempo esgotado, junto, enquanto as outras salas seguem e a
VPS nunca fica sem rede (o netwatch dizia "sem_rede=0"). O que ficou para pegar o culpado: `[perda]`
(o servidor confere o contador a cada 5 s e diz quantos o kernel descartou) e `[lento]` (uma volta acima
de 250 ms, com o tempo dos eventos, o das salas e o pacote mais lento - era `[trava]` acima de 1 s, e
abaixo disso já transbordava). Uma causa já tirada do caminho: o BANCO no laço - cada estatística era
uma transação, com o servidor parado esperando o disco (~2,4 ms por escrita sincronizada nesta VPS);
hoje tarefas e mortes ficam na memória e a partida inteira grava numa transação só
(`record_match_counters`, `begin_batch`). **E o banco roda em WAL com `synchronous=NORMAL`** (desde
2026-10-14): o `[lento]` mostrou um LOGIN parando o servidor por 4,3 s e um logout por 3,1 s, com 1.400
pacotes descartados - o banco tem 880 KB, nenhuma consulta é lenta; é o disco compartilhado da VPS, onde
cada gravação sincronizada levou de 4 a 25 ms medida dentro da imagem do servidor, e às vezes segundos.
No WAL a gravação não espera o disco (só o checkpoint, raro). O preço: numa queda de energia a última
gravação pode se perder, sem corromper. **Quem copia o banco copia o `-wal` junto** (`Vps-CopyDb`) e o
junta à cópia (`tools/consolidate_db.py`): só o `.db` não vê as gravações recentes - testado, a cópia só
do `.db` enxergava 0 de 5 recados gravados, a consolidada os 5. Foi esse risco que fez o WAL ser
descartado da primeira vez. E na VPS o teto do buffer
subiu (`/etc/sysctl.d/90-amongus-udp.conf`, `net.core.rmem_max = 1048576`): o ENet do NVGT pede 256 KB
(`ENET_HOST_RECEIVE_BUFFER_SIZE`), e o teto antigo (208 KB) cortava o pedido. Confira com `nsenter -t <pid
do contêiner> -n ss -uamn` - o `rb` da porta 8934 passou de 425984 para 524288. VPS nova: refaça o arquivo. A VPS também roda o `ampfirewall` (do painel
AMP) a cada 5 min; ele só mexe nas regras de entrada dele (80, 443, 2223, 12820), e as do Docker para a
8934 ficam intactas - investigado e, até aqui, inocentado.

**A rede da VPS vista de dentro: `infra\admin.ps1 rede [horas]`** (desde 2026-10-13). Um serviço na
VPS (`netwatch.service`, script `/opt/netwatch/netwatch.sh`; a fonte é `infra/netwatch/`) pinga a cada
segundo o NIC.br em São Paulo, o 1.1.1.1 e o 8.8.8.8, e grava em `/var/log/netwatch.log`: uma linha
"SEM REDE" por segundo em que NENHUM respondeu, e uma por minuto com o UDP que o contêiner do jogo
recebeu e as falhas de cada alvo. Nasceu da leva de 2026-10-13 (três jogadores, em duas salas, por
tempo esgotado no mesmo meio minuto, sem `[trava]`): numa próxima, "SEM REDE" no minuto = a VPS perdeu
a rede (chamado na Hostinger); nada = o caminho de alguns provedores. O que a instalação ensinou: **o
gateway da Hostinger não responde a ping** (o primeiro log era falha a cada segundo); **o NIC.br perde
um ping de vez em quando** (ruído - por isso a falha de um alvo só vai na conta do minuto); e **o
contador UDP da máquina fica em zero**, porque o Docker encaminha direto para o contêiner, que é outro
espaço de rede - o certo é `/proc/<pid do contêiner>/net/snmp`. Reinstalar (VPS nova): os dois arquivos
de `infra/netwatch/` por `scp`, o `sed` de BOM e `\r` (ver acima), `systemctl enable --now netwatch`.


