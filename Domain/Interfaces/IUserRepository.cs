using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Orion.Domain.Entities;

namespace Orion.Domain.Interfaces
{
    /// <summary>
    /// Interface específica para repositório de Usuários
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserWithRolesAsync(int userId);
        Task<User> GetUserWithPermissionsAsync(int userId);
        Task<User> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<IEnumerable<User>> GetActiveUsersAsync();
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
    }
}
