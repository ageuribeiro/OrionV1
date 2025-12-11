using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.Application.DTOs;
using Orion.Domain.Entities;
using Orion.Domain.Interfaces;

namespace Orion.Application.UseCases
{
    /// <summary>
    /// Use Case para gerenciar usuários
    /// Implementa as operações de negócio relacionadas a usuários
    /// </summary>
    public class UserUseCase
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPermissionService _permissionService;

        public UserUseCase(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPermissionService permissionService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _permissionService = permissionService;
        }

        /// <summary>
        /// Cria um novo usuário
        /// </summary>
        public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Valida se usuário já existe
            if (await _userRepository.UsernameExistsAsync(createUserDto.Username))
                throw new Exception("Username já existe");

            var user = new User
            {
                Username = createUserDto.Username,
                Email = createUserDto.Email,
                PasswordHash = HashPassword(createUserDto.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var createdUser = await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToUserDto(createdUser);
        }

        /// <summary>
        /// Obtém um usuário por ID
        /// </summary>
        public async Task<UserDto> GetUserByIdAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception($"Usuário {userId} não encontrado");

            return MapToUserDto(user);
        }

        /// <summary>
        /// Obtém usuário com todas as suas permissões
        /// </summary>
        public async Task<UserWithPermissionsDto> GetUserWithPermissionsAsync(int userId)
        {
            var user = await _userRepository.GetUserWithPermissionsAsync(userId);
            if (user == null)
                throw new Exception($"Usuário {userId} não encontrado");

            var permissions = await _permissionService.GetUserPermissionsAsync(userId);
            var roles = await _roleRepository.GetUserRolesAsync(userId);

            return new UserWithPermissionsDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Permissions = new List<string>(permissions.Select(p => p.Code)),
                Roles = new List<string>(roles.Select(r => r.Name))
            };
        }

        /// <summary>
        /// Busca usuários por termo de pesquisa
        /// </summary>
        public async Task<IEnumerable<UserDto>> SearchUsersAsync(string searchTerm)
        {
            var users = await _userRepository.SearchUsersAsync(searchTerm);
            return users.Select(MapToUserDto);
        }

        /// <summary>
        /// Lista todos os usuários ativos
        /// </summary>
        public async Task<IEnumerable<UserDto>> GetActiveUsersAsync()
        {
            var users = await _userRepository.GetActiveUsersAsync();
            return users.Select(MapToUserDto);
        }

        /// <summary>
        /// Atualiza um usuário existente
        /// </summary>
        public async Task<UserDto> UpdateUserAsync(int userId, CreateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception($"Usuário {userId} não encontrado");

            user.Email = updateUserDto.Email;
            if (!string.IsNullOrWhiteSpace(updateUserDto.Password))
                user.PasswordHash = HashPassword(updateUserDto.Password);
            
            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToUserDto(updatedUser);
        }

        /// <summary>
        /// Deleta um usuário
        /// </summary>
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var success = await _userRepository.DeleteAsync(userId);
            if (success)
                await _userRepository.SaveChangesAsync();

            return success;
        }

        /// <summary>
        /// Desativa um usuário (soft delete)
        /// </summary>
        public async Task<UserDto> DeactivateUserAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception($"Usuário {userId} não encontrado");

            user.IsActive = false;
            user.UpdatedAt = DateTime.UtcNow;

            var updatedUser = await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return MapToUserDto(updatedUser);
        }

        /// <summary>
        /// Atribui um papel a um usuário
        /// </summary>
        public async Task<bool> AssignRoleToUserAsync(int userId, int roleId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception($"Usuário {userId} não encontrado");

            var role = await _roleRepository.GetByIdAsync(roleId);
            if (role == null)
                throw new Exception($"Papel {roleId} não encontrado");

            var userRole = new UserRole
            {
                UserId = userId,
                RoleId = roleId
            };

            // Aqui você precisaria de um IRepository<UserRole>
            // Por enquanto, você pode usar o context diretamente
            // var existingUserRole = await _context.UserRoles
            //     .FirstOrDefaultAsync(ur => ur.UserId == userId && ur.RoleId == roleId);
            // if (existingUserRole != null) return false;

            // await _context.UserRoles.AddAsync(userRole);
            // await _context.SaveChangesAsync();

            return true;
        }

        // Helper methods
        private string HashPassword(string password)
        {
            // Implementar hashing seguro (BCrypt, PBKDF2, etc)
            // Exemplo com System.Security.Cryptography:
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashedBytes);
            }
        }

        private UserDto MapToUserDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
