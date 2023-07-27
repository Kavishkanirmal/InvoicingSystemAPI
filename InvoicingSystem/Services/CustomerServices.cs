using InvoicingSystem.Models;
using Microsoft.AspNetCore.JsonPatch;
using System.Data;
using System.Data.SqlClient;

namespace InvoicingSystem.Services
{
    public class CustomerServices
    {

        private readonly SqlConnection _sqlConnection;

        public CustomerServices(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }

        public async Task<bool> AddCustomer(Customer customer)
        {
            var query = "INSERT INTO Customers (CustomerName, Email, Address, ContactNo, DateOfBirth, Gender) VALUES (@CustomerName, @Email, @Address, @ContactNo, @DateOfBirth, @Gender)";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@CustomerName", customer.CustomerName);
                cmd.Parameters.AddWithValue("@Email", customer.Email);
                cmd.Parameters.AddWithValue("@Address", customer.Address);
                cmd.Parameters.AddWithValue("@ContactNo", customer.ContactNo);
                cmd.Parameters.AddWithValue("@DateOfBirth", customer.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", customer.Gender);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

        public async Task<List<Customer>> GetCustomers()
        {
            var query = "SELECT * FROM Customers";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
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
                        Address = Convert.ToString(dr["Address"]),
                        ContactNo = Convert.ToString(dr["ContactNo"]),
                        DateOfBirth = Convert.ToDateTime(dr["DateOfBirth"]),
                        Gender = Convert.ToString(dr["Gender"])
                    });
                }
                _sqlConnection.Close();
                return customers;
            }
        }

        public async Task<Customer> GetCustomerById(int customerId)
        {
            var query = "SELECT * FROM Customers WHERE CustomerId = @CustomerId";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    return null; // Return null if the customer is not found.
                }

                var customer = new Customer()
                {
                    CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"]),
                    CustomerName = Convert.ToString(dt.Rows[0]["CustomerName"]),
                    Email = Convert.ToString(dt.Rows[0]["Email"]),
                    Address = Convert.ToString(dt.Rows[0]["Address"]),
                    ContactNo = Convert.ToString(dt.Rows[0]["ContactNo"]),
                    DateOfBirth = Convert.ToDateTime(dt.Rows[0]["DateOfBirth"]),
                    Gender = Convert.ToString(dt.Rows[0]["Gender"])
                };

                _sqlConnection.Close();
                return customer;
            }
        }

        public async Task<bool> DeleteCustomer(int customerId)
        {
            var query = "DELETE FROM Customers WHERE CustomerId = @CustomerId";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> UpdateCustomerPartial(int customerId, JsonPatchDocument<Customer> patchedCustomer)
        {
            var existingCustomer = await GetCustomerById(customerId);
            if (existingCustomer == null)
            {
                return false; // Customer not found.
            }

            // Validate and apply the patch to the existing customer.
            if (!TryApplyPatch(patchedCustomer, existingCustomer, out var errors))
            {
                // Handle errors if necessary.
                return false; // Invalid patch data.
            }

            // Update the customer in the database.
            var query = "UPDATE Customers SET CustomerName = @CustomerName, Email = @Email, Address = @Address, ContactNo = @ContactNo, DateOfBirth = @DateOfBirth, Gender = @Gender WHERE CustomerId = @CustomerId";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@CustomerName", existingCustomer.CustomerName);
                cmd.Parameters.AddWithValue("@Email", existingCustomer.Email);
                cmd.Parameters.AddWithValue("@Address", existingCustomer.Address);
                cmd.Parameters.AddWithValue("@ContactNo", existingCustomer.ContactNo);
                cmd.Parameters.AddWithValue("@DateOfBirth", existingCustomer.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", existingCustomer.Gender);
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

        private bool TryApplyPatch(JsonPatchDocument<Customer> patch, Customer targetCustomer, out List<string> errors)
        {
            errors = new List<string>();
            try
            {
                patch.ApplyTo(targetCustomer);
                return true;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateCustomer(int customerId, Customer updatedCustomer)
        {
            var query = "UPDATE Customers SET CustomerName = @CustomerName, Email = @Email, Address = @Address, ContactNo = @ContactNo, DateOfBirth = @DateOfBirth, Gender = @Gender WHERE CustomerId = @CustomerId";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@CustomerName", updatedCustomer.CustomerName);
                cmd.Parameters.AddWithValue("@Email", updatedCustomer.Email);
                cmd.Parameters.AddWithValue("@Address", updatedCustomer.Address);
                cmd.Parameters.AddWithValue("@ContactNo", updatedCustomer.ContactNo);
                cmd.Parameters.AddWithValue("@DateOfBirth", updatedCustomer.DateOfBirth);
                cmd.Parameters.AddWithValue("@Gender", updatedCustomer.Gender);
                cmd.Parameters.AddWithValue("@CustomerId", customerId);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }
   
    }
}
