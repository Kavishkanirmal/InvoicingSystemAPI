using InvoicingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace InvoicingSystem.Services
{
    public class AdminServices
    {

        private readonly SqlConnection _sqlConnection;

        public AdminServices(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }

        public async Task<Admin> GetAdminByUsername(string username)
        {
            var query = "SELECT * FROM Admin WHERE Username = @username";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@username", username);
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    _sqlConnection.Close();
                    return null; // Return null if the admin is not found.
                }

                var admin = new Admin()
                {
                    Username = Convert.ToString(dt.Rows[0]["Username"]),
                    Password = Convert.ToString(dt.Rows[0]["Password"])
                };

                _sqlConnection.Close();
                return admin;
            }
        }

    }
}
