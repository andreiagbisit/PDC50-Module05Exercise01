using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Module05Exercise01.Services
{
    public class DatabaseConnectionService
    {
        private readonly string _connectionString;

        public DatabaseConnectionService()
        {
            _connectionString = "Server=localhost;Database=companyDB;User ID=testuser2;Password=testuser2";
        }

        public string GetConnectionString()
        {
            return
                _connectionString;
        }
    }
}
