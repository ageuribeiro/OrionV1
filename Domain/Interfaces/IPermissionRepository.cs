using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.Domain.Entities;

namespace Orion.Domain.Interfaces
{
    /// <summary>
    /// Interface para repositório de Permissões
    /// </summary>
    public interface IPermissionRepository : IRepository<Permission>
    {
        Task<Permission> GetByCodeAsync(string code);
        Task<bool> PermissionExistsAsync(string code);
        Task<IEnumerable<Permission>> GetActivePermissionsAsync();
    }
}
