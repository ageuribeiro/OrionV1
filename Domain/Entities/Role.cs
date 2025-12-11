using System;
using System.Collections.Generic;

namespace Orion.Domain.Entities
{
    /// <summary>
    /// Entidade de Papel/Função - Define os papéis disponíveis no sistema
    /// </summary>
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // Relacionamentos
        public virtual List<UserRole> UserRoles { get; set; } = new();
        public virtual List<RolePermission> RolePermissions { get; set; } = new();
    }
}
