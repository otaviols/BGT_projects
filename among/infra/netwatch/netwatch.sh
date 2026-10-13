#!/bin/sh
# netwatch.sh - a rede da VPS vista de dentro dela, segundo a segundo.
#
# Existe por causa das quedas em grupo (2026-10-13: tres jogadores, em duas salas, por tempo esgotado no
# mesmo meio minuto, com o servidor sem trava e o resto conectado). O log do jogo diz QUEM caiu; isto
# diz se a VPS perdeu a rede naquela hora. Se perdeu, e caso de chamado na Hostinger; se nao, o
# problema esta no caminho de alguns provedores ate ela.
#
# A cada segundo pinga tres alvos: 200.160.2.3 (a.dns.br, do NIC.br em Sao Paulo - a rede brasileira,
# por onde chega quase todo jogador), 1.1.1.1 e 8.8.8.8 (a internet por dois caminhos).
# - Segundo em que os TRES falham: uma linha "SEM REDE" na hora. E o sinal que importa.
# - Falha de um alvo so e ruido (o do NIC.br limita ping e perde um de vez em quando): vai so na conta
#   do minuto, por alvo.
# - A cada minuto: quantos pacotes UDP o CONTEINER do jogo recebeu (o contador da maquina nao ve: o
#   Docker encaminha direto para o conteiner, e la e outro espaco de rede - lido em /proc/<pid>/net).
#   Se o numero afunda no minuto de uma queda, os pacotes dos jogadores pararam de chegar.
# O gateway da Hostinger NAO serve de alvo: nao responde a ping (testado na instalacao).
#
# Lido por: infra\admin.ps1 rede. Instalado como servico (netwatch.service) - ver
# notes/infra-e-deploy.md.

LOG=/var/log/netwatch.log
MAX_LINES=20000
ALVOS="200.160.2.3 1.1.1.1 8.8.8.8"

# UDP recebido pelo conteiner do jogo (vazio se ele nao estiver de pe - num deploy, por exemplo).
udp_in() {
	pid=$(docker inspect -f '{{.State.Pid}}' amongus-server 2>/dev/null)
	[ -n "$pid" ] && [ "$pid" != "0" ] && awk '/^Udp:/ { n++; if (n == 2) print $2 }' "/proc/$pid/net/snmp" 2>/dev/null
}

echo "$(date -u +%FT%TZ) inicio" >> "$LOG"
ultimo_udp=$(udp_in)
ultimo_minuto=$(date -u +%M)
f_br=0; f_cf=0; f_gg=0; sem_rede=0

while true; do
	n=0; falhou=""
	for alvo in $ALVOS; do
		if ! ping -n -c 1 -W 1 "$alvo" > /dev/null 2>&1; then
			n=$((n + 1)); falhou="$falhou $alvo"
			case "$alvo" in
				200.160.2.3) f_br=$((f_br + 1)) ;;
				1.1.1.1) f_cf=$((f_cf + 1)) ;;
				8.8.8.8) f_gg=$((f_gg + 1)) ;;
			esac
		fi
	done
	if [ "$n" -eq 3 ]; then
		echo "$(date -u +%FT%TZ) SEM REDE (nenhum alvo respondeu)" >> "$LOG"
		sem_rede=$((sem_rede + 1))
	fi

	minuto=$(date -u +%M)
	if [ "$minuto" != "$ultimo_minuto" ]; then
		agora_udp=$(udp_in)
		if [ -n "$agora_udp" ] && [ -n "$ultimo_udp" ] && [ "$agora_udp" -ge "$ultimo_udp" ]; then
			udp=$((agora_udp - ultimo_udp))
		else
			udp="?"
		fi
		echo "$(date -u +%FT%TZ) minuto udp_jogo=$udp sem_rede=$sem_rede falhas br=$f_br cloudflare=$f_cf google=$f_gg" >> "$LOG"
		ultimo_udp=$agora_udp
		ultimo_minuto=$minuto
		f_br=0; f_cf=0; f_gg=0; sem_rede=0
		# Uma vez por hora o arquivo e aparado: ~1500 linhas por dia, entao 20000 sao semanas.
		if [ "$minuto" = "00" ] && [ "$(wc -l < "$LOG")" -gt "$MAX_LINES" ]; then
			tail -n "$MAX_LINES" "$LOG" > "$LOG.tmp" && mv "$LOG.tmp" "$LOG"
		fi
	fi
	sleep 1
done
