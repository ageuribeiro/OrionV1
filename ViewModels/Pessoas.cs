using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orion.ViewModels
{
    public class Pessoas
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Sobrenome { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public int Idade { get; set; }
        public string Cpf { get; set; } = string.Empty;
    }
}
