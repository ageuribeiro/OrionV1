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
    /// Repositório específico para Funções/Papéis
    /// </summary>
    public class RoleRepository : Repository<Role>, IRoleRepository
    {
        public RoleRepository(OrionDbContext context) : base(context)
        {
        }

        private OrionDbContext OrionContext => _context as OrionDbContext;

        /// <summary>
        /// Obtém papel com suas permissões
        /// </summary>
        public async Task<Role> GetRoleWithPermissionsAsync(int roleId)
        {
            return await OrionContext.Roles
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId);
        }

        /// <summary>
        /// Obtém todos os papéis de um usuário
        /// </summary>
        public async Task<IEnumerable<Role>> GetUserRolesAsync(int userId)
        {
            return await OrionContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .Select(ur => ur.Role)
                .Where(r => r.IsActive)
                .ToListAsync();
        }

        /// <summary>
        /// Verifica se um papel existe pelo nome
        /// </summary>
        public async Task<bool> RoleExistsAsync(string roleName)
        {
            return await OrionContext.Roles
                .AnyAsync(r => r.Name == roleName);
        }

        /// <summary>
        /// Obtém papel por nome
        /// </summary>
        public async Task<Role> GetByNameAsync(string name)
        {
            return await OrionContext.Roles
                .FirstOrDefaultAsync(r => r.Name == name);
        }
    }
}
