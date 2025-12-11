# Clean Architecture - Sistema Orion

## 📋 Estrutura da Arquitetura

```
┌─────────────────────────────────────────────────────────────┐
│                    PRESENTATION LAYER                        │
│              (Views, ViewModels, Controllers)                │
│                  - LoginWindow.xaml                          │
│                  - DashboardWindow.xaml                      │
│                  - UserManagementExample.cs                  │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                  APPLICATION LAYER                           │
│           (Use Cases, DTOs, Business Logic)                 │
│                  - UserUseCase.cs                           │
│                  - UserDtos.cs                              │
│                  - CreateUserDto                            │
│                  - UserWithPermissionsDto                   │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│                   DOMAIN LAYER                               │
│         (Entities, Interfaces, Business Rules)              │
│                  - User.cs                                  │
│                  - Role.cs                                  │
│                  - Permission.cs                            │
│                  - IRepository<T>                           │
│                  - IUserRepository                          │
│                  - IRoleRepository                          │
│                  - IPermissionService                       │
└──────────────────────┬──────────────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────────────┐
│              INFRASTRUCTURE LAYER                            │
│      (Database, Repositories, External Services)            │
│                  - OrionDbContext.cs                        │
│                  - UserRepository.cs                        │
│                  - RoleRepository.cs                        │
│                  - PermissionRepository.cs                  │
│                  - PermissionService.cs                     │
│                  - ServiceExtensions.cs (DI)                │
└──────────────────────┬──────────────────────────────────────┘
                       │
              ┌────────▼────────┐
              │   DATABASE      │
              │   SQL Server    │
              │   Tables: Users │
              │          Roles  │
              │   Permissions   │
              └─────────────────┘
```

## 🏗️ Camadas Explicadas

### 1️⃣ **DOMAIN LAYER** (Núcleo)
Contém as entidades e regras de negócio puras, independente de frameworks.

**Arquivos:**
- `Domain/Entities/` - Modelos de domínio
  - `User.cs` - Usuário do sistema
  - `Role.cs` - Papéis/Funções
  - `Permission.cs` - Permissões
  - `UserRole.cs` - Relacionamento M:M
  - `RolePermission.cs` - Relacionamento M:M
  - `UserPermission.cs` - Relacionamento M:M

- `Domain/Interfaces/` - Contratos
  - `IRepository<T>` - Interface genérica
  - `IUserRepository` - Operações de usuário
  - `IRoleRepository` - Operações de papel
  - `IPermissionRepository` - Operações de permissão
  - `IPermissionService` - Lógica de permissões

### 2️⃣ **APPLICATION LAYER** (Orquestração)
Contém Use Cases e DTOs que orquestram a lógica de negócio.

**Arquivos:**
- `Application/UseCases/` - Casos de uso
  - `UserUseCase.cs` - Operações de usuário
    - CreateUserAsync()
    - GetUserByIdAsync()
    - SearchUsersAsync()
    - DeleteUserAsync()

- `Application/DTOs/` - Data Transfer Objects
  - `UserDtos.cs`
    - `CreateUserDto` - Para criar/atualizar
    - `UserDto` - Para retornar
    - `UserWithPermissionsDto` - Para permissões

### 3️⃣ **INFRASTRUCTURE LAYER** (Implementação)
Implementações concretas de repositórios e acesso a dados.

**Arquivos:**
- `Infrastructure/Data/`
  - `OrionDbContext.cs` - EF Core DbContext com configurações

- `Infrastructure/Repositories/`
  - `Repository<T>` - Implementação genérica com LINQ
    - GetAllAsync()
    - FindAsync(predicate)
    - AddAsync()
    - UpdateAsync()
    - DeleteAsync()
  - `UserRepository.cs` - Operações específicas
    - GetUserWithRolesAsync()
    - GetUserWithPermissionsAsync()
    - SearchUsersAsync()
  - `RoleRepository.cs`
  - `PermissionRepository.cs`

- `Infrastructure/Security/`
  - `PermissionService.cs` - Verificação de permissões com LINQ
    - HasPermissionAsync()
    - HasAnyPermissionAsync()
    - HasAllPermissionsAsync()
    - GetUserPermissionsAsync()

- `Infrastructure/DependencyInjection/`
  - `ServiceExtensions.cs` - Configuração de DI

### 4️⃣ **PRESENTATION LAYER** (Interface)
Views e exemplos de uso.

**Arquivos:**
- `Presentation/`
  - `UserManagementExample.cs` - Exemplos práticos

---

## 📊 Fluxo de Dados

```
REQUISIÇÃO DO USUÁRIO
        │
        ▼
┌──────────────────────┐
│  PRESENTATION LAYER  │  ← Recebe a requisição
│  (LoginWindow)       │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ APPLICATION LAYER    │  ← Orquestra Use Case
│ (UserUseCase)        │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ DOMAIN LAYER         │  ← Valida regras de negócio
│ (Entities)           │
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ INFRASTRUCTURE LAYER │  ← Executa operação
│ (UserRepository)     │  ← Executa queries LINQ
└──────────┬───────────┘
           │
           ▼
┌──────────────────────┐
│ DATABASE             │  ← Persiste/Recupera dados
│ (SQL Server)         │
└──────────┬───────────┘
           │
        ◄──┴───────────── Retorna resultado
        
RESPOSTA AO USUÁRIO
```

---

## 🔍 Exemplos de LINQ Utilizados

### 1. **Buscar com Includes** (EF Core Loading)
```csharp
var user = await _context.Users
    .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
    .Include(u => u.Permissions)
        .ThenInclude(up => up.Permission)
    .FirstOrDefaultAsync(u => u.Id == userId);
```

### 2. **Verificar Permissão com Any()**
```csharp
var hasPermission = await _context.UserPermissions
    .AnyAsync(up => up.UserId == userId && 
                   up.Permission.Code == permissionCode &&
                   up.Permission.IsActive);
```

### 3. **Buscar com Predicado Dinâmico**
```csharp
var results = await _context.Users
    .Where(u => u.Username.Contains(searchTerm) || 
               u.Email.Contains(searchTerm))
    .OrderBy(u => u.Username)
    .ToListAsync();
```

### 4. **SelectMany para Relacionamentos**
```csharp
var rolePermissions = await _context.UserRoles
    .Where(ur => ur.UserId == userId && ur.Role.IsActive)
    .SelectMany(ur => ur.Role.RolePermissions)
    .Where(rp => rp.Permission.IsActive)
    .Select(rp => rp.Permission)
    .Distinct()
    .ToListAsync();
```

### 5. **Count com Where**
```csharp
var activeCount = await _context.Users
    .CountAsync(u => u.IsActive);
```

---

## 🔐 Sistema de Permissões

### Hierarquia:
```
┌─────────────────────────────────────┐
│          USUÁRIO (User)             │
├─────────────────────────────────────┤
│  ├─ Papéis (Roles) - M:M            │
│  │  ├─ Admin                        │
│  │  │   └─ Permissões (Permissions) │
│  │  │       ├─ USER_CREATE          │
│  │  │       ├─ USER_DELETE          │
│  │  │       └─ ROLE_MANAGE          │
│  │  └─ User                         │
│  │      └─ USER_READ                │
│  │                                  │
│  └─ Permissões Diretas - M:M        │
│      └─ SPECIAL_ACCESS              │
└─────────────────────────────────────┘
```

### Verificações:
```csharp
// Uma permissão específica
bool canCreate = await _permissionService
    .HasPermissionAsync(userId, "USER_CREATE");

// Qualquer uma das permissões
bool canManage = await _permissionService
    .HasAnyPermissionAsync(userId, "USER_DELETE", "ROLE_MANAGE");

// Todas as permissões
bool canFullAccess = await _permissionService
    .HasAllPermissionsAsync(userId, "USER_READ", "USER_CREATE");

// Listar todas
var allPerms = await _permissionService
    .GetUserPermissionsAsync(userId);
```

---

## 🔄 Operações CRUD Completas

### **CREATE** (Inserção)
```csharp
var newUser = new User { ... };
await _userRepository.AddAsync(newUser);
await _userRepository.SaveChangesAsync();
```

### **READ** (Consulta)
```csharp
// Por ID
var user = await _userRepository.GetByIdAsync(1);

// Com predicado
var users = await _userRepository.FindAsync(
    u => u.IsActive && u.Email.Contains("@")
);

// Todos
var all = await _userRepository.GetAllAsync();

// Contagem
var count = await _userRepository.CountAsync();
```

### **UPDATE** (Atualização)
```csharp
user.Email = "newemail@domain.com";
await _userRepository.UpdateAsync(user);
await _userRepository.SaveChangesAsync();
```

### **DELETE** (Exclusão)
```csharp
// Hard delete
await _userRepository.DeleteAsync(userId);
await _userRepository.SaveChangesAsync();

// Soft delete
user.IsActive = false;
await _userRepository.UpdateAsync(user);
await _userRepository.SaveChangesAsync();
```

---

## 💉 Injeção de Dependência

No `Program.cs` ou `Startup.cs`:

```csharp
services.AddInfrastructureServices(connectionString);

// Agora todos os serviços estão disponíveis
var userUseCase = serviceProvider.GetRequiredService<UserUseCase>();
var permissionService = serviceProvider.GetRequiredService<IPermissionService>();
```

---

## ✅ Vantagens desta Arquitetura

| Aspecto | Benefício |
|---------|-----------|
| **Testabilidade** | Interfaces permitem mocks e testes unitários |
| **Manutenibilidade** | Separação de responsabilidades |
| **Escalabilidade** | Fácil adicionar novos repositórios e use cases |
| **Reusabilidade** | DTOs e interfaces podem ser compartilhadas |
| **Independência** | Domain não depende de DB ou frameworks |
| **Flexibilidade** | Trocar banco de dados sem afetar aplicação |

---

## 📝 Próximos Passos

1. ✅ Criar migrations no Entity Framework
2. ✅ Implementar testes unitários
3. ✅ Adicionar validação em DTOs (FluentValidation)
4. ✅ Implementar criptografia de senha
5. ✅ Adicionar logging
6. ✅ Implementar autenticação JWT
7. ✅ Adicionar auditoria
