"""Junta o WAL de uma CÓPIA do banco de contas ao próprio .db e devolve um arquivo único.

O servidor roda o SQLite em modo WAL (ver user_database.open): as gravações recentes moram em
among_users.db-wal até o checkpoint. O Vps-CopyDb (infra/vps.ps1) copia os dois arquivos, e este
script aplica o WAL à cópia e a volta ao modo de diário comum - assim o backup é UM arquivo que abre
em qualquer lugar, e quem lê os recados não deixa um -wal largado para trás.

    python tools/consolidate_db.py <copia.db>

Só mexe na CÓPIA: nunca rode no banco do servidor (ele está aberto lá).
"""
import os
import sqlite3
import sys

path = sys.argv[1]
conn = sqlite3.connect(path)
conn.execute("PRAGMA wal_checkpoint(TRUNCATE)")
conn.execute("PRAGMA journal_mode=DELETE")
conn.close()
for extra in (path + "-wal", path + "-shm"):
    if os.path.exists(extra):
        os.remove(extra)
