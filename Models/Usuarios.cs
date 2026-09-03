using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orion.Controllers;

namespace Orion.Models
{
    public class Usuarios
    {

        public int Id { get; set; }
        public int IdPessoa { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Senha { get; set; } = string.Empty;
        public string IdNivelAcesso { get; set; } = string.Empty;

        public List<Usuarios> GetUsuarios()
        {
            List<Usuarios> listaUsuarios = new List<Usuarios>()
            {
                new Usuarios { Id =1, Nome = "João", Email = "joao@orion.com", Senha = "senha123", IdNivelAcesso = "Admin" },
                new Usuarios{ Id =2, Nome ="Josefa", Email = "josefa@orion.com", Senha="senha789", IdNivelAcesso = "User" },
                new Usuarios { Id =3, Nome = "Maria", Email = "maria@orion.com", Senha = "senha456", IdNivelAcesso = "User" }
            };

            return listaUsuarios;
        }
    }
}
