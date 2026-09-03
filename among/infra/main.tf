terraform {
  required_version = ">= 1.5"
  required_providers {
    azurerm = {
      source  = "hashicorp/azurerm"
      version = "~> 4.0"
    }
  }
}

provider "azurerm" {
  features {}
  subscription_id = var.subscription_id
}

resource "azurerm_resource_group" "game" {
  name     = "${var.prefix}-rg"
  location = var.location
}

# --- Rede ---------------------------------------------------------------------

resource "azurerm_virtual_network" "game" {
  name                = "${var.prefix}-vnet"
  address_space       = ["10.20.0.0/16"]
  location            = azurerm_resource_group.game.location
  resource_group_name = azurerm_resource_group.game.name
}

resource "azurerm_subnet" "game" {
  name                 = "${var.prefix}-subnet"
  resource_group_name  = azurerm_resource_group.game.name
  virtual_network_name = azurerm_virtual_network.game.name
  address_prefixes     = ["10.20.1.0/24"]
}

# O IP é ESTÁTICO de propósito: o endereço do servidor vai compilado dentro do cliente
# (DEFAULT_SERVER_HOST em config/game_constants.nvgt). Um IP que muda obrigaria a redistribuir o
# jogo para todo mundo.
resource "azurerm_public_ip" "game" {
  name                = "${var.prefix}-ip"
  location            = azurerm_resource_group.game.location
  resource_group_name = azurerm_resource_group.game.name
  allocation_method   = "Static"
  sku                 = "Standard"
}

resource "azurerm_network_security_group" "game" {
  name                = "${var.prefix}-nsg"
  location            = azurerm_resource_group.game.location
  resource_group_name = azurerm_resource_group.game.name

  # O jogo fala por ENet, que é UDP. Liberar só TCP aqui é o erro clássico: a porta parece aberta
  # em qualquer teste de porta e mesmo assim ninguém conecta.
  security_rule {
    name                       = "game-udp"
    priority                   = 100
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Udp"
    source_port_range          = "*"
    destination_port_range     = tostring(var.game_port)
    source_address_prefix      = "Internet"
    destination_address_prefix = "*"
  }

  # SSH só de onde você administra. "*" aqui deixaria o mundo inteiro batendo na porta.
  security_rule {
    name                       = "ssh"
    priority                   = 200
    direction                  = "Inbound"
    access                     = "Allow"
    protocol                   = "Tcp"
    source_port_range          = "*"
    destination_port_range     = "22"
    source_address_prefix      = var.admin_source_ip
    destination_address_prefix = "*"
  }
}

resource "azurerm_network_interface" "game" {
  name                = "${var.prefix}-nic"
  location            = azurerm_resource_group.game.location
  resource_group_name = azurerm_resource_group.game.name

  ip_configuration {
    name                          = "internal"
    subnet_id                     = azurerm_subnet.game.id
    private_ip_address_allocation = "Dynamic"
    public_ip_address_id          = azurerm_public_ip.game.id
  }
}

resource "azurerm_network_interface_security_group_association" "game" {
  network_interface_id      = azurerm_network_interface.game.id
  network_security_group_id = azurerm_network_security_group.game.id
}

# --- A máquina do servidor ----------------------------------------------------

resource "azurerm_linux_virtual_machine" "game" {
  name                            = "${var.prefix}-vm"
  resource_group_name             = azurerm_resource_group.game.name
  location                        = azurerm_resource_group.game.location
  size                            = var.vm_size
  admin_username                  = var.admin_username
  network_interface_ids           = [azurerm_network_interface.game.id]
  disable_password_authentication = true

  admin_ssh_key {
    username   = var.admin_username
    public_key = var.ssh_public_key
  }

  os_disk {
    caching              = "ReadWrite"
    storage_account_type = "Standard_LRS"
  }

  source_image_reference {
    publisher = "Canonical"
    offer     = "0001-com-ubuntu-server-jammy"
    sku       = "22_04-lts-gen2"
    version   = "latest"
  }

  # Prepara a máquina e instala o serviço, mas sem o jogo ainda: o binário sobe depois com o
  # deploy.ps1. O serviço fica reiniciando até o binário chegar, e aí sobe sozinho.
  custom_data = base64encode(templatefile("${path.module}/cloud-init.yaml", {
    admin_username = var.admin_username
  }))
}

# --- Site de download ---------------------------------------------------------
# Um Storage Account com site estático: sem servidor web pra manter, sem certificado pra renovar,
# HTTPS já incluso, e o custo é de centavos. O conteúdo (página, zip do jogo e version.json) sobe
# pelo deploy.ps1 - não fica no Terraform porque muda a cada versão, e isso sujaria o estado.
resource "azurerm_storage_account" "site" {
  name                            = var.storage_account_name
  resource_group_name             = azurerm_resource_group.game.name
  location                        = azurerm_resource_group.game.location
  account_tier                    = "Standard"
  account_replication_type        = "LRS"
  allow_nested_items_to_be_public = true
}

# Sem isto o primeiro deploy falha com um 403 confuso: ser dono da assinatura dá poder sobre o
# RECURSO (criar, apagar a conta), mas não sobre os DADOS dentro dele. Enviar um blob exige este
# papel, e é por isso que ele é criado junto com a infra em vez de virar um passo manual esquecido.
data "azurerm_client_config" "current" {}

resource "azurerm_role_assignment" "site_upload" {
  scope                = azurerm_storage_account.site.id
  role_definition_name = "Storage Blob Data Contributor"
  principal_id         = data.azurerm_client_config.current.object_id
}

# Recurso separado (e não o bloco static_website dentro da conta) porque aquele está descontinuado e
# some na versão 5 do provider - fazer certo agora evita um conserto no meio de um deploy futuro.
resource "azurerm_storage_account_static_website" "site" {
  storage_account_id = azurerm_storage_account.site.id
  index_document     = "index.html"
  error_404_document = "index.html"
}
