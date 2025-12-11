using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.Application.DTOs;
using Orion.Application.UseCases;
using Orion.Domain.Interfaces;

namespace Orion.Presentation.Examples
{
    /// <summary>
    /// EXEMPLO PRÁTICO DE USO DA ARQUITETURA CLEAN
    /// Mostra como utilizar os Use Cases e Serviços de forma prática
    /// </summary>
    public class UserManagementExample
    {
        private readonly UserUseCase _userUseCase;
        private readonly IPermissionService _permissionService;
        private readonly IUserRepository _userRepository;

        public UserManagementExample(
            UserUseCase userUseCase,
            IPermissionService permissionService,
            IUserRepository userRepository)
        {
            _userUseCase = userUseCase;
            _permissionService = permissionService;
            _userRepository = userRepository;
        }

        /// <summary>
        /// EXEMPLO 1: Criar um novo usuário
        /// </summary>
        public async Task Example1_CreateUserAsync()
        {
            Console.WriteLine("=== EXEMPLO 1: Criar Usuário ===");

            var createUserDto = new CreateUserDto
            {
                Username = "joao.silva",
                Email = "joao@example.com",
                Password = "SenhaSegura123!"
            };

            try
            {
                var createdUser = await _userUseCase.CreateUserAsync(createUserDto);
                Console.WriteLine($"✓ Usuário criado: {createdUser.Username} (ID: {createdUser.Id})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// EXEMPLO 2: Consultar usuário com suas permissões
        /// </summary>
        public async Task Example2_GetUserWithPermissionsAsync(int userId)
        {
            Console.WriteLine("\n=== EXEMPLO 2: Consultar Usuário com Permissões ===");

            try
            {
                var userWithPerms = await _userUseCase.GetUserWithPermissionsAsync(userId);
                
                Console.WriteLine($"Usuário: {userWithPerms.Username}");
                Console.WriteLine($"Email: {userWithPerms.Email}");
                Console.WriteLine($"Papéis: {string.Join(", ", userWithPerms.Roles)}");
                Console.WriteLine($"Permissões: {string.Join(", ", userWithPerms.Permissions)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// EXEMPLO 3: Buscar usuários por termo
        /// </summary>
        public async Task Example3_SearchUsersAsync()
        {
            Console.WriteLine("\n=== EXEMPLO 3: Buscar Usuários ===");

            var searchTerm = "joao";
            var users = await _userUseCase.SearchUsersAsync(searchTerm);

            var userList = users.ToList();
            Console.WriteLine($"Encontrados {userList.Count} usuários com '{searchTerm}':");
            
            foreach (var user in userList)
            {
                Console.WriteLine($"  - {user.Username} ({user.Email})");
            }
        }

        /// <summary>
        /// EXEMPLO 4: Verificar permissão de um usuário
        /// </summary>
        public async Task Example4_CheckUserPermissionAsync(int userId, string permissionCode)
        {
            Console.WriteLine($"\n=== EXEMPLO 4: Verificar Permissão '{permissionCode}' ===");

            var hasPermission = await _permissionService.HasPermissionAsync(userId, permissionCode);
            
            if (hasPermission)
                Console.WriteLine($"✓ Usuário tem permissão '{permissionCode}'");
            else
                Console.WriteLine($"✗ Usuário NÃO tem permissão '{permissionCode}'");
        }

        /// <summary>
        /// EXEMPLO 5: Verificar múltiplas permissões (qualquer uma)
        /// </summary>
        public async Task Example5_CheckAnyPermissionAsync(int userId)
        {
            Console.WriteLine("\n=== EXEMPLO 5: Verificar QUALQUER Permissão ===");

            var permissions = new[] { "USER_CREATE", "USER_UPDATE", "ROLE_MANAGE" };
            var hasAnyPermission = await _permissionService.HasAnyPermissionAsync(userId, permissions);

            if (hasAnyPermission)
                Console.WriteLine($"✓ Usuário tem ALGUMA dessas permissões: {string.Join(", ", permissions)}");
            else
                Console.WriteLine($"✗ Usuário NÃO tem NENHUMA dessas permissões");
        }

        /// <summary>
        /// EXEMPLO 6: Verificar múltiplas permissões (todas)
        /// </summary>
        public async Task Example6_CheckAllPermissionsAsync(int userId)
        {
            Console.WriteLine("\n=== EXEMPLO 6: Verificar TODAS as Permissões ===");

            var permissions = new[] { "USER_READ", "USER_UPDATE" };
            var hasAllPermissions = await _permissionService.HasAllPermissionsAsync(userId, permissions);

            if (hasAllPermissions)
                Console.WriteLine($"✓ Usuário tem TODAS essas permissões: {string.Join(", ", permissions)}");
            else
                Console.WriteLine($"✗ Usuário NÃO tem TODAS essas permissões");
        }

        /// <summary>
        /// EXEMPLO 7: Obter todas as permissões de um usuário
        /// </summary>
        public async Task Example7_GetAllUserPermissionsAsync(int userId)
        {
            Console.WriteLine("\n=== EXEMPLO 7: Obter Todas as Permissões do Usuário ===");

            var permissions = await _permissionService.GetUserPermissionsAsync(userId);
            var permList = permissions.ToList();

            Console.WriteLine($"Total de permissões: {permList.Count}");
            foreach (var permission in permList)
            {
                Console.WriteLine($"  - {permission.Code}: {permission.Description}");
            }
        }

        /// <summary>
        /// EXEMPLO 8: Atualizar usuário
        /// </summary>
        public async Task Example8_UpdateUserAsync(int userId)
        {
            Console.WriteLine("\n=== EXEMPLO 8: Atualizar Usuário ===");

            var updateUserDto = new CreateUserDto
            {
                Username = "joao.silva", // Mantém o mesmo
                Email = "joao.silva@newemail.com",
                Password = "NovaSenha123!"
            };

            try
            {
                var updatedUser = await _userUseCase.UpdateUserAsync(userId, updateUserDto);
                Console.WriteLine($"✓ Usuário atualizado: {updatedUser.Email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// EXEMPLO 9: Listar usuários ativos
        /// </summary>
        public async Task Example9_GetActiveUsersAsync()
        {
            Console.WriteLine("\n=== EXEMPLO 9: Listar Usuários Ativos ===");

            var activeUsers = await _userUseCase.GetActiveUsersAsync();
            var userList = activeUsers.ToList();

            Console.WriteLine($"Usuários ativos: {userList.Count}");
            foreach (var user in userList)
            {
                Console.WriteLine($"  - {user.Username} ({user.Email}) - Criado em: {user.CreatedAt:dd/MM/yyyy}");
            }
        }

        /// <summary>
        /// EXEMPLO 10: Desativar usuário (Soft Delete)
        /// </summary>
        public async Task Example10_DeactivateUserAsync(int userId)
        {
            Console.WriteLine("\n=== EXEMPLO 10: Desativar Usuário ===");

            try
            {
                var deactivatedUser = await _userUseCase.DeactivateUserAsync(userId);
                Console.WriteLine($"✓ Usuário desativado: {deactivatedUser.Username}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Erro: {ex.Message}");
            }
        }

        /// <summary>
        /// EXEMPLO 11: Deletar usuário (Hard Delete)
        /// </summary>
        public async Task Example11_DeleteUserAsync(int userId)
        {
            Console.WriteLine("\n=== EXEMPLO 11: Deletar Usuário ===");

            var success = await _userUseCase.DeleteUserAsync(userId);
            
            if (success)
                Console.WriteLine($"✓ Usuário {userId} deletado com sucesso");
            else
                Console.WriteLine($"✗ Falha ao deletar usuário {userId}");
        }

        /// <summary>
        /// EXEMPLO 12: Consultas avançadas com LINQ
        /// </summary>
        public async Task Example12_AdvancedQueriesAsync()
        {
            Console.WriteLine("\n=== EXEMPLO 12: Consultas Avançadas com LINQ ===");

            // Buscar usuário específico
            var user = await _userRepository.GetByUsernameAsync("joao.silva");
            if (user != null)
            {
                Console.WriteLine($"✓ Usuário encontrado: {user.Username}");
            }

            // Verificar se username existe
            var exists = await _userRepository.UsernameExistsAsync("joao.silva");
            Console.WriteLine($"Username 'joao.silva' existe: {exists}");

            // Contar usuários ativos
            var activeCount = await _userRepository.CountAsync(u => u.IsActive);
            Console.WriteLine($"Usuários ativos: {activeCount}");

            // Buscar usuários com termo
            var searchResults = await _userRepository.FindAsync(
                u => u.Email.Contains("@example.com") && u.IsActive
            );
            Console.WriteLine($"Usuários com @example.com: {searchResults.Count()}");
        }
    }
}
