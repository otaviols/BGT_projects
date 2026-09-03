variable "subscription_id" {
  description = "ID da assinatura do Azure onde tudo será criado (az account show --query id -o tsv)."
  type        = string
}

variable "prefix" {
  description = "Prefixo dos nomes dos recursos. Trocar isto permite subir um segundo ambiente (teste, por exemplo) sem colidir com o primeiro."
  type        = string
  default     = "amongus"
}

variable "location" {
  description = "Região do Azure. Latência importa num jogo em tempo real: escolha a mais perto de quem vai jogar. Vale manter na mesma região do cluster - o IP e o balanceador precisam estar juntos."
  type        = string
  default     = "brazilsouth"
}

# --- Cluster que hospeda o servidor -------------------------------------------
# O servidor roda no AKS em vez de numa VM própria (ver o comentário no main.tf). O cluster NÃO é
# criado aqui: ele já existia, é compartilhado com outro jogo, e apagá-lo por causa deste projeto
# derrubaria o outro junto. Aqui ele é só consultado, para conceder a permissão do IP.

variable "aks_cluster_name" {
  description = "Nome do cluster AKS que hospeda o servidor."
  type        = string
  default     = "aks-fallenrealms-alpha"
}

variable "aks_resource_group_name" {
  description = "Grupo de recursos do cluster AKS. É o grupo do cluster em si, não o MC_... que o Azure cria sozinho para os nós."
  type        = string
  default     = "rg-fallenrealms-alpha"
}

# --- Site de download ---------------------------------------------------------

variable "storage_account_name" {
  description = "Nome do Storage Account do site de download. Precisa ser único no Azure inteiro, só letras minúsculas e números, 3 a 24 caracteres."
  type        = string

  validation {
    condition     = can(regex("^[a-z0-9]{3,24}$", var.storage_account_name))
    error_message = "Só letras minúsculas e números, de 3 a 24 caracteres - é a regra do Azure para nome de Storage Account."
  }
}
