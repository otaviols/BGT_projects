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
  description = "Região do Azure. Latência importa num jogo em tempo real: escolha a mais perto de quem vai jogar."
  type        = string
  default     = "brazilsouth"
}

variable "vm_size" {
  description = "Tamanho da VM. B1s (1 vCPU, 1 GB) dá conta de sobra: o servidor só valida movimento e retransmite pacotes pequenos para no máximo 10 jogadores."
  type        = string
  default     = "Standard_B1s"
}

variable "admin_username" {
  description = "Usuário administrador da VM (acesso por SSH)."
  type        = string
  default     = "azureuser"
}

variable "ssh_public_key" {
  description = "Conteúdo da sua chave pública SSH (o arquivo .pub inteiro). Gere com: ssh-keygen -t ed25519"
  type        = string
}

variable "admin_source_ip" {
  description = "De onde o SSH pode entrar. Use o seu IP público (ex.: \"189.10.20.30\") ou uma faixa CIDR. Evite \"*\": isso abre a porta 22 pra internet inteira."
  type        = string
}

variable "game_port" {
  description = "Porta UDP do servidor do jogo. Precisa bater com DEFAULT_SERVER_PORT em config/game_constants.nvgt."
  type        = number
  default     = 8934
}

variable "storage_account_name" {
  description = "Nome do Storage Account do site de download. Precisa ser único no Azure inteiro, só letras minúsculas e números, 3 a 24 caracteres."
  type        = string

  validation {
    condition     = can(regex("^[a-z0-9]{3,24}$", var.storage_account_name))
    error_message = "Só letras minúsculas e números, de 3 a 24 caracteres - é a regra do Azure para nome de Storage Account."
  }
}
