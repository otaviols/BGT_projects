output "server_ip" {
  description = "IP do servidor do jogo. É este valor que vai em DEFAULT_SERVER_HOST (src/config/game_constants.nvgt) antes de compilar o cliente para os jogadores."
  value       = azurerm_public_ip.game.ip_address
}

output "site_url" {
  description = "Endereço do site de download."
  value       = azurerm_storage_account.site.primary_web_endpoint
}

# Não há mais `ssh_command`: o servidor não é mais uma VM. Para olhar o que ele está fazendo, o
# equivalente agora é:  kubectl logs -n amongus deploy/amongus-server -f
output "logs_command" {
  description = "Comando para acompanhar o servidor ao vivo."
  value       = "kubectl logs -n amongus deploy/amongus-server -f"
}

output "next_steps" {
  description = "O que fazer depois do apply."
  value       = <<-EOT
    1. Coloque o IP ${azurerm_public_ip.game.ip_address} em DEFAULT_SERVER_HOST (src/config/game_constants.nvgt).
    2. Gere os pacotes:  nvgt tools/build_pack.nvgt
                         nvgt -c -plinux server_main.nvgt
                         nvgt -c AmongUs.nvgt
    3. Publique tudo:    infra/deploy.ps1 -StorageAccount ${azurerm_storage_account.site.name}
    4. Site de download: ${azurerm_storage_account.site.primary_web_endpoint}
  EOT
}
