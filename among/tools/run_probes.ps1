# run_probes.ps1
# Roda as sondas automáticas contra um servidor LOCAL desta pasta e resume cada uma numa linha.
#
#   tools\run_probes.ps1                       # todas as automáticas
#   tools\run_probes.ps1 -Filtro wolf          # só as que têm "wolf" no nome
#   tools\run_probes.ps1 -Saida antes.txt      # grava o resumo (para comparar com outra rodada)
#   tools\run_probes.ps1 -Raiz C:\outra\pasta  # roda as sondas e o servidor de outra cópia (worktree)
#
# Para que serve: mudança que só MOVE código (a divisão do servidor em partes, por exemplo) se prova
# comparando a rodada de antes com a de depois - mesmo número de sondas terminando, mesmos "ok",
# mesmas falhas. As sondas não seguem um formato só (umas terminam com "RESULTADO:", outras imprimem
# "FALHA" no caminho), então o resumo guarda as duas coisas.
#
# Ficam de fora as que precisam de gente, de microfone/alto-falante ou de argumentos (lista abaixo).

param(
	[string]$Raiz = "",
	[string]$Filtro = "",
	[string]$Saida = "",
	[int]$TempoMaximo = 180
)

$ErrorActionPreference = "Stop"
if ($Raiz -eq "") { $Raiz = Split-Path -Parent $PSScriptRoot }
$fora = @("probe_devices", "probe_playback", "probe_play_wait", "probe_stream", "probe_stream_block",
	"probe_talker", "probe_sender", "voice_probe", "probe_voice_no_freeze", "probe_updater_script",
	"probe_limbo", "probe_android_assets", "probe_net_stall")

$sondas = Get-ChildItem (Join-Path $Raiz "tools\probes") -Filter *.nvgt |
	Where-Object { $fora -notcontains $_.BaseName -and ($Filtro -eq "" -or $_.BaseName -like "*$Filtro*") } |
	Sort-Object Name

$tokenAntes = $env:AMONGUS_ADMIN_TOKEN
$hostAntes = $env:AMONGUS_SERVER_HOST
$env:AMONGUS_ADMIN_TOKEN = "teste-local"
$env:AMONGUS_SERVER_HOST = "127.0.0.1"
Get-Process nvgt -ErrorAction SilentlyContinue | Stop-Process -Force
# Banco novo a cada rodada: contas e recados de uma rodada não podem mudar o resultado da outra.
Remove-Item (Join-Path $Raiz "among_users.db") -ErrorAction SilentlyContinue
$servidor = Start-Process nvgt -ArgumentList "server_main.nvgt" -WorkingDirectory $Raiz -PassThru -WindowStyle Hidden
Start-Sleep -Seconds 6
$linhas = @()
try {
	foreach ($s in $sondas) {
		if ($servidor.HasExited) { throw "O servidor local caiu durante a rodada (antes de $($s.Name))." }
		$out = Join-Path $env:TEMP "probe_out.txt"
		$p = Start-Process nvgt -ArgumentList "tools/probes/$($s.Name)" -WorkingDirectory $Raiz -PassThru -WindowStyle Hidden `
			-RedirectStandardOutput $out -RedirectStandardError "$out.err"
		$terminou = $p.WaitForExit($TempoMaximo * 1000)
		if (-not $terminou) { Stop-Process $p -Force }
		$texto = (Get-Content $out -Encoding UTF8 -ErrorAction SilentlyContinue) + (Get-Content "$out.err" -ErrorAction SilentlyContinue)
		$resultado = ($texto | Where-Object { $_ -match "RESULTADO" } | Select-Object -Last 1)
		$falhas = @($texto | Where-Object { $_ -match "FALHA|falhou|nao conectou|Exception|ERROR" }).Count
		$oks = @($texto | Where-Object { $_ -match "^\s*ok\b" }).Count
		$linha = "{0,-34} {1,-9} ok={2,-3} falhas={3,-3} {4}" -f $s.BaseName, $(if ($terminou) { "terminou" } else { "TEMPO" }), $oks, $falhas, "$resultado".Trim()
		Write-Host $linha
		$linhas += $linha
	}
}
finally {
	Stop-Process $servidor -Force -ErrorAction SilentlyContinue
	$env:AMONGUS_ADMIN_TOKEN = $tokenAntes
	$env:AMONGUS_SERVER_HOST = $hostAntes
}
if ($Saida -ne "") { $linhas | Out-File -FilePath $Saida -Encoding utf8; Write-Host "Resumo em $Saida" }
