namespace Orion.Domain.Entities
{
    /// <summary>
    /// Relacionamento muitos-para-muitos entre Usuário e Papel
    /// </summary>
    public class UserRole
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }

        // Navegações
        public virtual User User { get; set; }
        public virtual Role Role { get; set; }
    }
}
