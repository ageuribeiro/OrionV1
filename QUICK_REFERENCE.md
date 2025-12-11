# 📋 QUICK REFERENCE - Checklist Rápida

Use este documento como referência rápida enquanto implementa.

---

## 🚀 SETUP INICIAL

```bash
# 1. Instalar packages
[ ] dotnet add package Microsoft.EntityFrameworkCore
[ ] dotnet add package Microsoft.EntityFrameworkCore.SqlServer
[ ] dotnet add package Microsoft.EntityFrameworkCore.Tools

# 2. Configurar Program.cs
[ ] Adicionar: using Orion.Infrastructure.DependencyInjection;
[ ] Adicionar: services.AddInfrastructureServices(connectionString);
[ ] Adicionar: await ServiceExtensions.InitializeDatabase(serviceProvider);

# 3. Configurar appsettings.json
[ ] Adicionar ConnectionString "DefaultConnection"
```

---

## 📂 ESTRUTURA QUE VOCÊ TEM

### Domain (Entidades + Interfaces)
```
✅ User           - Usuário
✅ Role           - Papel
✅ Permission     - Permissão
✅ UserRole       - Relacionamento
✅ RolePermission - Relacionamento
✅ UserPermission - Relacionamento

✅ IRepository<T>        - Interface genérica
✅ IUserRepository       - User específico
✅ IRoleRepository       - Role específico
✅ IPermissionRepository - Permission específico
✅ IPermissionService    - Permissões
```

### Application (Use Cases + DTOs)
```
✅ UserUseCase   - Orquestrador
✅ CreateUserDto - Para criar
✅ UserDto       - Para retornar
✅ UserWithPermissionsDto - Com perms
```

### Infrastructure (Repositórios + DB)
```
✅ OrionDbContext         - EF Core
✅ Repository<T>          - Genérico
✅ UserRepository         - User
✅ RoleRepository         - Role
✅ PermissionRepository   - Permission
✅ PermissionService      - Permissões
✅ ServiceExtensions      - DI
```

---

## 🔄 OPERAÇÕES BÁSICAS

### CREATE (Inserir)
```csharp
var newUser = new User { Username = "joao", ... };
await userRepository.AddAsync(newUser);
await userRepository.SaveChangesAsync();
```

### READ (Ler)
```csharp
// Por ID
var user = await userRepository.GetByIdAsync(1);

// Com filtro
var users = await userRepository.FindAsync(u => u.IsActive);

// Primeiro
var user = await userRepository.FirstOrDefaultAsync(u => u.Email == email);

// Verificar existência
bool exists = await userRepository.AnyAsync(u => u.Username == username);

// Contar
int count = await userRepository.CountAsync(u => u.IsActive);
```

### UPDATE (Atualizar)
```csharp
user.Email = "novo@email.com";
await userRepository.UpdateAsync(user);
await userRepository.SaveChangesAsync();
```

### DELETE (Deletar)
```csharp
// Hard delete
await userRepository.DeleteAsync(userId);
await userRepository.SaveChangesAsync();

// Soft delete
user.IsActive = false;
await userRepository.UpdateAsync(user);
await userRepository.SaveChangesAsync();
```

---

## 🔐 PERMISSÕES

### Verificar Uma
```csharp
bool can = await permissionService
    .HasPermissionAsync(userId, "USER_CREATE");

if (can) { /* fazer algo */ }
```

### Verificar Qualquer Uma
```csharp
bool can = await permissionService
    .HasAnyPermissionAsync(userId, 
        "USER_DELETE", "ROLE_MANAGE");

if (can) { /* fazer algo */ }
```

### Verificar Todas
```csharp
bool can = await permissionService
    .HasAllPermissionsAsync(userId,
        "USER_READ", "USER_CREATE");

if (can) { /* fazer algo */ }
```

### Listar Permissões
```csharp
var perms = await permissionService
    .GetUserPermissionsAsync(userId);

foreach (var perm in perms)
{
    Console.WriteLine(perm.Code);
}
```

### Atribuir Permissão
```csharp
bool success = await permissionService
    .AssignPermissionToUserAsync(userId, permissionId);
```

---

## 🎯 USE CASES

### Criar Usuário
```csharp
var dto = new CreateUserDto
{
    Username = "joao",
    Email = "joao@example.com",
    Password = "Senha123!"
};

var user = await userUseCase.CreateUserAsync(dto);
```

### Obter Usuário
```csharp
var user = await userUseCase.GetUserByIdAsync(userId);
```

### Obter com Permissões
```csharp
var user = await userUseCase.GetUserWithPermissionsAsync(userId);

// user.Permissions = [...]
// user.Roles = [...]
```

### Buscar Usuários
```csharp
var users = await userUseCase.SearchUsersAsync("joao");
```

### Usuários Ativos
```csharp
var users = await userUseCase.GetActiveUsersAsync();
```

### Atualizar Usuário
```csharp
var dto = new CreateUserDto { ... };
var updated = await userUseCase.UpdateUserAsync(userId, dto);
```

### Deletar Usuário
```csharp
bool success = await userUseCase.DeleteUserAsync(userId);
```

### Desativar Usuário
```csharp
var user = await userUseCase.DeactivateUserAsync(userId);
```

---

## 💻 EM UM CONTROLLER

```csharp
[ApiController]
[Route("api/users")]
public class UsersController
{
    private readonly UserUseCase _userUseCase;
    private readonly IPermissionService _permissionService;

    public UsersController(
        UserUseCase userUseCase,
        IPermissionService permissionService)
    {
        _userUseCase = userUseCase;
        _permissionService = permissionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        // Verificar permissão
        var canCreate = await _permissionService
            .HasPermissionAsync(GetCurrentUserId(), "USER_CREATE");
        
        if (!canCreate) return Unauthorized();

        // Executar
        var user = await _userUseCase.CreateUserAsync(dto);
        
        // Retornar
        return Created($"/api/users/{user.Id}", user);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userUseCase.GetUserByIdAsync(id);
        return Ok(user);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, CreateUserDto dto)
    {
        var user = await _userUseCase.UpdateUserAsync(id, dto);
        return Ok(user);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var success = await _userUseCase.DeleteUserAsync(id);
        return success ? NoContent() : NotFound();
    }

    private int GetCurrentUserId() => 1; // Implementar
}
```

---

## 📝 EM UMA VIEW (WPF)

```csharp
public partial class UserWindow : Window
{
    private readonly UserUseCase _userUseCase;
    private readonly IPermissionService _permissionService;

    public UserWindow(
        UserUseCase userUseCase,
        IPermissionService permissionService)
    {
        InitializeComponent();
        _userUseCase = userUseCase;
        _permissionService = permissionService;
    }

    private async void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var dto = new CreateUserDto
            {
                Username = UsernameTextBox.Text,
                Email = EmailTextBox.Text,
                Password = PasswordBox.Password
            };

            var user = await _userUseCase.CreateUserAsync(dto);
            MessageBox.Show($"Usuário {user.Username} criado!");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}");
        }
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var users = await _userUseCase.GetActiveUsersAsync();
        UserDataGrid.ItemsSource = users;
    }
}
```

---

## 🗄️ MIGRATIONS

```bash
# Criar migration inicial
dotnet ef migrations add InitialCreate

# Visualizar SQL gerado
dotnet ef migrations script

# Aplicar ao banco
dotnet ef database update

# Criar nova migration após alterar modelo
dotnet ef migrations add AddNewField

# Reverter
dotnet ef database update PreviousMigration

# Remover última (antes de aplicar)
dotnet ef migrations remove
```

---

## 🧪 TESTES BÁSICOS

```csharp
[Fact]
public async Task CreateUser_ShouldCreateSuccessfully()
{
    // Arrange
    var dto = new CreateUserDto { Username = "test", ... };

    // Act
    var result = await userUseCase.CreateUserAsync(dto);

    // Assert
    Assert.NotNull(result);
    Assert.Equal("test", result.Username);
}

[Fact]
public async Task CreateUser_WithDuplicateUsername_ShouldThrow()
{
    // Arrange
    var dto = new CreateUserDto { Username = "existing", ... };
    _userRepositoryMock
        .Setup(r => r.UsernameExistsAsync("existing"))
        .ReturnsAsync(true);

    // Act & Assert
    await Assert.ThrowsAsync<Exception>(() => 
        userUseCase.CreateUserAsync(dto)
    );
}
```

---

## 🔍 LINQ CHEAT SHEET

```csharp
// WHERE - Filtrar
var users = await repository.FindAsync(u => u.IsActive);

// FIRST/FIRSTORDEFAULT - Primeira
var user = await repository.FirstOrDefaultAsync(u => u.Id == 1);

// ANY - Verificar existência
bool exists = await repository.AnyAsync(u => u.Username == "joao");

// COUNT - Contar
int count = await repository.CountAsync(u => u.IsActive);

// SELECT - Transformar
var names = users.Select(u => u.Username);

// SELECTMANY - Flatten
var perms = user.UserRoles
    .SelectMany(ur => ur.Role.RolePermissions)
    .Select(rp => rp.Permission);

// INCLUDE/THENINCLUDE - Eager loading
var user = await context.Users
    .Include(u => u.UserRoles)
        .ThenInclude(ur => ur.Role)
    .FirstOrDefaultAsync(u => u.Id == userId);

// DISTINCT - Remover duplicatas
var distinct = permissions.DistinctBy(p => p.Id);

// ORDER BY - Ordenar
var sorted = users.OrderBy(u => u.Username);

// GROUP BY - Agrupar
var grouped = users.GroupBy(u => u.IsActive);

// JOIN - Juntar
var joined = users.Join(roles, ...);

// UNION - Combinar
var combined = list1.Union(list2);
```

---

## 🚨 ERROS COMUNS

### ❌ "Unable to create an object of type 'OrionDbContext'"
✅ Implementar IDesignTimeDbContextFactory

### ❌ "No database provider configured"
✅ Adicionar UseSqlServer no DbContext

### ❌ "Migration already applied"
✅ Verificar __EFMigrationsHistory

### ❌ "Property is not mapped"
✅ Configurar no OnModelCreating ou ignorar

### ❌ "FK constraint violated"
✅ Inserir pai antes do filho

---

## 📚 DOCUMENTAÇÃO

| Documento | Para... |
|-----------|---------|
| README.md | Overview geral |
| IMPLEMENTATION_GUIDE.cs | Exemplos práticos |
| CLEAN_ARCHITECTURE_GUIDE.md | Detalhes técnicos |
| MIGRATIONS_GUIDE.md | Setup do banco |
| FOLDER_STRUCTURE.md | Navegar código |

---

## ⚡ QUICK WINS

1. **Injetar UserUseCase no seu Controller**
   ```csharp
   public class MyController
   {
       public MyController(UserUseCase userUseCase) { }
   }
   ```

2. **Criar migrations**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

3. **Usar um método**
   ```csharp
   var user = await userUseCase.GetUserByIdAsync(1);
   ```

4. **Verificar permissão**
   ```csharp
   if (await permissionService.HasPermissionAsync(userId, "USER_CREATE"))
   {
       // fazer algo
   }
   ```

5. **Buscar com filtro**
   ```csharp
   var users = await userRepository.FindAsync(u => u.IsActive && u.Email.Contains("@"));
   ```

---

## 🎯 WORKFLOW TÍPICO

```
1. Usuário submete formulário
2. Controller recebe dados
3. Controller chama Use Case
4. Use Case valida e chama Repository
5. Repository executa query LINQ
6. EF Core gera SQL
7. Database retorna dados
8. Repository retorna entidade
9. Use Case retorna DTO
10. Controller retorna resposta
11. View exibe resultado
```

---

## 💡 DICAS IMPORTANTES

- ✅ Sempre usar async/await
- ✅ Sempre injetar via constructor
- ✅ Sempre verificar permissões
- ✅ Sempre fazer SaveChangesAsync()
- ✅ Sempre usar DTOs para transferência
- ✅ Sempre mockar em testes
- ✅ Nunca acessar DB diretamente
- ❌ Nunca usar ToList() sem motivo
- ❌ Nunca misturar camadas

---

**Use esta checklist como referência rápida enquanto implementa!**

Boa programação! 🚀
