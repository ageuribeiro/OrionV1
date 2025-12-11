using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Orion.Domain.Entities;
using Orion.Domain.Interfaces;
using Orion.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Orion.Infrastructure.Security
{
    /// <summary>
    /// Serviço de Permissões - Centraliza toda a lógica de verificação de permissões
    /// Usa LINQ para consultas complexas no banco de dados
    /// </summary>
    public class PermissionService : IPermissionService
    {
        private readonly OrionDbContext _context;

        public PermissionService(OrionDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Verifica se um usuário tem uma permissão específica
        /// Consulta tanto as permissões diretas quanto as herdadas via papéis
        /// </summary>
        public async Task<bool> HasPermissionAsync(int userId, string permissionCode)
        {
            if (userId <= 0 || string.IsNullOrWhiteSpace(permissionCode))
                return false;

            // Verifica permissão direta do usuário
            var hasDirectPermission = await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId && 
                               up.Permission.Code == permissionCode &&
                               up.Permission.IsActive);

            if (hasDirectPermission)
                return true;

            // Verifica permissão herdada via papel
            var hasRolePermission = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId &&
                               ur.Role.IsActive &&
                               ur.Role.RolePermissions.Any(rp => 
                                   rp.Permission.Code == permissionCode &&
                                   rp.Permission.IsActive));

            return hasRolePermission;
        }

        /// <summary>
        /// Verifica se o usuário tem QUALQUER uma das permissões fornecidas
        /// </summary>
        public async Task<bool> HasAnyPermissionAsync(int userId, params string[] permissionCodes)
        {
            if (userId <= 0 || !permissionCodes.Any())
                return false;

            // Permissões diretas
            var hasDirectPermission = await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId &&
                               permissionCodes.Contains(up.Permission.Code) &&
                               up.Permission.IsActive);

            if (hasDirectPermission)
                return true;

            // Permissões via papel
            var hasRolePermission = await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId &&
                               ur.Role.IsActive &&
                               ur.Role.RolePermissions.Any(rp => 
                                   permissionCodes.Contains(rp.Permission.Code) &&
                                   rp.Permission.IsActive));

            return hasRolePermission;
        }

        /// <summary>
        /// Verifica se o usuário tem TODAS as permissões fornecidas
        /// </summary>
        public async Task<bool> HasAllPermissionsAsync(int userId, params string[] permissionCodes)
        {
            if (userId <= 0 || !permissionCodes.Any())
                return false;

            foreach (var code in permissionCodes)
            {
                var hasPermission = await HasPermissionAsync(userId, code);
                if (!hasPermission)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Obtém todas as permissões de um usuário (diretas + via papéis)
        /// </summary>
        public async Task<IEnumerable<Permission>> GetUserPermissionsAsync(int userId)
        {
            // Permissões diretas
            var directPermissions = await _context.UserPermissions
                .Where(up => up.UserId == userId && up.Permission.IsActive)
                .Select(up => up.Permission)
                .ToListAsync();

            // Permissões via papéis
            var rolePermissions = await _context.UserRoles
                .Where(ur => ur.UserId == userId && ur.Role.IsActive)
                .SelectMany(ur => ur.Role.RolePermissions)
                .Where(rp => rp.Permission.IsActive)
                .Select(rp => rp.Permission)
                .Distinct()
                .ToListAsync();

            // Combine e remova duplicatas
            var allPermissions = directPermissions
                .Union(rolePermissions)
                .DistinctBy(p => p.Id)
                .OrderBy(p => p.Code)
                .ToList();

            return allPermissions;
        }

        /// <summary>
        /// Obtém todas as permissões de um papel
        /// </summary>
        public async Task<IEnumerable<Permission>> GetRolePermissionsAsync(int roleId)
        {
            return await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId && rp.Permission.IsActive)
                .Select(rp => rp.Permission)
                .OrderBy(p => p.Code)
                .ToListAsync();
        }

        /// <summary>
        /// Atribui uma permissão diretamente a um usuário
        /// </summary>
        public async Task<bool> AssignPermissionToUserAsync(int userId, int permissionId)
        {
            // Verifica se já existe
            var exists = await _context.UserPermissions
                .AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId);

            if (exists)
                return false;

            var userPermission = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId
            };

            await _context.UserPermissions.AddAsync(userPermission);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Atribui uma permissão a um papel
        /// </summary>
        public async Task<bool> AssignPermissionToRoleAsync(int roleId, int permissionId)
        {
            // Verifica se já existe
            var exists = await _context.RolePermissions
                .AnyAsync(rp => rp.RoleId == roleId && rp.PermissionId == permissionId);

            if (exists)
                return false;

            var rolePermission = new RolePermission
            {
                RoleId = roleId,
                PermissionId = permissionId
            };

            await _context.RolePermissions.AddAsync(rolePermission);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// Remove uma permissão de um usuário
        /// </summary>
        public async Task<bool> RemovePermissionFromUserAsync(int userId, int permissionId)
        {
            var userPermission = await _context.UserPermissions
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId);

            if (userPermission == null)
                return false;

            _context.UserPermissions.Remove(userPermission);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
