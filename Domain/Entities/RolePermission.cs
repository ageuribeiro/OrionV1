namespace Orion.Domain.Entities
{
    /// <summary>
    /// Relacionamento muitos-para-muitos entre Papel e Permissão
    /// </summary>
    public class RolePermission
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        // Navegações
        public virtual Role Role { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
