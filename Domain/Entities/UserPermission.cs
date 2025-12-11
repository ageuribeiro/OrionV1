namespace Orion.Domain.Entities
{
    /// <summary>
    /// Relacionamento muitos-para-muitos entre Usuário e Permissão (permissões diretas)
    /// </summary>
    public class UserPermission
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int PermissionId { get; set; }

        // Navegações
        public virtual User User { get; set; }
        public virtual Permission Permission { get; set; }
    }
}
