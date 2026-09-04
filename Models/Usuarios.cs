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


        public List<Usuarios> GetByName(string nome)
        {
            var users = GetUsuarios();
            var user = from person in users
                        where person.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase)
                        select person;
            return user.ToList();
        }

        public List<Usuarios> GetUsuarios()
        {
            var listaUsuarios = new List<Usuarios>();

            for (int i=0; i< 20; i++)
            {
                listaUsuarios.Add(new Usuarios
                {
                    Id = 1,
                    Nome = "João",
                    Email = "joao@orion.com",
                    Senha = "senha123",
                    IdNivelAcesso = "Admin"
                });
            };

            return listaUsuarios;
        }
    }
}
