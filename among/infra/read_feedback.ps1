# read_feedback.ps1
# Mostra os recados que os jogadores mandaram de dentro do jogo.
#
# Os recados ficam na tabela `feedback` do mesmo SQLite das contas, no volume do servidor. Este
# script traz uma cópia do banco e lê aqui, em vez de tentar consultar dentro do contêiner: a imagem
# do servidor não tem o sqlite3 de linha de comando (ela tem só o que o jogo precisa para rodar), e
# instalar um cliente lá dentro só para ler seria carregar peso por nada.
#
#   infra\read_feedback.ps1                # TODOS os recados
#   infra\read_feedback.ps1 -After 40      # só os que chegaram depois do #40
#   infra\read_feedback.ps1 -Limit 20      # só os 20 mais recentes
#   infra\read_feedback.ps1 -WithCrashLog  # mostra o crash.log inteiro de cada um
#   infra\read_feedback.ps1 -Out recados.txt  # grava em arquivo (UTF-8), em vez de mostrar
#
# Acentos: os recados vêm em três idiomas e o Python imprime UTF-8, mas o console do Windows lê
# outra coisa por padrão e mostra "vers�o". Por isso o script força UTF-8 no Python E no console.
# Para arquivo, use -Out em vez de ">": no Windows PowerShell o ">" grava UTF-16, e o arquivo abre
# como "p e s s o a", com um espaço entre cada letra.
#
# O padrão é mostrar tudo. Já foi "os 20 mais recentes", e o efeito foi parecer que o script cortava
# a lista: os recados chegam em dezenas por dia, e ninguém lembra de passar -Limit quando o que quer
# é simplesmente ler o que chegou. -After é o jeito de retomar de onde parou.

param(
	[int]$Limit = 0,
	[int]$After = 0,
	[switch]$WithCrashLog,
	# Por padrão os recados ARQUIVADOS ficam de fora (ver infra\resolve_feedback.ps1): a caixa mostra
	# o que ainda pede atenção. -All traz tudo de volta, com os arquivados marcados.
	[switch]$All,
	[string]$Namespace = "amongus",
	[string]$Out = ""
)

$ErrorActionPreference = "Stop"

$pod = kubectl get pods -n $Namespace -o jsonpath="{.items[0].metadata.name}"
if ([string]::IsNullOrWhiteSpace($pod)) { throw "Não achei o pod do servidor no namespace $Namespace." }

$origem = Get-Location # -Out relativo é relativo a de onde o script foi chamado, não à pasta de trabalho
$work = Join-Path $env:TEMP "amongus_feedback"
New-Item -ItemType Directory -Force $work | Out-Null
Push-Location $work
try {
	Remove-Item "feedback.db" -ErrorAction SilentlyContinue
	# Caminho RELATIVO de propósito: com um caminho absoluto do Windows, o "C:" é lido pelo kubectl
	# como o separador de "pod:caminho" e ele recusa dizendo que um dos lados tem que ser local.
	# O kubectl cp imprime um aviso do tar ("Removing leading '/'") que não é erro; vai para o nada.
	# Se a cópia falhar de verdade, o teste do arquivo logo abaixo é que acusa.
	kubectl cp -n $Namespace "${pod}:/data/among_users.db" "feedback.db" 2>&1 | Out-Null
	if (-not (Test-Path "feedback.db")) { throw "Não consegui copiar o banco do servidor." }

	# O banco é lido por Python porque ele já vem com o módulo sqlite3 embutido - nada a instalar.
	# O limite e o "mostrar crash.log" vão como ARGUMENTOS, e não interpolados no meio do código:
	# interpolar um booleano do PowerShell aqui produz "true" em minúsculo, que não é Python válido.
	$py = @"
import sqlite3, sys
limite = int(sys.argv[1])       # 0 = sem limite
depois_de = int(sys.argv[2])    # 0 = desde o comeco
mostrar_crash = sys.argv[3] == '1'
mostrar_tudo = sys.argv[4] == '1'
db = sqlite3.connect('feedback.db')
try:
    base = 'id, username, text, version, language, in_match, role, room, crash_log, created_at'
    # A coluna resolved_at so existe depois que o servidor da 0.32.0 subiu (ele a acrescenta ao
    # abrir o banco). Esta ferramenta e' usada ANTES e DEPOIS desse deploy, entao ela tenta com a
    # coluna e cai para o formato antigo - onde nada esta arquivado. Sem isto, rodar o leitor com um
    # banco antigo dizia "a tabela nem existe", que e' mentira e manda investigar a coisa errada.
    def consulta(com_resolvido):
        sql = 'SELECT %s FROM feedback WHERE id > ?' % (base + (', resolved_at' if com_resolvido else ''))
        if com_resolvido and not mostrar_tudo: sql += ' AND resolved_at IS NULL'
        sql += ' ORDER BY id DESC'
        if limite > 0: sql += ' LIMIT %d' % limite
        return list(db.execute(sql, (depois_de,)))
    try:
        rows = consulta(True)
    except sqlite3.OperationalError:
        rows = [tuple(r) + (None,) for r in consulta(False)]
except sqlite3.OperationalError:
    print('Nenhum recado ainda (a tabela nem existe: ninguem enviou nada).')
    sys.exit()
if not rows:
    print('Nenhum recado' + (' depois do #%d.' % depois_de if depois_de else ' ainda.'))
    sys.exit()
print('%d recado(s), do mais recente (#%d) para o mais antigo (#%d):' % (len(rows), rows[0][0], rows[-1][0]))
for r in rows:
    rid, user, text, ver, lang, in_match, role, room, crash, when, resolvido = r
    print('')
    print('=' * 70)
    print('#%s  %s  |  %s  |  versao %s  |  %s%s' % (rid, when, user or '(sem conta)', ver or '?', lang or '?', '  [ARQUIVADO]' if resolvido else ''))
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
	$env:PYTHONIOENCODING = "utf-8"
	[Console]::OutputEncoding = [System.Text.Encoding]::UTF8
	$flagCrash = if ($WithCrashLog) { "1" } else { "0" }
$flagAll = if ($All) { "1" } else { "0" }
	if ($Out -ne "") {
		$destino = if ([System.IO.Path]::IsPathRooted($Out)) { $Out } else { Join-Path $origem $Out }
		python "ler.py" $Limit $After $flagCrash $flagAll | Out-File -FilePath $destino -Encoding utf8
		Write-Host "Gravado em $destino"
	} else {
		python "ler.py" $Limit $After $flagCrash $flagAll
	}
}
finally {
	Pop-Location
}
