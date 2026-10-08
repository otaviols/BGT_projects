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
de Linux, Mac e Android) e `C:\nvgt` no PATH; `az login`; `az aks get-credentials -g
rg-fallenrealms-alpha -n aks-fallenrealms-alpha`; Docker Desktop; `docker login ghcr.io -u otaviols`
com token de `write:packages` (o do `gh auth login` NÃO tem: `gh auth refresh -s write:packages` e
`gh auth token | docker login ghcr.io -u otaviols --password-stdin`); Python 3 no PATH; identidade
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
  `docker save -o` aqui, `scp`, `docker load` lá (55 MB comprimida). Contêiner:
  `docker run -d --name amongus-server --restart unless-stopped -p 8934:8934/udp -v
  /opt/amongus/data:/data --env-file /opt/amongus/admin.env <imagem>`. Testar com
  `AMONGUS_SERVER_HOST=179.199.151.166` antes do `admin.ps1 status` e das sondas.

**O segredo `ghcr-pull` tem prazo de validade.** O pacote da imagem é privado, então o cluster precisa
de credencial. Quando o token do GitHub expirar, o servidor para de subir com um `ImagePullBackOff`
genérico que **não menciona token**. Diagnóstico e cura em `infra/README.md`.

**Uma réplica só, e não é para aumentar** — o estado das partidas vive na memória do processo.

**O `among_users.db` (contas + feedback) vive no volume `amongus-data`.** `kubectl delete -f` apaga o
volume junto.

## Operar

```
kubectl get pods -n amongus
kubectl logs -n amongus deploy/amongus-server -f
infra\admin.ps1 recados [-Depois <id>] [-Crash] [-Saida arquivo]   # recados dos jogadores
infra\admin.ps1 responder <id> "..."     # o jogador ouve dentro do jogo
infra\admin.ps1 arquivar <id> [-Ate]     # tira da caixa (desarquivar <id> desfaz)
infra\admin.ps1 aviso "..." | -Apagar    # recado fixo do saguão
infra\admin.ps1 traducoes                # traduções enviadas pelo jogo -> translations_inbox/ (e apaga do servidor)
infra\admin.ps1 status | drenar <s> | plataformas <dias>
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

**Backup do banco: `infra\admin.ps1 backup`, para `%USERPROFILE%\amongus_backups\`**, fora do
repositório (tem hash de senha), guardando os 30 mais recentes; o `release.ps1` faz um a cada
publicação, antes do push. Até 2026-10-07 não havia NENHUM - a assinatura desativada mostrou que o
banco só sairia do disco do cluster com ela ativa. A pasta é local: uma cópia fora desta máquina é
com o usuário.

**"Caí do servidor": cruze o crash.log do recado com as linhas `[queda]` do log** (`kubectl logs -n
amongus deploy/amongus-server --timestamps | Select-String queda`, desde a 0.50.9). O jogador só sabe
dizer "o servidor encerrou a conexão"; o servidor diz se foi tempo esgotado, o cliente desligando ou
sessão substituída (a mesma conta entrando em outro aparelho - o único lugar em que o servidor
derruba alguém de propósito). O horário do crash.log é o relógio do JOGADOR, no fuso dele, e o do
`--timestamps` é UTC. O log só guarda desde o último pod: um deploy apaga o histórico.


