using InvoicingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.JsonPatch;

namespace InvoicingSystem.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {

        private readonly SqlConnection _sqlConnection;

        public CustomerController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> AddCustomer([FromBody] Customer customer)
        {
            var query = "INSERT INTO Customers (CustomerName, Email, Address, ContactNo, DateOfBirth, Gender) VALUES (@customerName, @Email, @Address, @ContactNo, @DateOfBirth, @Gender)";
            _sqlConnection.Open();
            var cmd = new SqlCommand(query, _sqlConnection);

            cmd.Parameters.AddWithValue("CustomerName", customer.CustomerName);
            cmd.Parameters.AddWithValue("Email", customer.Email);
            cmd.Parameters.AddWithValue("Address", customer.Address);
            cmd.Parameters.AddWithValue("ContactNo", customer.ContactNo);
            cmd.Parameters.AddWithValue("DateOfBirth", customer.DateOfBirth);
            cmd.Parameters.AddWithValue("Gender", customer.Gender);

            var result = await cmd.ExecuteNonQueryAsync();
            return Ok(result);

        }


        [HttpGet("Get")]
        public IActionResult GetCustomers()
        {
            var query = "SELECT * FROM Customers";
            _sqlConnection.Open();
            var cmd = new SqlCommand(query, _sqlConnection);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            var customers = new List<Customer>();
            foreach (DataRow dr in dt.Rows)
            {
                customers.Add(new Customer()
                {
                    CustomerId = Convert.ToInt32(dr["CustomerId"]),
                    CustomerName = Convert.ToString(dr["CustomerName"]),
                    Email = Convert.ToString(dr["Email"]),
                    ContactNo = Convert.ToString(dr["ContactNo"]),
                    DateOfBirth = Convert.ToDateTime(dr["DateOfBirth"]),
                    Gender = Convert.ToString(dr["Gender"])
                });
            }
            return Ok(customers);
        }


        [HttpGet("GetById/{customerId}")]
        public IActionResult GetCustomerById(int customerId)
        {
            _sqlConnection.Open();
            var query = "SELECT * FROM Customers WHERE CustomerId = @CustomerId";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return NotFound("Customer not found.");
            }

            var customer = new Customer()
            {
                CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]),
                CustomerName = Convert.ToString(dt.Rows[0]["CustomerName"]),
                Email = Convert.ToString(dt.Rows[0]["Email"]),
                ContactNo = Convert.ToString(dt.Rows[0]["ContactNo"]),
                DateOfBirth = Convert.ToDateTime(dt.Rows[0]["DateOfBirth"]),
                Gender = Convert.ToString(dt.Rows[0]["Gender"])
            };

            return Ok(customer);
        }


        [HttpDelete("Delete/{customerId}")]
        public IActionResult DeleteCustomer(int customerId)
        {
            _sqlConnection.Open();
            var query = "DELETE FROM Customers WHERE CustomerId = @CustomerId";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok("Customer Deleted!");
            }
            else
            {
                return NotFound("Customer not found.");
            }
        }


        [HttpPatch("UpdatePartial/{customerId}")]
        public IActionResult UpdateCustomerPartial(int customerId, [FromBody] JsonPatchDocument<Customer> patchedCustomer)
        {
            if (patchedCustomer == null || customerId <= 0)
                return BadRequest();


            _sqlConnection.Open();
            var query = "SELECT * FROM Customers WHERE CustomerId = @CustomerId";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return NotFound("Customer not found.");
            }

            var existingCustomer = new Customer()
            {
                CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]),
                CustomerName = Convert.ToString(dt.Rows[0]["CustomerName"]),
                Email = Convert.ToString(dt.Rows[0]["Email"]),
                ContactNo = Convert.ToString(dt.Rows[0]["ContactNo"]),
                DateOfBirth = Convert.ToDateTime(dt.Rows[0]["DateOfBirth"]),
                Gender = Convert.ToString(dt.Rows[0]["Gender"])
            };

            patchedCustomer.ApplyTo(existingCustomer, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            query = "UPDATE Customers SET CustomerName = @CustomerName, Email = @Email, ContactNo = @ContactNo, DateOfBirth = @DateOfBirth, Gender = @Gender WHERE CustomerId = @CustomerId";
            cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@CustomerName", existingCustomer.CustomerName);
            cmd.Parameters.AddWithValue("@Email", existingCustomer.Email);
            cmd.Parameters.AddWithValue("@ContactNo", existingCustomer.ContactNo);
            cmd.Parameters.AddWithValue("@DateOfBirth", existingCustomer.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gender", existingCustomer.Gender);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok("Customer Updated!");
            }
            else
            {
                return NotFound("Customer not found.");
            }

        }


        [HttpPut("Update/{customerId}")]
        public IActionResult UpdateCustomer(int customerId, [FromBody] Customer updatedCustomer)
        {
            if (updatedCustomer == null)
            {
                return BadRequest("Invalid data provided for updating the customer.");
            }

            _sqlConnection.Open();
            var query = "UPDATE Customers SET CustomerName = @CustomerName, Email = @Email, ContactNo = @ContactNo, DateOfBirth = @DateOfBirth, Gender = @Gender WHERE CustomerId = @CustomerId";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@CustomerName", updatedCustomer.CustomerName);
            cmd.Parameters.AddWithValue("@Email", updatedCustomer.Email);
            cmd.Parameters.AddWithValue("@ContactNo", updatedCustomer.ContactNo);
            cmd.Parameters.AddWithValue("@DateOfBirth", updatedCustomer.DateOfBirth);
            cmd.Parameters.AddWithValue("@Gender", updatedCustomer.Gender);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok("Customer Updated!");
            }
            else
            {
                return NotFound("Customer not found.");
            }
        }

    }
}
