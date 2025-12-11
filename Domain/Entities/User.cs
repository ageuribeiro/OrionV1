using System;
using System.Collections.Generic;

namespace Orion.Domain.Entities
{
    /// <summary>
    /// Entidade de Usuário - Representa um usuário no sistema
    /// </summary>
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Relacionamentos
        public virtual List<UserRole> UserRoles { get; set; } = new();
        public virtual List<UserPermission> Permissions { get; set; } = new();
    }
}
