using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orion.Domain.Entities;
using Orion.Domain.Interfaces;
using Orion.Infrastructure.Data;

namespace Orion.Infrastructure.Repositories
{
    /// <summary>
    /// Repositório específico para Usuários
    /// Implementa operações específicas de negócio para usuários
    /// </summary>
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(OrionDbContext context) : base(context)
        {
        }

        private OrionDbContext OrionContext => _context as OrionDbContext;

        /// <summary>
        /// Obtém usuário com seus papéis carregados
        /// </summary>
        public async Task<User> GetUserWithRolesAsync(int userId)
        {
            return await OrionContext.Users
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Obtém usuário com suas permissões carregadas (diretas + através de papéis)
        /// </summary>
        public async Task<User> GetUserWithPermissionsAsync(int userId)
        {
            return await OrionContext.Users
                .Include(u => u.Permissions)
                    .ThenInclude(up => up.Permission)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        /// <summary>
        /// Busca usuário por nome de usuário
        /// </summary>
        public async Task<User> GetByUsernameAsync(string username)
        {
            return await OrionContext.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        /// <summary>
        /// Verifica se o nome de usuário já existe
        /// </summary>
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await OrionContext.Users
                .AnyAsync(u => u.Username == username);
        }

        /// <summary>
        /// Obtém todos os usuários ativos
        /// </summary>
        public async Task<IEnumerable<User>> GetActiveUsersAsync()
        {
            return await OrionContext.Users
                .Where(u => u.IsActive)
                .OrderBy(u => u.Username)
                .ToListAsync();
        }

        /// <summary>
        /// Busca usuários por termo de pesquisa (username ou email)
        /// </summary>
        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<User>();

            var term = searchTerm.ToLower();

            return await OrionContext.Users
                .Where(u => u.Username.ToLower().Contains(term) || 
                           u.Email.ToLower().Contains(term))
                .OrderBy(u => u.Username)
                .ToListAsync();
        }
    }
}
