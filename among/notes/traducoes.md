# Traduções

De onde vem cada idioma, como uma tradução chega e o portão do deploy. Leia antes de mexer em lang/ ou promover um envio.

Parte das notas do projeto: o CLAUDE.md diz quando ler este arquivo. Vale aqui a mesma regra
dele — aprendeu algo que teria economizado tempo, escreva aqui, na mesma sessão.

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


