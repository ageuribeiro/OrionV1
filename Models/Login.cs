using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.Models
{
    internal class Login
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; }
        public string IdNivelAcesso { get; set; } = string.Empty;

        public List<Login> GetUserLogins () 
        {
            List<Login> listaUserLogin = new List<Login>()
            {
                new Login { Id =1, Email = "joao@orion.com", Password = "senha123", IdNivelAcesso = "Admin" },
                new Login{ Id =2,  Email = "josefa@orion.com", Password="senha789", IdNivelAcesso = "User" },
                new Login { Id =3, Email = "maria@orion.com", Password = "senha456", IdNivelAcesso = "User" }
            };

            return listaUserLogin;
        }
    }
}
