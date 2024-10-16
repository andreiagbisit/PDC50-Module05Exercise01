using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Module05Exercise01.Model;
using MySql.Data.MySqlClient;

namespace Module05Exercise01.Services
{
    public class PersonalService
    {
        private readonly string _connectionString;


        public PersonalService()
        {
            var dbService = new DatabaseConnectionService();
            _connectionString = dbService.GetConnectionString();
        }

        public async Task<List<Personal>> GetAllPersonalsAsync()
        {
            var personalService = new List<Personal>();
            using (var conn = new MySqlConnection(_connectionString))
            {
                await conn.OpenAsync();

                //Retrieve Data
                var cmd = new MySqlCommand("SELECT * FROM tblEmployee", conn);

                using (var reader = await cmd.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        personalService.Add(new Personal
                        {
                            EmployeeID = reader.GetInt32("EmployeeID"),
                            Name = reader.GetString("Name"),
                            Address = reader.GetString("Address"),
                            Email = reader.GetString("Email"),
                            ContactNo = reader.GetString("ContactNo")
                        });
                    }
                }
            }
            return personalService;
        }
    }
}
