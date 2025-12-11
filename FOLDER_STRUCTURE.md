# 📁 Estrutura de Pastas - Clean Architecture Orion

```
OrionV1/
│
├── 📄 .sln                                  (Solução Visual Studio)
│
├── 📄 CLEAN_ARCHITECTURE_GUIDE.md          ⭐ Documentação completa
├── 📄 IMPLEMENTATION_GUIDE.cs              ⭐ Exemplos de implementação
│
├── 📂 Domain/                              ✅ CAMADA DE DOMÍNIO
│   │
│   ├── Entities/
│   │   ├── User.cs                        (Entidade de Usuário)
│   │   ├── Role.cs                        (Papel/Função)
│   │   ├── Permission.cs                  (Permissão)
│   │   ├── UserRole.cs                    (Relacionamento)
│   │   ├── RolePermission.cs              (Relacionamento)
│   │   └── UserPermission.cs              (Relacionamento)
│   │
│   └── Interfaces/
│       ├── IRepository.cs                 (Interface genérica base)
│       ├── IUserRepository.cs             (Operações de usuário)
│       ├── IRoleRepository.cs             (Operações de papel)
│       ├── IPermissionRepository.cs       (Operações de permissão)
│       └── IPermissionService.cs          (Interface de permissões)
│
├── 📂 Application/                        ✅ CAMADA DE APLICAÇÃO
│   │
│   ├── UseCases/
│   │   └── UserUseCase.cs                (Orquestrador de operações de usuário)
│   │       ├─ CreateUserAsync()
│   │       ├─ GetUserByIdAsync()
│   │       ├─ GetUserWithPermissionsAsync()
│   │       ├─ SearchUsersAsync()
│   │       ├─ UpdateUserAsync()
│   │       ├─ DeleteUserAsync()
│   │       └─ DeactivateUserAsync()
│   │
│   └── DTOs/
│       └── UserDtos.cs
│           ├─ CreateUserDto              (Para criar/editar)
│           ├─ UserDto                    (Para retorno)
│           └─ UserWithPermissionsDto     (Com permissões)
│
├── 📂 Infrastructure/                     ✅ CAMADA DE INFRAESTRUTURA
│   │
│   ├── Data/
│   │   └── OrionDbContext.cs             (Entity Framework Core DbContext)
│   │       ├─ DbSet<User>
│   │       ├─ DbSet<Role>
│   │       ├─ DbSet<Permission>
│   │       ├─ DbSet<UserRole>
│   │       ├─ DbSet<RolePermission>
│   │       └─ DbSet<UserPermission>
│   │
│   ├── Repositories/
│   │   ├── Repository.cs                 (Implementação genérica)
│   │   │   ├─ AddAsync()
│   │   │   ├─ GetByIdAsync()
│   │   │   ├─ FindAsync()
│   │   │   ├─ UpdateAsync()
│   │   │   ├─ DeleteAsync()
│   │   │   └─ SaveChangesAsync()
│   │   │
│   │   ├── UserRepository.cs             (Específico para User)
│   │   │   ├─ GetUserWithRolesAsync()
│   │   │   ├─ GetUserWithPermissionsAsync()
│   │   │   ├─ GetByUsernameAsync()
│   │   │   ├─ UsernameExistsAsync()
│   │   │   ├─ GetActiveUsersAsync()
│   │   │   └─ SearchUsersAsync()
│   │   │
│   │   ├── RoleRepository.cs             (Específico para Role)
│   │   │   ├─ GetRoleWithPermissionsAsync()
│   │   │   ├─ GetUserRolesAsync()
│   │   │   ├─ RoleExistsAsync()
│   │   │   └─ GetByNameAsync()
│   │   │
│   │   └── PermissionRepository.cs       (Específico para Permission)
│   │       ├─ GetByCodeAsync()
│   │       ├─ PermissionExistsAsync()
│   │       └─ GetActivePermissionsAsync()
│   │
│   ├── Security/
│   │   └── PermissionService.cs          (Verificação de Permissões)
│   │       ├─ HasPermissionAsync()
│   │       ├─ HasAnyPermissionAsync()
│   │       ├─ HasAllPermissionsAsync()
│   │       ├─ GetUserPermissionsAsync()
│   │       ├─ GetRolePermissionsAsync()
│   │       ├─ AssignPermissionToUserAsync()
│   │       ├─ AssignPermissionToRoleAsync()
│   │       └─ RemovePermissionFromUserAsync()
│   │
│   └── DependencyInjection/
│       └── ServiceExtensions.cs          (Configuração de DI)
│           ├─ AddInfrastructureServices()
│           └─ InitializeDatabase()
│
├── 📂 Presentation/                       ✅ CAMADA DE APRESENTAÇÃO
│   │
│   ├── UserManagementExample.cs          (Exemplos práticos de uso)
│   │   ├─ Example1_CreateUserAsync()
│   │   ├─ Example2_GetUserWithPermissionsAsync()
│   │   ├─ Example3_SearchUsersAsync()
│   │   ├─ Example4_CheckUserPermissionAsync()
│   │   ├─ Example5_CheckAnyPermissionAsync()
│   │   ├─ Example6_CheckAllPermissionsAsync()
│   │   ├─ Example7_GetAllUserPermissionsAsync()
│   │   ├─ Example8_UpdateUserAsync()
│   │   ├─ Example9_GetActiveUsersAsync()
│   │   ├─ Example10_DeactivateUserAsync()
│   │   ├─ Example11_DeleteUserAsync()
│   │   └─ Example12_AdvancedQueriesAsync()
│   │
│   └── Views/
│       ├── LoginWindow.xaml
│       ├── LoginWindow.xaml.cs
│       ├── DashboardWindow.xaml
│       ├── DashboardWindow.xaml.cs
│       ├── UserListWindow.xaml
│       ├── UserListWindow.xaml.cs
│       ├── MainWindow1.xaml
│       └── MainWindow1.xaml.cs
│
├── 📂 Services/                           (Serviços existentes)
│   └── loginService.cs
│
├── 📂 Assets/                             (Recursos visuais)
│
├── 📂 bin/                                (Compilados)
│
└── 📂 obj/                                (Objetos de compilação)
```

---

## 📊 Mapa de Relacionamentos

```
┌─────────────────┐         ┌──────────────────┐
│     USER        │◄────────┤   USER_ROLE      │
├─────────────────┤         └────────┬─────────┘
│ - Id            │                  │
│ - Username      │                  │
│ - Email         │                  │
│ - PasswordHash  │          ┌────────▼──────┐
│ - IsActive      │          │     ROLE       │
│ - CreatedAt     │          ├────────────────┤
│ - UpdatedAt     │          │ - Id           │
│                 │          │ - Name         │
│ Navegações:     │          │ - Description  │
│ - UserRoles[]   │          │ - IsActive     │
│ - Permissions[] │          │ - CreatedAt    │
└────────┬────────┘          │                │
         │                   │ Navegações:    │
         │                   │ - UserRoles[]  │
         │                   │ - RolePerms[]  │
         │                   └────────┬───────┘
         │                            │
         │              ┌─────────────┼─────────────┐
         │              │                           │
         │        ┌─────▼──────────┐     ┌─────────▼─────┐
         │        │ROLE_PERMISSION │     │USER_PERMISSION│
         │        └────────┬───────┘     └────────┬──────┘
         │                 │                      │
         │                 │                      │
         └─────────────────┼──────────────────────┘
                           │
                      ┌────▼────────────┐
                      │   PERMISSION    │
                      ├─────────────────┤
                      │ - Id            │
                      │ - Code          │
                      │ - Description   │
                      │ - IsActive      │
                      │ - CreatedAt     │
                      │                 │
                      │ Navegações:     │
                      │ - RolePerms[]   │
                      │ - UserPerms[]   │
                      └─────────────────┘
```

---

## 🔗 Fluxo de Injeção de Dependência

```
Program.cs
    ↓
services.AddInfrastructureServices(connectionString)
    ↓
    ├─→ DbContext configurado (SQL Server)
    │
    ├─→ IRepository<T> → Repository<T>
    ├─→ IUserRepository → UserRepository
    ├─→ IRoleRepository → RoleRepository
    ├─→ IPermissionRepository → PermissionRepository
    │
    ├─→ IPermissionService → PermissionService
    │
    └─→ UserUseCase (recebe dependências)
         ├─ IUserRepository
         ├─ IRoleRepository
         └─ IPermissionService
```

---

## 🎯 Como Usar Cada Camada

### **Domain Layer** ✅
- ✏️ Editar: Adicionar novas entidades
- ✏️ Editar: Criar novas interfaces de repositório
- ❌ NÃO editar: Lógica de banco de dados

### **Application Layer** ✅
- ✏️ Editar: Criar novos Use Cases
- ✏️ Editar: Adicionar novos DTOs
- ✏️ Editar: Lógica de orquestração
- ❌ NÃO editar: Acesso direto ao banco

### **Infrastructure Layer** ✅
- ✏️ Editar: Implementar repositórios
- ✏️ Editar: Configurar DbContext
- ✏️ Editar: Adicionar novos serviços
- ❌ NÃO editar: Lógica de negócio

### **Presentation Layer** ✅
- ✏️ Editar: Views e Controllers
- ✏️ Editar: Chamar Use Cases
- ✏️ Editar: Exibir dados
- ❌ NÃO editar: Lógica de negócio

---

## 📋 Checklist de Implementação

```
[ ] 1. Criar entidades no Domain/Entities
[ ] 2. Criar interfaces no Domain/Interfaces
[ ] 3. Criar DbContext em Infrastructure/Data
[ ] 4. Implementar repositórios em Infrastructure/Repositories
[ ] 5. Criar serviços em Infrastructure/Security
[ ] 6. Criar DTOs em Application/DTOs
[ ] 7. Criar Use Cases em Application/UseCases
[ ] 8. Configurar DI em Infrastructure/DependencyInjection
[ ] 9. Usar em Controllers/Views da Presentation
[ ] 10. Criar migrations (dotnet ef migrations add)
[ ] 11. Atualizar banco (dotnet ef database update)
[ ] 12. Testar operações CRUD
[ ] 13. Testar sistema de permissões
```

---

## 💡 Dicas Importantes

1. **DbContext**: Sempre injetado como Scoped
2. **Repositórios**: Sempre injetados como Scoped
3. **Use Cases**: Sempre injetados como Scoped
4. **Serviços**: Podem ser Singleton se stateless
5. **LINQ**: Use com cuidado com `Include()` para evitar N+1
6. **Permissões**: Sempre verificar no serviço de permissão
7. **Transações**: Usar para operações múltiplas relacionadas
8. **Testes**: Fazer mock das interfaces, não das implementações
