using InvoicingSystem.Models;
using System.Data.SqlClient;

namespace InvoicingSystem.Services
{
    public class CustomerChangesServices
    {

        private readonly SqlConnection _sqlConnection;

        public CustomerChangesServices(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }

        public async Task<bool> AddTrackCustomerChanges(TrackCustomerChanges trackCustomerChanges)
        {
            var query = "INSERT INTO TrackCustomerChanges (Date, Operation, CustomerId) VALUES (@Date, @Operation, @CustomerId)";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@Date", trackCustomerChanges.Date);
                cmd.Parameters.AddWithValue("@Operation", trackCustomerChanges.Operation);
                cmd.Parameters.AddWithValue("@CustomerId", trackCustomerChanges.CustomerId);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

    }
}
