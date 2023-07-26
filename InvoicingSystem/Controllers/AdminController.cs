using InvoicingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace InvoicingSystem.Controllers
{
    public class AdminController : Controller
    {

        private readonly SqlConnection _sqlConnection;

        public AdminController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }

        [HttpGet("GetByUsername/{username}")]
        public IActionResult GetAdmin(string username)
        {
            _sqlConnection.Open();
            var query = "SELECT * FROM Admin WHERE Username = @username";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@Username", username);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return NotFound("User not found.");
            }

            var Admin = new Admin()
            {
                Username = Convert.ToString(dt.Rows[0]["Username"]),
                Password = Convert.ToString(dt.Rows[0]["Password"]),
            };

            return Ok(Admin);
        }

    }
}
