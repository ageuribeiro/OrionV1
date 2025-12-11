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
    /// Repositório específico para Permissões
    /// </summary>
    public class PermissionRepository : Repository<Permission>, IPermissionRepository
    {
        public PermissionRepository(OrionDbContext context) : base(context)
        {
        }

        private OrionDbContext OrionContext => _context as OrionDbContext;

        /// <summary>
        /// Obtém permissão pelo código
        /// </summary>
        public async Task<Permission> GetByCodeAsync(string code)
        {
            return await OrionContext.Permissions
                .FirstOrDefaultAsync(p => p.Code == code);
        }

        /// <summary>
        /// Verifica se uma permissão existe pelo código
        /// </summary>
        public async Task<bool> PermissionExistsAsync(string code)
        {
            return await OrionContext.Permissions
                .AnyAsync(p => p.Code == code);
        }

        /// <summary>
        /// Obtém todas as permissões ativas
        /// </summary>
        public async Task<IEnumerable<Permission>> GetActivePermissionsAsync()
        {
            return await OrionContext.Permissions
                .Where(p => p.IsActive)
                .OrderBy(p => p.Code)
                .ToListAsync();
        }
    }
}
