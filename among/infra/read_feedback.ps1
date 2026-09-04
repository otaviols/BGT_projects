# read_feedback.ps1
# Mostra os recados que os jogadores mandaram de dentro do jogo.
#
# Os recados ficam na tabela `feedback` do mesmo SQLite das contas, no volume do servidor. Este
# script traz uma cópia do banco e lê aqui, em vez de tentar consultar dentro do contêiner: a imagem
# do servidor não tem o sqlite3 de linha de comando (ela tem só o que o jogo precisa para rodar), e
# instalar um cliente lá dentro só para ler seria carregar peso por nada.
#
#   infra\read_feedback.ps1              # os 20 mais recentes
#   infra\read_feedback.ps1 -Limit 100   # mais
#   infra\read_feedback.ps1 -WithCrashLog  # mostra o crash.log inteiro de cada um

param(
	[int]$Limit = 20,
	[switch]$WithCrashLog,
	[string]$Namespace = "amongus"
)

$ErrorActionPreference = "Stop"

$pod = kubectl get pods -n $Namespace -o jsonpath="{.items[0].metadata.name}"
if ([string]::IsNullOrWhiteSpace($pod)) { throw "Não achei o pod do servidor no namespace $Namespace." }

$work = Join-Path $env:TEMP "amongus_feedback"
New-Item -ItemType Directory -Force $work | Out-Null
Push-Location $work
try {
	Remove-Item "feedback.db" -ErrorAction SilentlyContinue
	# Caminho RELATIVO de propósito: com um caminho absoluto do Windows, o "C:" é lido pelo kubectl
	# como o separador de "pod:caminho" e ele recusa dizendo que um dos lados tem que ser local.
	kubectl cp -n $Namespace "${pod}:/data/among_users.db" "feedback.db" 2>$null
	if (-not (Test-Path "feedback.db")) { throw "Não consegui copiar o banco do servidor." }

	# O banco é lido por Python porque ele já vem com o módulo sqlite3 embutido - nada a instalar.
	# O limite e o "mostrar crash.log" vão como ARGUMENTOS, e não interpolados no meio do código:
	# interpolar um booleano do PowerShell aqui produz "true" em minúsculo, que não é Python válido.
	$py = @"
import sqlite3, sys
limite = int(sys.argv[1])
mostrar_crash = sys.argv[2] == '1'
db = sqlite3.connect('feedback.db')
try:
    rows = list(db.execute('SELECT id, username, text, version, language, in_match, role, room, crash_log, created_at FROM feedback ORDER BY id DESC LIMIT ?', (limite,)))
except sqlite3.OperationalError:
    print('Nenhum recado ainda (a tabela nem existe: ninguem enviou nada).')
    sys.exit()
if not rows:
    print('Nenhum recado ainda.')
    sys.exit()
print('%d recado(s), do mais recente para o mais antigo:' % len(rows))
for r in rows:
    rid, user, text, ver, lang, in_match, role, room, crash, when = r
    print('')
    print('=' * 70)
    print('#%s  %s  |  %s  |  versao %s  |  %s' % (rid, when, user or '(sem conta)', ver or '?', lang or '?'))
    onde = []
    if in_match: onde.append('em partida')
    if role: onde.append('papel: ' + role)
    if room: onde.append('sala: ' + room)
    if onde: print('   ' + '  |  '.join(onde))
    print('-' * 70)
    print(text)
    if crash:
        if mostrar_crash:
            print('')
            print('--- crash.log ---')
            print(crash)
        else:
            print('')
            print('   [tem crash.log anexado - rode com -WithCrashLog para ver]')
"@
	$py | Out-File -FilePath "ler.py" -Encoding utf8
	python "ler.py" $Limit $(if ($WithCrashLog) { "1" } else { "0" })
}
finally {
	Pop-Location
}
