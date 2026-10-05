"""Mostra os recados de uma CÓPIA do banco do servidor. Não rode direto: use infra\\admin.ps1 recados.

    python tools/read_feedback.py <banco.db> <limite> <depois_de> <mostrar_crash 0|1> <mostrar_tudo 0|1>

Python porque ele já vem com o sqlite3 - a imagem do servidor não tem o sqlite3 de linha de comando, e
instalar um lá dentro só para ler seria carregar peso por nada. Era um texto dentro do PowerShell;
virou arquivo para poder ser lido e mudado como código.
"""
import sqlite3
import sys

sys.stdout.reconfigure(encoding='utf-8')
banco = sys.argv[1]
limite = int(sys.argv[2])       # 0 = sem limite
depois_de = int(sys.argv[3])    # 0 = desde o começo
mostrar_crash = sys.argv[4] == '1'
mostrar_tudo = sys.argv[5] == '1'

db = sqlite3.connect(banco)
base = 'id, username, text, version, language, in_match, role, room, crash_log, created_at'


def consulta(com_resolvido):
    sql = 'SELECT %s FROM feedback WHERE id > ?' % (base + (', resolved_at' if com_resolvido else ''))
    if com_resolvido and not mostrar_tudo:
        sql += ' AND resolved_at IS NULL'
    sql += ' ORDER BY id DESC'
    if limite > 0:
        sql += ' LIMIT %d' % limite
    return list(db.execute(sql, (depois_de,)))


try:
    # A coluna resolved_at só existe depois que o servidor da 0.32.0 subiu. Sem ela, cai para o
    # formato antigo - onde nada está arquivado - em vez de dizer que a tabela nem existe.
    try:
        rows = consulta(True)
    except sqlite3.OperationalError:
        rows = [tuple(r) + (None,) for r in consulta(False)]
except sqlite3.OperationalError:
    print('Nenhum recado ainda (a tabela nem existe: ninguém enviou nada).')
    sys.exit()

if not rows:
    print('Nenhum recado' + (' depois do #%d.' % depois_de if depois_de else ' pendente.'))
    sys.exit()
respostas = {}
try:
    for fid, quando, resposta, lida in db.execute(
            'SELECT feedback_id, created_at, text, read_at FROM feedback_replies ORDER BY id'):
        respostas.setdefault(fid, []).append((quando, resposta, lida))
except sqlite3.OperationalError:
    pass  # banco de antes das respostas existirem
print('%d recado(s), do mais recente (#%d) para o mais antigo (#%d):' % (len(rows), rows[0][0], rows[-1][0]))
for r in rows:
    rid, user, text, ver, lang, in_match, role, room, crash, when, resolvido = r
    print('')
    print('=' * 70)
    print('#%s  %s  |  %s  |  versão %s  |  %s%s' % (rid, when, user or '(sem conta)', ver or '?', lang or '?', '  [ARQUIVADO]' if resolvido else ''))
    onde = []
    if in_match:
        onde.append('em partida')
    if role:
        onde.append('papel: ' + role)
    if room:
        onde.append('sala: ' + room)
    if onde:
        print('   ' + '  |  '.join(onde))
    print('-' * 70)
    print(text)
    if crash:
        print('')
        if mostrar_crash:
            print('--- crash.log ---')
            print(crash)
        else:
            print('   [tem crash.log anexado - rode com -Crash para ver]')
    # As respostas já mandadas, embaixo do recado: sem elas, quem responde não sabe o que já foi dito
    # (nem o tom em que foi dito), e o jogador ouviria duas respostas para a mesma coisa.
    for quando, resposta, lida in respostas.get(rid, []):
        print('')
        print('   >> resposta %s%s' % (quando, '' if lida else '  (ainda não ouvida)'))
        print('   ' + resposta.replace('\n', '\n   '))
