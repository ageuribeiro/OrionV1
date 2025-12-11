using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.Domain.Entities;

namespace Orion.Domain.Interfaces
{
    /// <summary>
    /// Interface para repositório de Funções/Papéis
    /// </summary>
    public interface IRoleRepository : IRepository<Role>
    {
        Task<Role> GetRoleWithPermissionsAsync(int roleId);
        Task<IEnumerable<Role>> GetUserRolesAsync(int userId);
        Task<bool> RoleExistsAsync(string roleName);
        Task<Role> GetByNameAsync(string name);
    }
}
