// ╔════════════════════════════════════════════════════════════════════════════╗
// ║                   GUIA RÁPIDO DE IMPLEMENTAÇÃO                             ║
// ║              Clean Architecture com Permissões em C#.NET                   ║
// ╚════════════════════════════════════════════════════════════════════════════╝

// ========== 1. INSTALAR PACKAGES ==========
/*
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add package Microsoft.Extensions.DependencyInjection
*/

// ========== 2. CONFIGURAR NO Program.cs ==========

using Microsoft.EntityFrameworkCore;
using Orion.Infrastructure.DependencyInjection;

var builder = WebApplicationBuilder.CreateBuilder(args);

// Adicionar serviços de infraestrutura
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddInfrastructureServices(connectionString);

// Adicionar CORS, Controllers, etc...
builder.Services.AddControllers();

var app = builder.Build();

// Inicializar banco de dados
using (var scope = app.Services.CreateScope())
{
    await ServiceExtensions.InitializeDatabase(scope.ServiceProvider);
}

app.Run();

// ========== 3. USAR EM UM CONTROLLER ==========

using Microsoft.AspNetCore.Mvc;
using Orion.Application.DTOs;
using Orion.Application.UseCases;
using Orion.Domain.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
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

    // ✅ CRIAR USUÁRIO
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(CreateUserDto createDto)
    {
        // Verificar permissão
        var canCreate = await _permissionService
            .HasPermissionAsync(GetCurrentUserId(), "USER_CREATE");
        
        if (!canCreate)
            return Unauthorized();

        var user = await _userUseCase.CreateUserAsync(createDto);
        return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
    }

    // ✅ OBTER USUÁRIO
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUser(int id)
    {
        var canRead = await _permissionService
            .HasPermissionAsync(GetCurrentUserId(), "USER_READ");
        
        if (!canRead)
            return Unauthorized();

        var user = await _userUseCase.GetUserByIdAsync(id);
        return Ok(user);
    }

    // ✅ OBTER COM PERMISSÕES
    [HttpGet("{id}/with-permissions")]
    public async Task<ActionResult<UserWithPermissionsDto>> GetUserWithPermissions(int id)
    {
        var user = await _userUseCase.GetUserWithPermissionsAsync(id);
        return Ok(user);
    }

    // ✅ BUSCAR USUÁRIOS
    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserDto>>> SearchUsers(string term)
    {
        var users = await _userUseCase.SearchUsersAsync(term);
        return Ok(users);
    }

    // ✅ ATUALIZAR USUÁRIO
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, CreateUserDto updateDto)
    {
        var canUpdate = await _permissionService
            .HasPermissionAsync(GetCurrentUserId(), "USER_UPDATE");
        
        if (!canUpdate)
            return Unauthorized();

        var user = await _userUseCase.UpdateUserAsync(id, updateDto);
        return Ok(user);
    }

    // ✅ DELETAR USUÁRIO
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var canDelete = await _permissionService
            .HasPermissionAsync(GetCurrentUserId(), "USER_DELETE");
        
        if (!canDelete)
            return Unauthorized();

        var success = await _userUseCase.DeleteUserAsync(id);
        return success ? NoContent() : NotFound();
    }

    // ✅ DESATIVAR USUÁRIO (Soft Delete)
    [HttpPut("{id}/deactivate")]
    public async Task<ActionResult<UserDto>> DeactivateUser(int id)
    {
        var user = await _userUseCase.DeactivateUserAsync(id);
        return Ok(user);
    }

    private int GetCurrentUserId()
    {
        // Implementar extração do usuário atual do JWT/Session
        return 1; // Placeholder
    }
}

// ========== 4. USAR EM UMA VIEW (WPF/Xaml) ==========

using System.Windows;
using Orion.Application.DTOs;
using Orion.Application.UseCases;
using Orion.Domain.Interfaces;

public partial class UserManagementWindow : Window
{
    private readonly UserUseCase _userUseCase;
    private readonly IPermissionService _permissionService;

    public UserManagementWindow(
        UserUseCase userUseCase,
        IPermissionService permissionService)
    {
        InitializeComponent();
        _userUseCase = userUseCase;
        _permissionService = permissionService;
    }

    // Botão Criar Usuário
    private async void CreateButton_Click(object sender, RoutedEventArgs e)
    {
        var dto = new CreateUserDto
        {
            Username = UsernameTextBox.Text,
            Email = EmailTextBox.Text,
            Password = PasswordBox.Password
        };

        try
        {
            var newUser = await _userUseCase.CreateUserAsync(dto);
            MessageBox.Show($"Usuário {newUser.Username} criado com sucesso!");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Erro: {ex.Message}");
        }
    }

    // Carregar lista de usuários
    private async void Window_Loaded(object sender, RoutedEventArgs e)
    {
        var users = await _userUseCase.GetActiveUsersAsync();
        UserDataGrid.ItemsSource = users;
    }

    // Verificar permissão antes de deletar
    private async void DeleteButton_Click(object sender, RoutedEventArgs e)
    {
        var canDelete = await _permissionService
            .HasPermissionAsync(GetCurrentUserId(), "USER_DELETE");

        if (!canDelete)
        {
            MessageBox.Show("Você não tem permissão para deletar usuários!");
            return;
        }

        if (UserDataGrid.SelectedItem is UserDto selectedUser)
        {
            var success = await _userUseCase.DeleteUserAsync(selectedUser.Id);
            if (success)
                MessageBox.Show("Usuário deletado com sucesso!");
        }
    }

    private int GetCurrentUserId() => 1; // Placeholder
}

// ========== 5. TESTES UNITÁRIOS ==========

using Xunit;
using Moq;
using Orion.Application.UseCases;
using Orion.Application.DTOs;
using Orion.Domain.Interfaces;

public class UserUseCaseTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;
    private readonly Mock<IPermissionService> _permissionServiceMock;
    private readonly UserUseCase _userUseCase;

    public UserUseCaseTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _permissionServiceMock = new Mock<IPermissionService>();

        _userUseCase = new UserUseCase(
            _userRepositoryMock.Object,
            _roleRepositoryMock.Object,
            _permissionServiceMock.Object
        );
    }

    [Fact]
    public async Task CreateUser_WithValidData_ShouldCreateSuccessfully()
    {
        // Arrange
        var createDto = new CreateUserDto
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123"
        };

        _userRepositoryMock
            .Setup(r => r.UsernameExistsAsync("testuser"))
            .ReturnsAsync(false);

        // Act
        var result = await _userUseCase.CreateUserAsync(createDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
    }

    [Fact]
    public async Task CreateUser_WithDuplicateUsername_ShouldThrow()
    {
        // Arrange
        var createDto = new CreateUserDto { Username = "existing" };

        _userRepositoryMock
            .Setup(r => r.UsernameExistsAsync("existing"))
            .ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => 
            _userUseCase.CreateUserAsync(createDto)
        );
    }
}

// ========== 6. QUERIES LINQ COMUNS ==========

// Buscar usuário com filtro
var activeUsers = await _userRepository.FindAsync(u => u.IsActive);

// Verificar existência
var exists = await _userRepository.AnyAsync(u => u.Username == "joao");

// Contar com filtro
var count = await _userRepository.CountAsync(u => u.IsActive);

// Primeiro ou padrão
var user = await _userRepository.FirstOrDefaultAsync(u => u.Email == "test@test.com");

// Busca com múltiplas condições
var results = await _userRepository.FindAsync(u =>
    u.IsActive &&
    u.CreatedAt > DateTime.Now.AddMonths(-1) &&
    (u.Username.Contains("admin") || u.Email.Contains("admin"))
);

// ========== 7. PADRÕES COMUNS ==========

// ✅ VERIFICAR PERMISSÃO E EXECUTAR
if (await _permissionService.HasPermissionAsync(userId, "USER_DELETE"))
{
    await _userUseCase.DeleteUserAsync(userId);
}

// ✅ BUSCAR E VALIDAR
var user = await _userUseCase.GetUserByIdAsync(id);
if (user == null)
    throw new NotFoundException($"Usuário {id} não encontrado");

// ✅ BUSCAR COM RELACIONAMENTOS
var userWithPerms = await _userRepository.GetUserWithPermissionsAsync(userId);
var roleNames = userWithPerms.UserRoles.Select(ur => ur.Role.Name);

// ✅ TRANSAÇÕES
using (var transaction = await _context.Database.BeginTransactionAsync())
{
    try
    {
        await _userRepository.AddAsync(newUser);
        await _permissionService.AssignPermissionToUserAsync(userId, permId);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    catch
    {
        await transaction.RollbackAsync();
        throw;
    }
}

// ========== 8. MIGRATIONS ==========

/*
// Criar migration
dotnet ef migrations add InitialCreate

// Atualizar banco
dotnet ef database update

// Remover última migration
dotnet ef migrations remove
*/

// ╔════════════════════════════════════════════════════════════════════════════╗
// ║                           ESTRUTURA FINAL                                  ║
// ║                                                                            ║
// ║  ✅ Domain Layer       → Entidades + Interfaces                           ║
// ║  ✅ Application Layer  → Use Cases + DTOs                                 ║
// ║  ✅ Infrastructure     → Repositórios + DbContext + DI                    ║
// ║  ✅ Presentation       → Controllers + Views                              ║
// ║                                                                            ║
// ║  🔐 Sistema de Permissões Completo                                        ║
// ║  🔄 CRUD com LINQ                                                         ║
// ║  💉 Injeção de Dependência                                                ║
// ║  ✔️  Testável e Escalável                                                 ║
// ╚════════════════════════════════════════════════════════════════════════════╝
