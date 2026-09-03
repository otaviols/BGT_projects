#!/bin/sh
# Entrypoint do servidor.
#
# Existe por causa de um detalhe do NVGT que não é óbvio e custa caro descobrir: ele resolve caminho
# RELATIVO pelo diretório do EXECUTÁVEL, não pelo diretório de trabalho. O servidor abre o banco de
# contas como "among_users.db" (ver src/database/user_db.nvgt), então o arquivo nasce ao lado do
# binário, e um `cd` antes de rodar não muda nada.
#
# Na VM isso passava despercebido porque o binário e o banco moravam os dois em /opt/amongus. Num
# contêiner, o binário mora na imagem - que é descartada a cada deploy - e o banco PRECISA morar no
# volume. Ou os dois estão no volume, ou o banco é gravado dentro da imagem e as contas de todo
# mundo somem na próxima versão.
#
# A saída é a mesma que o deploy.ps1 fazia na VM com `unzip -o`: o app da imagem é copiado por cima
# do que está no volume a cada boot, e o banco, que não faz parte do pacote, fica onde está. Assim o
# volume guarda app e dados juntos - exatamente o modelo que /opt/amongus tinha na máquina.
#
# Symlink NÃO serve aqui: o SQLite abre o banco com O_NOFOLLOW e recusa seguir link. E apontar só o
# banco para outro lugar também não serve - os arquivos -wal e -journal são criados ao lado dele, e
# separá-los do banco é receita para corromper tudo se o pod morrer no meio de uma escrita.
set -e

APP_DIR=/data
IMAGE_DIR=/opt/amongus

# -f para sobrescrever a versão anterior: é o deploy acontecendo. O banco não está no pacote, então
# ele não é tocado por nada disto.
mkdir -p "$APP_DIR/lib" "$APP_DIR/lang"
cp -f "$IMAGE_DIR/server_main" "$APP_DIR/server_main"
cp -f "$IMAGE_DIR"/lib/*.so "$APP_DIR/lib/"
cp -f "$IMAGE_DIR"/lang/*.json "$APP_DIR/lang/"
chmod +x "$APP_DIR/server_main"

cd "$APP_DIR"

# exec, e não uma chamada comum: assim o servidor vira o processo 1 do contêiner e recebe o SIGTERM
# do Kubernetes direto. Sem isso o shell segurava o sinal, o servidor não era avisado do
# desligamento e todo deploy terminava em kill forçado depois do período de tolerância.
exec "$APP_DIR/server_main" "$@"
