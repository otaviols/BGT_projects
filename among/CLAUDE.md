# Among Us Audiogame — guia do projeto

Jogo de dedução social **jogado inteiramente por som**, escrito em [NVGT](https://nvgt.gg)
(AngelScript). Cliente Windows distribuído por um site estático; servidor dedicado rodando como
contêiner num cluster AKS. Está em **beta**, com jogadores reais usando.

Tudo que o jogador percebe passa por leitor de tela e áudio posicionado — **não existe informação
visual**. Ao decidir qualquer coisa de interface, a pergunta certa é "como isso soa?", não "como isso
aparece".

## Estrutura

| Caminho | O que é |
|---|---|
| `AmongUs.nvgt` | ponto de entrada do cliente |
| `server_main.nvgt` | ponto de entrada do servidor |
| `src/` | **todo** o código: `config/`, `core/`, `game/`, `network/`, `ui/`, `database/`, `audio/`, `i18n.nvgt` |
| `lang/` | **só dados** de tradução (`pt_BR.json`, `en_US.json`) — o motor de i18n fica em `src/` |
| `sounds/` | áudio fonte; vira `sounds.dat` no build |
| `tools/` | `build_pack` (gera o `sounds.dat`), `check_sounds`, `bots` |
| `infra/` | Terraform, Dockerfile, manifests do Kubernetes, `deploy.ps1`, `read_feedback.ps1` |

`lang/` fica fora de `src/` de propósito: é lido por caminho em tempo de execução, e esse caminho
precisa ser o mesmo rodando do fonte ou do build compilado.

## Compilar

```
nvgt tools/build_pack.nvgt          # regera sounds.dat a partir de sounds/
nvgt -c AmongUs.nvgt                # cliente
nvgt -c -plinux server_main.nvgt    # servidor (Linux, para o contêiner)
```

**Mexeu em qualquer arquivo de `sounds/`? Rode o `build_pack` antes de compilar.** O jogo empacota o
`sounds.dat`, não a pasta — sem regerar, o build sai com o som antigo e nada avisa.

## Publicar uma versão

Sempre, e nesta ordem:

1. **Suba `GAME_VERSION`** em `src/config/game_constants.nvgt`.
2. **Atualize `infra/site/version.json`** com a **mesma** versão e as notas.
3. Compile o que mudou.
4. `infra\deploy.ps1 -StorageAccount amongusaudiogame` (use `-SkipServer` quando só o cliente mudou).

As duas versões **têm que bater**. O `version.json` é o que os clientes instalados comparam contra si
mesmos: se ele ficar para trás, ninguém é avisado da atualização.

**O servidor só precisa de deploy quando o código dele muda** (`src/network/server.nvgt`,
`src/core/game_state.nvgt`, protocolo, banco). Som, UI e textos são só cliente.

Para o servidor, a imagem é etiquetada `v<versão>` e o `deploy.ps1` **sobe a imagem e confere que o
servidor fica de pé antes de publicar** — ver "compilação que sai defeituosa", abaixo.

## Ao terminar uma feature

**Commite.** O usuário autorizou commits ao fim de cada trabalho concluído. A mensagem deve dizer
**por que**, não só o quê — o histórico é onde as decisões ficam explicadas.

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

**Nunca `latest` como tag de imagem.** Com tag fixa o Kubernetes não vê diferença e não reinicia nada.

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
infra\read_feedback.ps1 [-WithCrashLog]     # recados dos jogadores
```

## Pendências conhecidas

- **`sounds/ejected.wav` não existe.** Está no catálogo, o `build_pack` avisa a cada build, e o jogo
  compilado sai sem o som de alguém ser expulso na votação.
- **O campo legado `message`** nos pacotes do servidor pode sair quando ninguém mais estiver em
  versões até a 0.11.1.
- **Jogadores em versões anteriores à 0.9.x** precisam baixar manualmente uma vez: a build deles é
  anterior ao updater e nunca vai perguntar nada.
