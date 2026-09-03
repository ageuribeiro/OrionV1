using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Orion.DatabaseOperations
{
    class OrionDbContext
    {
        private readonly string connectionString;
        public OrionDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }
    }
}
