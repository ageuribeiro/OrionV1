namespace Orion.Application.DTOs
{
    /// <summary>
    /// DTO para criação/atualização de usuário
    /// </summary>
    public class CreateUserDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// DTO para resposta de usuário
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public System.DateTime CreatedAt { get; set; }
        public System.DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO para usuário com suas permissões
    /// </summary>
    public class UserWithPermissionsDto
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public System.Collections.Generic.List<string> Permissions { get; set; } = new();
        public System.Collections.Generic.List<string> Roles { get; set; } = new();
    }
}
