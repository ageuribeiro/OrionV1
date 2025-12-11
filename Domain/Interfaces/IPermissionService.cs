using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.Domain.Entities;

namespace Orion.Domain.Interfaces
{
    /// <summary>
    /// Interface para gerenciamento de permissões
    /// </summary>
    public interface IPermissionService
    {
        Task<bool> HasPermissionAsync(int userId, string permissionCode);
        Task<bool> HasAnyPermissionAsync(int userId, params string[] permissionCodes);
        Task<bool> HasAllPermissionsAsync(int userId, params string[] permissionCodes);
        Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId);
        Task<IEnumerable<Permission>> GetRolePermissionsAsync(int roleId);
        Task<bool> AssignPermissionToUserAsync(int userId, int permissionId);
        Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId);
        Task<bool> RemovePermissionFromUserAsync(int userId, int permissionId);
    }
}
