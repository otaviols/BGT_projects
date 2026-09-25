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

## Operar

```
kubectl get pods -n amongus
kubectl logs -n amongus deploy/amongus-server -f
infra\read_feedback.ps1 [-After <id>] [-WithCrashLog] [-Out arquivo]   # recados dos jogadores
infra\reply_feedback.ps1 -Id <id> -Text "..."   # responder; o jogador ouve dentro do jogo
infra\read_translations.ps1                    # traduções enviadas pelo jogo -> translations_inbox/ (e apaga do servidor)
```


