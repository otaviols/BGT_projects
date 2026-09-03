# deploy.ps1
# Publica uma versão: manda o servidor pra VM e o cliente pro site de download.
#
# Rode da raiz do projeto (a pasta among), depois de ter gerado os pacotes:
#   nvgt tools/build_pack.nvgt
#   nvgt -c -plinux server_main.nvgt
#   nvgt -c AmongUs.nvgt
#   infra\deploy.ps1 -StorageAccount <nome> -ServerIp <ip>
#
# Os dois valores saem do `terraform output` depois do apply.

param(
	[Parameter(Mandatory = $true)][string]$StorageAccount,
	[Parameter(Mandatory = $true)][string]$ServerIp,
	[string]$AdminUser = "azureuser",
	# Pular uma das partes é útil quando só o cliente mudou (ou só o servidor).
	[switch]$SkipServer,
	[switch]$SkipSite
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot

if (-not $SkipServer) {
	$serverZip = Join-Path $root "server_main.zip"
	if (-not (Test-Path $serverZip)) {
		throw "Não achei $serverZip. Gere com: nvgt -c -plinux server_main.nvgt"
	}

	Write-Host "Enviando o servidor para $ServerIp..."
	scp $serverZip "${AdminUser}@${ServerIp}:/tmp/server_main.zip"

	# O unzip sobrescreve o binário e as bibliotecas, mas NÃO toca no among_users.db: ele não está
	# no pacote, então as contas dos jogadores sobrevivem a todo deploy.
	$remote = @"
set -e
sudo unzip -o /tmp/server_main.zip -d /opt/amongus
sudo chown -R amongus:amongus /opt/amongus
sudo chmod +x /opt/amongus/server_main
sudo systemctl restart amongus.service
rm -f /tmp/server_main.zip
sleep 2
systemctl is-active amongus.service
"@
	ssh "${AdminUser}@${ServerIp}" $remote
	Write-Host "Servidor no ar."
}

if (-not $SkipSite) {
	$clientZip = Join-Path $root "AmongUs.zip"
	if (-not (Test-Path $clientZip)) {
		throw "Não achei $clientZip. Gere com: nvgt -c AmongUs.nvgt"
	}

	Write-Host "Publicando o site e o cliente..."
	# O site estático mora no container `$web` - é o nome que o Azure exige, não é escolha nossa.
	az storage blob upload --account-name $StorageAccount --auth-mode login `
		--container-name '$web' --name "AmongUs.zip" --file $clientZip --overwrite | Out-Null
	az storage blob upload-batch --account-name $StorageAccount --auth-mode login `
		--destination '$web' --source (Join-Path $PSScriptRoot "site") --overwrite | Out-Null

	$url = az storage account show --name $StorageAccount --query "primaryEndpoints.web" -o tsv
	Write-Host "Site publicado: $url"
}
