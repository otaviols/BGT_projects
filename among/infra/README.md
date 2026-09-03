# Infraestrutura no Azure

Sobe tudo que o jogo precisa para ficar no ar: uma VM Linux rodando o servidor dedicado e um site
estático de onde os jogadores baixam o cliente e de onde o jogo pode buscar atualizações.

São dois recursos que custam dinheiro e um que custa centavos:

| Recurso | Para quê | Custo aproximado |
|---|---|---|
| VM `Standard_B1s` (Ubuntu 22.04) | roda o `server_main` | ~US$ 8 a 10 por mês |
| IP público estático | o endereço vai compilado dentro do cliente | ~US$ 3 por mês |
| Storage Account (site estático) | download do jogo e `version.json` | centavos |

## Antes de começar

1. **Terraform** e **az CLI** instalados (já estão nesta máquina).
2. Uma chave SSH: `ssh-keygen -t ed25519` — o conteúdo do arquivo `.pub` vai na variável.
3. `az login` e anote a assinatura: `az account show --query id -o tsv`.
4. Descubra o seu IP público (para liberar o SSH só para você): <https://ifconfig.me>.

## Subir a infra

Crie o arquivo `terraform.tfvars` nesta pasta:

```hcl
subscription_id      = "00000000-0000-0000-0000-000000000000"
ssh_public_key       = "ssh-ed25519 AAAA... voce@maquina"
admin_source_ip      = "189.10.20.30"
storage_account_name = "amongusaudiogame"
```

Depois:

```
terraform init
terraform plan
terraform apply
```

No fim ele imprime o IP do servidor, o endereço do site e o passo a passo do que fazer em seguida.

`storage_account_name` precisa ser único no Azure inteiro (é o que vira o endereço do site), só
minúsculas e números.

## Publicar uma versão

Da raiz do projeto (`among`), com o IP do servidor já dentro de `DEFAULT_SERVER_HOST` em
`config/game_constants.nvgt`:

```
nvgt tools/build_pack.nvgt          # regera o sounds.dat
nvgt -c -plinux server_main.nvgt    # servidor para a VM
nvgt -c AmongUs.nvgt                # cliente para os jogadores
infra\deploy.ps1 -StorageAccount <nome> -ServerIp <ip>
```

O script manda o servidor por SSH, reinicia o serviço e confere se ele voltou; e sobe o cliente e o
site para o Storage. `-SkipServer` ou `-SkipSite` publicam só uma das metades.

**O banco de contas não é tocado pelo deploy.** O `among_users.db` fica em `/opt/amongus` e não está
dentro do pacote, então as contas dos jogadores sobrevivem a qualquer atualização.

## Operar o servidor

```
ssh azureuser@<ip>
sudo systemctl status amongus     # está no ar?
sudo journalctl -u amongus -f     # log ao vivo
sudo systemctl restart amongus
```

O serviço reinicia sozinho se cair (`Restart=always`), e sobe junto com a máquina. Enquanto o
primeiro deploy não chega, ele fica tentando iniciar a cada 10 segundos e entra no ar sozinho no
minuto em que o binário aparecer.

## Detalhes que não são óbvios

**A porta do jogo é UDP.** O ENet não usa TCP. Liberar só TCP é o erro clássico: qualquer teste de
porta diz que está tudo certo e mesmo assim ninguém conecta.

**O IP é estático de propósito.** Ele vai compilado dentro do cliente de todo jogador. Se mudar,
todo mundo precisa de um build novo — por isso vale os poucos dólares por mês. Quem testa contra
outro servidor não precisa recompilar nada: basta um `server.txt` ao lado do jogo (ver
`config/server_address.nvgt`).

**As bibliotecas do NVGT ficam em `/opt/amongus/lib`** e o Linux não procura nelas sozinho — quem
resolve é o `LD_LIBRARY_PATH` no serviço systemd. Sem essa linha o servidor morre no start sem
explicação decente.

**O servidor roda como um usuário sem shell e sem senha** (`amongus`), separado do seu usuário de
administração.

**Ser dono da assinatura não basta para enviar arquivos ao Storage.** No Azure, mandar em um recurso
e mandar nos dados dentro dele são permissões diferentes — sem a função *Storage Blob Data
Contributor*, o primeiro `deploy.ps1` falharia com um 403 difícil de entender. O Terraform já
concede essa função a quem roda o `apply`. Se outra pessoa for publicar versões, ela precisa da
mesma função na conta de storage.

## Derrubar tudo

```
terraform destroy
```

Isso apaga a VM, o IP e o site — **inclusive o banco de contas**, que mora no disco da VM. Se quiser
guardá-lo antes: `scp azureuser@<ip>:/opt/amongus/among_users.db .`
