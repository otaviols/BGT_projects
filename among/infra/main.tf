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

# --- O servidor ---------------------------------------------------------------
#
# Não há VM aqui, e isso é deliberado. O servidor roda como um contêiner no cluster AKS que já
# existe nesta assinatura (ver infra/k8s/amongus.yaml), pelos dois motivos abaixo:
#
# 1. CUSTO. O nó do cluster já está pago e tem folga de sobra para um servidor que só valida
#    movimento e retransmite pacotes pequenos para no máximo 10 jogadores. Uma VM dedicada seria uma
#    segunda máquina cobrada integralmente para fazer muito menos do que o nó já faz.
#
# 2. ESTA ASSINATURA NÃO CONSEGUE CRIAR A VM. Todas as SKUs baratas (família B, Dv3/v5, F2s_v2,
#    A_v2) estão marcadas "NotAvailableForSubscription" - em TODAS as regiões testadas, então não é
#    questão de escolher outra. As únicas SKUs sem restrição em brazilsouth são as de computação
#    confidencial (DCasv6/ECasv6), e essas têm cota ZERO (`az vm list-usage` mostra limite 0), o que
#    não aparece na lista de SKUs disponíveis e fazia o apply falhar sempre no mesmo ponto: com toda
#    a rede já criada e só a VM faltando.
#
#    O AKS escapa disso porque cria os nós por um caminho diferente (VMSS gerenciado) - o nó atual é
#    justamente uma D2als_v6, que a criação direta de VM recusa.
#
# Se um dia esta infra voltar a ter uma VM própria, o histórico do git tem a versão anterior deste
# arquivo com a VM, o cloud-init e o serviço systemd prontos.

# O endereço do servidor vai COMPILADO dentro do cliente (DEFAULT_SERVER_HOST em
# src/config/game_constants.nvgt). Um IP que muda obrigaria a redistribuir o jogo para todo mundo, então
# ele é estático e vive aqui - fora do cluster - de propósito: assim ele sobrevive a apagar o
# Service, recriar o balanceador ou até trocar de cluster.
resource "azurerm_public_ip" "game" {
  name                = "${var.prefix}-ip"
  location            = azurerm_resource_group.game.location
  resource_group_name = azurerm_resource_group.game.name
  allocation_method   = "Static"
  # Standard porque é o que o balanceador do AKS usa. Um IP Basic aqui é recusado na hora de
  # associar, com um erro que fala de SKU e não deixa claro qual dos dois lados está errado.
  sku = "Standard"
}

# O cluster mora noutro grupo de recursos, e por padrão ele só enxerga o próprio. Sem esta
# permissão, o Service do jogo fica eternamente em "pending": o balanceador tenta pegar o IP acima,
# recebe um 403 e continua tentando, sem nada no `kubectl get svc` que explique o motivo.
# O escopo é o grupo de recursos, e não o IP: o balanceador precisa consultar o grupo para achá-lo
# pelo nome (é o que a anotação azure-load-balancer-resource-group faz).
data "azurerm_kubernetes_cluster" "host" {
  name                = var.aks_cluster_name
  resource_group_name = var.aks_resource_group_name
}

resource "azurerm_role_assignment" "aks_can_use_ip" {
  scope                = azurerm_resource_group.game.id
  role_definition_name = "Network Contributor"
  principal_id         = data.azurerm_kubernetes_cluster.host.identity[0].principal_id
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
