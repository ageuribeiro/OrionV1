# 🏗️ Clean Architecture - Sistema Orion

Uma implementação completa de **Clean Architecture** em C#.NET com:
- ✅ Sistema de permissões robusto
- ✅ CRUD com LINQ
- ✅ Entity Framework Core
- ✅ Injeção de Dependência
- ✅ Padrão de Repositório

---

## 🎯 Objetivo

Demonstrar uma arquitetura escalável, testável e fácil de manter para aplicações .NET com:
- Separação clara de responsabilidades
- Acesso a dados com LINQ
- Sistema de permissões baseado em Papéis e Permissões
- Exemplos práticos de implementação

---

## 📚 Documentação

| Arquivo | Descrição |
|---------|-----------|
| [CLEAN_ARCHITECTURE_GUIDE.md](CLEAN_ARCHITECTURE_GUIDE.md) | 📖 Guia completo da arquitetura |
| [FOLDER_STRUCTURE.md](FOLDER_STRUCTURE.md) | 📁 Estrutura de pastas explicada |
| [IMPLEMENTATION_GUIDE.cs](IMPLEMENTATION_GUIDE.cs) | 💻 Exemplos de código |
| [MIGRATIONS_GUIDE.md](MIGRATIONS_GUIDE.md) | 🔄 Guia de migrations EF Core |

---

## 🏛️ Camadas da Arquitetura

```
┌─────────────────────────────────────┐
│    PRESENTATION LAYER               │  Views, Controllers
├─────────────────────────────────────┤
│    APPLICATION LAYER                │  Use Cases, DTOs
├─────────────────────────────────────┤
│    DOMAIN LAYER                     │  Entities, Interfaces
├─────────────────────────────────────┤
│    INFRASTRUCTURE LAYER             │  Repositories, DB Context
├─────────────────────────────────────┤
│    DATABASE (SQL Server)            │  Dados persistidos
└─────────────────────────────────────┘
```

---

## 📂 Estrutura do Projeto

```
Domain/
├── Entities/           → User, Role, Permission
└── Interfaces/         → IRepository, IUserRepository, IPermissionService

Application/
├── UseCases/           → UserUseCase
└── DTOs/               → CreateUserDto, UserWithPermissionsDto

Infrastructure/
├── Data/               → OrionDbContext
├── Repositories/       → UserRepository, RoleRepository
├── Security/           → PermissionService
└── DependencyInjection/→ ServiceExtensions

Presentation/
├── Views/              → LoginWindow, DashboardWindow
└── Examples/           → UserManagementExample
```

---

## 🔐 Sistema de Permissões

```
USUÁRIO
├─ Papéis (Roles)
│  └─ Admin → [USER_CREATE, USER_DELETE, ROLE_MANAGE, ...]
│  └─ User  → [USER_READ, ...]
│
└─ Permissões Diretas
   └─ [SPECIAL_ACCESS, ...]
```

### Verificação de Permissões

```csharp
// Uma permissão
bool canCreate = await permissionService
    .HasPermissionAsync(userId, "USER_CREATE");

// Qualquer uma
bool canManage = await permissionService
    .HasAnyPermissionAsync(userId, "USER_DELETE", "ROLE_MANAGE");

// Todas
bool fullAccess = await permissionService
    .HasAllPermissionsAsync(userId, "USER_READ", "USER_CREATE");
```

---

## 🔄 Operações CRUD com LINQ

### CREATE (Inserção)
```csharp
var user = new User { Username = "joao", Email = "joao@test.com", ... };
await userRepository.AddAsync(user);
await userRepository.SaveChangesAsync();
```

### READ (Consulta)
```csharp
var user = await userRepository.GetByIdAsync(1);
var users = await userRepository.FindAsync(u => u.IsActive);
var count = await userRepository.CountAsync();
```

### UPDATE (Atualização)
```csharp
user.Email = "novo@email.com";
await userRepository.UpdateAsync(user);
await userRepository.SaveChangesAsync();
```

### DELETE (Exclusão)
```csharp
await userRepository.DeleteAsync(userId);
await userRepository.SaveChangesAsync();
```

---

## 💡 Exemplos de LINQ Utilizados

```csharp
// Include com ThenInclude
var user = await context.Users
    .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
    .FirstOrDefaultAsync(u => u.Id == userId);

// Any para verificação
var exists = await context.Users
    .AnyAsync(u => u.Username == "joao");

// Where com múltiplas condições
var active = await context.Users
    .Where(u => u.IsActive && u.CreatedAt > DateTime.Now.AddMonths(-1))
    .ToListAsync();

// SelectMany para relacionamentos
var permissions = await context.UserRoles
    .Where(ur => ur.UserId == userId)
    .SelectMany(ur => ur.Role.RolePermissions)
    .Select(rp => rp.Permission)
    .ToListAsync();

// Count com predicado
var count = await context.Users
    .CountAsync(u => u.IsActive);
```

---

## 🚀 Quick Start

### 1. Instalar Packages

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

### 2. Configurar Program.cs

```csharp
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructureServices(connectionString);
```

### 3. Criar Migrations

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 4. Usar em Controller

```csharp
[ApiController]
[Route("api/users")]
public class UsersController
{
    private readonly UserUseCase _userUseCase;
    
    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var user = await _userUseCase.CreateUserAsync(dto);
        return Ok(user);
    }
}
```

---

## ✨ Vantagens

| Aspecto | Benefício |
|---------|-----------|
| **Testabilidade** | Fácil mockar dependências |
| **Manutenibilidade** | Código organizado e claro |
| **Escalabilidade** | Simples adicionar novos features |
| **Reusabilidade** | DTOs e interfaces compartilháveis |
| **Independência** | Domain não depende de frameworks |
| **Flexibilidade** | Trocar implementações facilmente |

---

## 📖 Documentação Completa

Consulte os arquivos de documentação para mais detalhes:

1. **[CLEAN_ARCHITECTURE_GUIDE.md](CLEAN_ARCHITECTURE_GUIDE.md)** - Guia detalhado de todas as camadas
2. **[FOLDER_STRUCTURE.md](FOLDER_STRUCTURE.md)** - Estrutura de pastas e como usar
3. **[IMPLEMENTATION_GUIDE.cs](IMPLEMENTATION_GUIDE.cs)** - Exemplos práticos de código
4. **[MIGRATIONS_GUIDE.md](MIGRATIONS_GUIDE.md)** - Guia de Entity Framework Migrations

---

## 🔍 Principais Classes

### Domain

- **User** - Usuário do sistema
- **Role** - Papéis/Funções
- **Permission** - Permissões de acesso
- **IRepository<T>** - Interface genérica
- **IPermissionService** - Verificação de permissões

### Application

- **UserUseCase** - Orquestrador de operações
- **CreateUserDto** - DTO de entrada
- **UserWithPermissionsDto** - DTO com permissões

### Infrastructure

- **OrionDbContext** - EF Core DbContext
- **UserRepository** - Operações de User
- **PermissionService** - Lógica de permissões
- **ServiceExtensions** - Configuração de DI

---

## 📋 Checklist de Implementação

```
[ ] Entender as 4 camadas da arquitetura
[ ] Criar entidades no Domain
[ ] Criar interfaces de repositório
[ ] Implementar repositórios genéricos
[ ] Criar Use Cases na Application
[ ] Criar DTOs para transferência de dados
[ ] Configurar DbContext
[ ] Implementar injeção de dependência
[ ] Criar migrations EF Core
[ ] Testar operações CRUD
[ ] Testar sistema de permissões
[ ] Implementar em Controller/View
```

---

## 🎓 O Que Você Aprenderá

✅ Separação de responsabilidades em 4 camadas  
✅ Padrão de Repositório com generics  
✅ Entity Framework Core com LINQ  
✅ Sistema de permissões completo  
✅ Injeção de Dependência  
✅ DTOs e mapeamento  
✅ Use Cases e orquestração  
✅ Migrations e versionamento de DB  

---

## 🤝 Contribuir

Sugestões de melhorias:

1. Adicionar FluentValidation para DTOs
2. Implementar testes unitários
3. Adicionar logging
4. Implementar auditoria
5. Adicionar cache
6. Implementar CQRS

---

## 📞 Suporte

Para dúvidas sobre a implementação, consulte:

- [Microsoft Docs - Clean Architecture](https://docs.microsoft.com/en-us/dotnet/architecture/clean-code)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [SOLID Principles](https://en.wikipedia.org/wiki/SOLID)

---

## 📄 Licença

Este projeto é fornecido como exemplo educacional.

---

## 🎉 Conclusão

Esta estrutura de Clean Architecture fornece uma base sólida para qualquer aplicação .NET que necessite:

- ✅ Escalabilidade
- ✅ Manutenibilidade  
- ✅ Testabilidade
- ✅ Flexibilidade

Siga os guias de documentação e comece a construir seu projeto hoje!

**Boa sorte! 🚀**
