using System;
using System.Collections.Generic;

namespace Orion.Domain.Entities
{
    /// <summary>
    /// Entidade de Permissão - Define as ações que podem ser realizadas
    /// </summary>
    public class Permission
    {
        public int Id { get; set; }
        public string Code { get; set; }        // Ex: "USER_CREATE", "USER_DELETE"
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Relacionamentos
        public virtual List<RolePermission> RolePermissions { get; set; } = new();
        public virtual List<UserPermission> UserPermissions { get; set; } = new();
    }
}
