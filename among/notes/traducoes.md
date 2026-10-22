# Traduções

De onde vem cada idioma, como uma tradução chega e o portão do deploy. Leia antes de mexer em lang/ ou promover um envio.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

## Traduções

**Fonte da verdade: github.com/otaviols/game-translations** (clone ao lado do BGT_projects, `<disco>:\git\game-translations`),
pasta `among-us/lang/`. `en_US` e `pt_BR` são EMBUTIDOS e mantidos aqui, junto do código (as chaves
novas nascem aqui); os outros idiomas são da comunidade e vivem lá. `tools\sync_translations.ps1`
faz os dois sentidos - traz os da comunidade para `lang/`, manda os embutidos para lá como referência
(commit + push automático) - e roda no começo do `build_clients.ps1`. Não edite um idioma da
comunidade em `lang/`: o próximo sync sobrescreve; edite no repositório de traduções.

**Chave NOVA: só `pt_BR` e `en_US`. Chave EXISTENTE que muda de texto: todos os idiomas** (decisões do
usuário, 2026-10-05 e 2026-10-14). Os dois embutidos são os obrigatórios, e não temos conhecimento dos
outros idiomas para mantê-los: chave nova cai no inglês nos da comunidade até ela traduzir. Mas um texto
que NÓS mudamos (erro nosso, ou sentido novo) muda em todos, na mesma chave - senão cada idioma ficaria
dizendo a coisa errada até o tradutor voltar. Pedido de revisão vai à COMUNIDADE, pelo aviso do saguão,
nunca a um tradutor pelo nome: a comunidade de cada língua se ajuda, e o peso não cai numa pessoa só. Em 2026-10-14 houve UMA exceção: os três da
comunidade foram completados de uma vez (126 chaves no espanhol e no turco, 90 no chinês), para ficar
tudo sincronizado e pedir à comunidade que revise - não é o padrão. Se um dia precisar mexer neles:
no repositório de traduções, mantendo o `language.translator` (as `language.*` são o crédito e nunca
saem), os marcadores `{...}` e a terminologia de cada tradutor (o espanhol diz "alcantarilla" e
"asesinato", o turco "kurucu" e "Havalandırma", o chinês "通风口" e "主持人"). E **não crie chave nova só
para não mexer nas traduções**: texto que muda de sentido fica na mesma chave (foi o erro de 2026-10-14).

**Como uma tradução chega:** o jogador manda pelo jogo ("Enviar uma tradução", na lista de partidas)
-> fica no banco do servidor, UMA por (usuário, idioma), reenvio substitui -> `infra\admin.ps1 traducoes`
traz para `translations_inbox/` e APAGA do servidor -> `python tools/check_translation.py <arquivo>`
diz o que falta/sobra -> copiar para `<clone>\among-us\lang\<código>.json` (o clone fica ao lado do BGT_projects: `C:\git\game-translations` ou `D:\git\...`),
commit, push -> responder ao jogador com `admin.ps1 responder` se ele mandou recado -> o próximo build
traz. O `build_clients.ps1` recolhe a caixa de entrada sozinho e PARA se houver algo para revisar
(`-SkipInbox` pula). Pull request no repositório também serve para quem sabe usar GitHub.

**Idiomas da comunidade chegam PELO SITE, sem versão nova do jogo (1.1, pedido de jogadores).** Depois de
promover no repositório de traduções, `tools\publish_translations.ps1` sincroniza, confere e sobe os
idiomas para `https://amongus.blindtabern.com/idiomas/` (arquivos + `revisions.list`), trocando a pasta
inteira de uma vez - não compila nada nem toca no servidor do jogo. O jogo, ao abrir (logo depois do
updater), lê o `revisions.list` do site e baixa o idioma com revisão MAIOR que a dele para a pasta de
dados do jogador (`DIRECTORY_PREFERENCES`, gravável também no Android); o i18n usa o baixado só se for
mais novo que o embutido, e cai no embutido se o baixado não abrir. A REVISÃO é a hora unix do último
commit do idioma no repositório de traduções - igual em qualquer máquina - e o `sync_translations.ps1` a
grava em `lang/revisions.list`, que vai DENTRO do jogo (e é rastreado: mudou, commite). Ela fica fora do
JSON de propósito: seria uma chave que o inglês não tem, e a conferência a acusaria como sobra.
`pt_BR`/`en_US` nunca vêm pelo site: nascem com o código. O deploy publica os idiomas junto do site, para
o site nunca ficar atrás do jogo. Sonda: `probe_lang_download` (com o argumento `site`, baixa do ar).

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
ar, e por isso ele **recolhe do servidor junto** (`admin.ps1 traducoes`): uma tradução que o
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

**O `tools\release.ps1` recolhe e sincroniza as traduções logo nas conferências** (também no
`-Conferir`). Antes isso só acontecia dentro do `build_clients`, no meio da publicação: a 0.50.0 parou
ali com um espanhol novo na caixa, e o sync, que muda `lang/` (rastreado), deixaria a árvore suja para
o deploy do servidor recusar. Se a conferência acusar algo, promova e commite o `lang/` atualizado antes
de publicar.

**`parse_json` LANÇA exceção em JSON malformado** (não devolve null). Já derrubou o servidor inteiro
num teste - um envio com `{` solto matou o processo. Todo `parse_json` de conteúdo que vem de fora
(rede, arquivo de idioma, version.json) fica em try/catch; ver `validate_translation_json`.


