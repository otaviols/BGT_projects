# Infraestrutura no Azure

Sobe tudo que o jogo precisa para ficar no ar: o servidor dedicado, rodando como um contêiner no
cluster AKS, e um site estático de onde os jogadores baixam o cliente e de onde o jogo pode buscar
atualizações.

| Recurso | Para quê | Custo aproximado |
|---|---|---|
| Contêiner no cluster AKS | roda o `server_main` | ~zero: aproveita um nó que já estava pago |
| IP público estático | o endereço vai compilado dentro do cliente | ~US$ 3 por mês |
| Storage Account (site estático) | download do jogo e `version.json` | centavos |

## Por que não tem uma VM

Esta era a pergunta óbvia, e a resposta tem duas metades.

**A primeira é custo.** O cluster `aks-fallenrealms-alpha` já existe nesta assinatura, já está pago e
tem folga de sobra. O servidor deste jogo só valida movimento e retransmite pacotes pequenos para no
máximo 10 jogadores — cabe no que sobra, e uma VM dedicada seria uma segunda máquina cobrada
integralmente para fazer bem menos do que o nó já faz.

**A segunda é que esta assinatura simplesmente não consegue criar a VM.** Todas as SKUs baratas
(família B, Dv3/v5, F2s_v2, A_v2) estão marcadas `NotAvailableForSubscription` — em todas as regiões
testadas, então não era questão de escolher outra região. As únicas SKUs sem restrição em
`brazilsouth` são as de computação confidencial (DCasv6/ECasv6), e essas têm **cota zero**:

```
az vm list-usage --location brazilsouth   # Standard DCasv6 Family vCPUs -> 0 / 0
```

Isso não aparece em `az vm list-skus` (lá elas constam como disponíveis), e era exatamente o que
fazia o `terraform apply` falhar sempre no mesmo ponto: rede, IP, NSG e storage criados, e só a VM
faltando. O AKS escapa da restrição porque cria os nós por outro caminho (VMSS gerenciado) — o nó
atual é justamente uma `D2als_v6`, que a criação direta de VM recusa.

O histórico do git tem a versão anterior deste diretório, com a VM, o `cloud-init` e o serviço
systemd, caso um dia valha a pena voltar atrás.

## Antes de começar

1. **Terraform**, **az CLI**, **kubectl** e **Docker** instalados (já estão nesta máquina).
2. `az login` e anote a assinatura: `az account show --query id -o tsv`.
3. Credencial do cluster: `az aks get-credentials -g rg-fallenrealms-alpha -n aks-fallenrealms-alpha`.
4. Login no registro de imagens: `docker login ghcr.io -u otaviols` (usa um token do GitHub com
   `write:packages` — `repo` sozinho **não** basta, e o `docker login` passa mesmo assim: quem
   recusa é o `push`, com "does not match expected scopes").
5. Credencial de leitura do registro DENTRO do cluster, uma vez só (ver "O segredo do registro"
   abaixo):

   ```
   kubectl create secret docker-registry ghcr-pull \
     --docker-server=ghcr.io --docker-username=otaviols \
     --docker-password=<token com read:packages> -n amongus
   ```

Não há mais chave SSH nem IP de administrador para configurar: o servidor deixou de ser uma máquina
alcançada por SSH. Uma coisa a menos para vazar, e uma a menos para atualizar toda vez que o seu IP
residencial muda.

## Subir a infra

Crie o arquivo `terraform.tfvars` nesta pasta:

```hcl
subscription_id      = "00000000-0000-0000-0000-000000000000"
storage_account_name = "amongusaudiogame"
```

Depois:

```
terraform init
terraform plan
terraform apply
kubectl apply -f k8s/amongus.yaml
```

O Terraform cria o IP, o site e a permissão do cluster; o `kubectl apply` cria o servidor. No fim o
Terraform imprime o IP do servidor, o endereço do site e o passo a passo do que fazer em seguida.

`storage_account_name` precisa ser único no Azure inteiro (é o que vira o endereço do site), só
minúsculas e números.

## Publicar uma versão

Da raiz do projeto (`among`), com o IP do servidor já dentro de `DEFAULT_SERVER_HOST` em
`src/config/game_constants.nvgt`:

```
nvgt tools/build_pack.nvgt          # regera o sounds.dat
nvgt -c -plinux server_main.nvgt    # servidor para o contêiner
nvgt -c AmongUs.nvgt                # cliente para os jogadores
infra\deploy.ps1 -StorageAccount <nome>
```

O script constrói a imagem, publica no registro, atualiza o servidor no cluster e espera ele subir;
e manda o cliente e o site para o Storage. `-SkipServer` ou `-SkipSite` publicam só uma das metades.

**O banco de contas não é tocado pelo deploy.** O `among_users.db` fica no volume `amongus-data`, que
é montado em `/data` e não faz parte da imagem — as contas dos jogadores sobrevivem a qualquer
atualização.

## Operar o servidor

```
kubectl get pods -n amongus                          # está no ar?
kubectl logs -n amongus deploy/amongus-server -f     # log ao vivo
kubectl rollout restart -n amongus deploy/amongus-server
```

O pod reinicia sozinho se cair — é o mesmo papel que o `Restart=always` do systemd fazia na VM.

## Ler os recados dos jogadores

O jogo tem uma opção de mandar recado para quem o faz (no navegador de partidas). Os recados chegam
pelo próprio servidor, autenticados, e ficam na tabela `feedback` do mesmo SQLite das contas — no
volume, então sobrevivem a deploy.

```
infra\read_feedback.ps1                 # os 20 mais recentes
infra\read_feedback.ps1 -Limit 100
infra\read_feedback.ps1 -WithCrashLog   # inclui o crash.log anexado
```

Cada recado vem com o que o jogo sabia na hora: versão, idioma, se estava em partida, papel, sala —
e o `crash.log`, se existir. É isso que separa um relato investigável de um "travou aqui": ninguém
digita a versão que está usando, e o `crash.log` é justamente o arquivo que o jogador tem e não sabe
que existe.

Para ver um recado chegando ao vivo, sem abrir o banco:

```
kubectl logs -n amongus deploy/amongus-server -f
```

Cada envio bem-sucedido também sai no log como `[feedback] usuario: texto`.

## Detalhes que não são óbvios

**A porta do jogo é UDP.** O ENet não usa TCP. Expor só TCP é o erro clássico: qualquer teste de
porta diz que está tudo certo e mesmo assim ninguém conecta.

**O IP é estático e mora FORA do cluster.** Ele vai compilado dentro do cliente de todo jogador; se
mudar, todo mundo precisa de um build novo. Por isso ele é um recurso do Terraform, e não um IP
sorteado pelo balanceador: assim ele sobrevive a apagar o Service, recriar o balanceador ou até
trocar de cluster. Quem testa contra outro servidor não precisa recompilar nada — basta um
`server.txt` ao lado do jogo (ver `src/config/server_address.nvgt`).

**O IP está num grupo de recursos diferente do cluster**, e por isso o Service precisa das duas
anotações `azure-pip-name` e `azure-load-balancer-resource-group`, mais a função *Network
Contributor* que o `main.tf` concede ao cluster. Sem a permissão, o Service fica eternamente em
`pending`: o balanceador tenta pegar o IP, recebe um 403 e continua tentando, sem nada no
`kubectl get svc` que explique o motivo.

**Uma réplica só, e não é para aumentar.** O estado das partidas vive na memória do processo
(`src/core/game_state.nvgt`). Uma segunda réplica não dividiria a carga — criaria um segundo servidor
com outras partidas, e quem entrasse pelo balanceador cairia num dos dois ao acaso.

**O deploy usa `Recreate`, não `RollingUpdate`.** O volume é `ReadWriteOnce` e não aceita dois pods
ao mesmo tempo; com a estratégia padrão, o pod novo ficaria travado esperando um volume que o pod
velho ainda segura. São alguns segundos fora do ar a cada deploy.

**As bibliotecas do NVGT ficam em `/opt/amongus/lib`** e o Linux não procura nelas sozinho — quem
resolve é o `LD_LIBRARY_PATH` na imagem. Sem essa linha o servidor morre no start sem explicação
decente.

**O segredo do registro (`ghcr-pull`) tem prazo de validade.** O pacote `amongus-server` é privado
no GHCR, então o cluster precisa de credencial para baixá-lo — é o `imagePullSecrets` do Deployment.
Esse segredo guarda um token do GitHub, e **quando o token for rotacionado ou expirar, o servidor
para de subir**. O sintoma engana: um `ImagePullBackOff` que não menciona token nenhum. Para
confirmar que é isso:

```
kubectl describe pod -n amongus -l app=amongus-server | grep -A2 Failed
```

Um `401 Unauthorized` ao buscar o token anônimo é a assinatura do problema. A cura é recriar o
segredo com um token novo. Se um dia o pacote virar público (como o do outro jogo neste mesmo
cluster), o segredo e o bloco `imagePullSecrets` podem sumir, e não há mais nada para manter.

**A imagem é etiquetada com o commit, nunca `latest`.** Além de dar para saber que código está no ar
olhando o pod, o Kubernetes só reinicia o servidor quando a tag muda — com `latest` fixo, o
`kubectl apply` não veria diferença nenhuma e o deploy não faria nada.

**Ser dono da assinatura não basta para enviar arquivos ao Storage.** No Azure, mandar em um recurso
e mandar nos dados dentro dele são permissões diferentes — sem a função *Storage Blob Data
Contributor*, o primeiro `deploy.ps1` falharia com um 403 difícil de entender. O Terraform já
concede essa função a quem roda o `apply`. Se outra pessoa for publicar versões, ela precisa da
mesma função na conta de storage.

## Derrubar tudo

```
kubectl delete -f k8s/amongus.yaml
terraform destroy
```

O `kubectl delete` apaga o servidor **e o volume com o banco de contas**. Se quiser guardá-lo antes:

```
kubectl cp -n amongus <nome-do-pod>:/data/among_users.db among_users.db
```

O `terraform destroy` apaga o IP e o site. Ele **não** toca no cluster: o AKS não é criado aqui, é
compartilhado com outro jogo, e derrubá-lo levaria o outro junto.
