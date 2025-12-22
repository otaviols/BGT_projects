# Among Us - Audiogame Multiplayer

Um jogo multiplayer estilo Among Us adaptado para audiogame, desenvolvido com NVGT.

## Estrutura do Projeto

```
among/
├── audio/                    # Sistema de áudio
│   ├── audio_manager.nvgt   # Gerenciador centralizado de áudio
│   └── spatial_audio.nvgt    # Áudio espacial 3D para jogadores
├── config/                   # Configurações
│   ├── game_constants.nvgt  # Constantes do jogo
│   ├── lobby_config.nvgt    # Configurações de lobby
│   └── presets.nvgt         # Presets pré-definidos
├── core/                     # Classes core (a criar)
│   ├── player.nvgt          # Classe Player (a implementar)
│   └── game_state.nvgt      # Gerenciamento de estado (a implementar)
├── database/                 # Persistência
│   ├── user_db.nvgt         # Banco de dados SQLite3
│   └── models.nvgt          # Modelos de dados
├── game/                     # Lógica do jogo
│   ├── map.nvgt             # Sistema de mapa e salas
│   ├── task_manager.nvgt    # Gerenciamento de tasks
│   ├── roles/               # Roles (a criar)
│   └── tasks/               # Tasks individuais (a criar)
├── lang/                     # Internacionalização
│   ├── i18n.nvgt            # Sistema i18n
│   ├── pt_BR.json           # Português Brasil
│   └── en_US.json           # Inglês
├── network/                  # Networking
│   ├── protocol.nvgt        # Definição de protocolo
│   ├── packet.nvgt          # Sistema de pacotes
│   ├── server.nvgt          # Servidor dedicado completo
│   └── client.nvgt          # Cliente (a implementar)
├── ui/                       # Interface de usuário (a criar)
│   ├── menu_manager.nvgt    # Gerenciador de menus
│   ├── lobby_ui.nvgt        # UI de lobby
│   └── task_ui.nvgt         # UI de tasks
├── sounds/                   # Arquivos de áudio (já existe)
├── main.nvgt                # Entry point do cliente (a criar)
└── server_main.nvgt         # Entry point do servidor ✓
```

## Status de Implementação

### ✅ Completo
- Sistema de internacionalização (i18n) com PT-BR e EN-US
- Sistema de autenticação com SQLite3 (login, registro, sessões)
- Modelos de dados (User, PlayerData, LobbyData, TaskData)
- Sistema de configurações de lobby com presets e validação
- Protocolo de rede completo com todos os tipos de mensagens
- Sistema de pacotes com serialização/deserialização
- **Servidor dedicado completo** com:
  - Gerenciamento de lobbies
  - Autenticação de jogadores
  - Atribuição de roles
  - Sistema de tasks
  - Votação e reuniões
  - Kill e mecânicas de impostor
  - Verificação de vitória
- Sistema de áudio com AudioManager e SpatialAudioManager
- Mapa com 8 salas conectadas
- Sistema básico de tasks com definições

### 🚧 A Implementar

#### 1. Cliente do Jogo (network/client.nvgt)
```angelscript
// Precisa implementar:
- Conexão ao servidor
- Envio/recebimento de pacotes
- Sincronização de estado
- Gerenciamento de jogadores remotos
- Input do jogador local
- Interface com UI e áudio
```

#### 2. Classe Player (core/player.nvgt)
```angelscript
class Player {
    // Movimento
    void update(float delta);
    void move(float dx, float dz);
    
    // Interação
    bool can_interact_with_task(int task_id);
    bool can_use_vent();
    
    // Estado
    void kill();
    void report_body(uint64 body_peer_id);
    void call_emergency_meeting();
}
```

#### 3. Sistema de Menus (ui/)
- Menu principal (login, play, stats, settings)
- Lista de lobbies
- Sala de espera
- Menu de tasks (tecla T)
- Menu de votação
- HUD em jogo

#### 4. Roles (game/roles/)
- Crewmate: implementação de comportamento específico
- Impostor: kill, vent, sabotage
- Integração com sistema de tasks

#### 5. Tasks Individuais (game/tasks/)
Criar mini-games para cada task:
- fix_wiring.nvgt: QTE de conectar fios
- download_data.nvgt: Timer de download
- empty_garbage.nvgt: Pressionar sequência
- fuel_engines.nvgt: Multi-step entre salas
- etc.

#### 6. Entry Point do Cliente (main.nvgt)
```angelscript
void main() {
    // Inicializar sistemas
    // Mostrar tela de login
    // Loop de menu principal
    // Conectar ao servidor
    // Loop do jogo
    // Cleanup
}
```

## Como Compilar

### Servidor
```bash
# Compile o servidor
nvgt -c server_main.nvgt -o among_server.exe

# Execute
./among_server.exe
```

### Cliente (quando implementado)
```bash
# Compile o cliente
nvgt -c main.nvgt -o among_client.exe

# Execute
./among_client.exe
```

## Configuração do Banco de Dados

O banco de dados SQLite3 será criado automaticamente na primeira execução do servidor:
- **Arquivo**: `among_users.db`
- **Tabelas**: 
  - `users` - Informações de usuários
  - `user_stats` - Estatísticas de jogo
  - `sessions` - Sessões ativas

## Sons Necessários

Os seguintes arquivos de som devem estar na pasta `sounds/`:

### UI
- click.ogg
- select.ogg
- error.ogg
- success.ogg

### Gameplay
- footstep.ogg
- door_open.ogg
- door_close.ogg
- task_complete.ogg
- emergency_meeting.ogg
- body_reported.ogg
- kill.ogg
- vent_enter.ogg
- vent_exit.ogg

### Votação
- vote_submitted.ogg
- discussion_start.ogg
- voting_start.ogg

### Vitória
- victory_crewmates.ogg
- victory_impostors.ogg

## Controles (Planejados)

- **W, A, S, D**: Movimento
- **E**: Interagir / Usar
- **Tab**: Listar jogadores próximos
- **T**: Abrir menu de tasks
- **K** (Impostor): Kill
- **V** (Impostor): Usar vent
- **R**: Reportar corpo
- **Q**: Chamar reunião de emergência
- **M**: Abrir mapa
- **Esc**: Menu / Sair

## Fluxo do Jogo

1. **Login**: Jogador faz login ou cria conta
2. **Lobby**: 
   - Host cria lobby ou jogador entra em lobby existente
   - Host configura regras
   - Aguarda jogadores (mínimo 4)
   - Host inicia jogo
3. **Jogo**:
   - Roles são atribuídos
   - Crewmates: completar tasks
   - Impostors: eliminar crewmates
   - Reuniões e votações quando acionadas
4. **Fim**: Vitória de crewmates ou impostors
5. **Volta ao lobby** ou **sai do jogo**

## Próximos Passos

1. Implementar cliente básico com conexão ao servidor
2. Criar sistema de menus com menu.nvgt (incluído no NVGT)
3. Implementar movimento e sincronização de posição
4. Criar mini-games para tasks
5. Implementar mecânicas de impostor (kill, vent)
6. Criar sistema de votação no cliente
7. Integrar áudio espacial com movimento
8. Testar multiplayer completo
9. Balanceamento e ajustes
10. Polish (sons, feedback, acessibilidade)

## Arquitetura de Rede

### Servidor (Autoritativo)
- Valida todas as ações
- Gerencia estado do jogo
- Broadcast de eventos para clientes
- Prevenção de cheating server-side

### Cliente
- Envia input para servidor
- Recebe updates de estado
- Client-side prediction para movimento suave
- Renderização de áudio espacial

## Contribuindo

Para contribuir com o projeto:
1. Implemente os componentes marcados como "A Implementar"
2. Teste com múltiplos clientes
3. Adicione comentários e documentação
4. Siga o padrão de código existente

## Licença

[Definir licença do projeto]
