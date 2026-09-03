# Imagem do servidor dedicado.
#
# O contexto de build é a RAIZ do projeto (a pasta among), não esta pasta - o Dockerfile precisa do
# server_main.zip, que o NVGT gera lá. Ver deploy.ps1, que já chama o build com o contexto certo:
#   nvgt -c -plinux server_main.nvgt
#   docker build -f infra/Dockerfile -t <imagem> .
#
# Jammy de propósito: é a mesma base que a VM usava (cloud-init.yaml), então o binário do NVGT roda
# contra a mesma glibc contra a qual ele já era testado. Trocar por uma base menor (alpine/musl)
# quebraria o binário, que é glibc.
FROM ubuntu:22.04 AS extract

RUN apt-get update \
	&& apt-get install -y --no-install-recommends unzip \
	&& rm -rf /var/lib/apt/lists/*

WORKDIR /pkg
COPY server_main.zip .
# O zip vem com o binário na raiz e as bibliotecas em lib/ - a mesma disposição que o systemd
# esperava na VM, e é por isso que o LD_LIBRARY_PATH lá embaixo aponta para lib/.
RUN unzip -q server_main.zip \
	&& rm server_main.zip \
	&& chmod +x server_main

FROM ubuntu:22.04

# ca-certificates não é firula: sem ele qualquer chamada TLS que o SQLite ou o runtime venham a
# fazer falha com erro de certificado, e o motivo é bem difícil de adivinhar num container.
RUN apt-get update \
	&& apt-get install -y --no-install-recommends ca-certificates libstdc++6 \
	&& rm -rf /var/lib/apt/lists/*

# Mesmo princípio do usuário `amongus` do cloud-init: o servidor não roda como root, então uma falha
# nele não vira poder sobre o container. O UID é fixo porque o PVC guarda arquivos com dono - se o
# UID mudasse entre versões, o servidor perderia acesso ao próprio banco de contas no deploy seguinte.
RUN useradd --system --uid 10001 --shell /usr/sbin/nologin amongus \
	&& mkdir -p /data \
	&& chown amongus:amongus /data

COPY --from=extract /pkg /opt/amongus
COPY infra/docker-entrypoint.sh /usr/local/bin/docker-entrypoint.sh
RUN chmod +x /usr/local/bin/docker-entrypoint.sh

# O binário do NVGT carrega as bibliotecas dele de lib/, e o Linux não procura ali sozinho - é a
# mesma linha que o serviço systemd precisava na VM, pelo mesmo motivo.
#
# Aponta para /data, e não para /opt/amongus, porque é de /data que o servidor roda: o entrypoint
# copia o app para lá no boot, já que o NVGT grava o banco de contas ao lado do executável e esse
# lugar precisa ser o volume. Ver o comentário no docker-entrypoint.sh.
ENV LD_LIBRARY_PATH=/data/lib

# /data é o volume persistente: é onde ficam o among_users.db E o app copiado da imagem. O conteúdo
# de /opt/amongus é só o molde que o entrypoint copia - a cada versão nova ele é sobrescrito.
WORKDIR /data
VOLUME ["/data"]
USER amongus

EXPOSE 8934/udp

ENTRYPOINT ["/usr/local/bin/docker-entrypoint.sh"]
