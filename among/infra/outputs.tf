output "server_ip" {
  description = "IP do servidor do jogo. É este valor que vai em DEFAULT_SERVER_HOST (config/game_constants.nvgt) antes de compilar o cliente para os jogadores."
  value       = azurerm_public_ip.game.ip_address
}

output "site_url" {
  description = "Endereço do site de download."
  value       = azurerm_storage_account.site.primary_web_endpoint
}

output "ssh_command" {
  description = "Comando pronto para entrar na máquina."
  value       = "ssh ${var.admin_username}@${azurerm_public_ip.game.ip_address}"
}

output "next_steps" {
  description = "O que fazer depois do apply."
  value       = <<-EOT
    1. Coloque o IP ${azurerm_public_ip.game.ip_address} em DEFAULT_SERVER_HOST (config/game_constants.nvgt).
    2. Gere os pacotes:  nvgt tools/build_pack.nvgt
                         nvgt -c -plinux server_main.nvgt
                         nvgt -c AmongUs.nvgt
    3. Publique tudo:    infra/deploy.ps1 -StorageAccount ${azurerm_storage_account.site.name} -ServerIp ${azurerm_public_ip.game.ip_address}
    4. Site de download: ${azurerm_storage_account.site.primary_web_endpoint}
  EOT
}
