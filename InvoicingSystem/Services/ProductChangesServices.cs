using InvoicingSystem.Models;
using System.Data.SqlClient;

namespace InvoicingSystem.Services
{
    public class ProductChangesServices
    {

        private readonly SqlConnection _sqlConnection;

        public ProductChangesServices(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }

        public async Task<bool> AddTrackProductChanges(TrackProductChanges trackProductChanges)
        {
            var query = "INSERT INTO TrackProductChanges (Date, Operation, ProductId) VALUES (@Date, @Operation, @ProductId)";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@Date", trackProductChanges.Date);
                cmd.Parameters.AddWithValue("@Operation", trackProductChanges.Operation);
                cmd.Parameters.AddWithValue("@ProductId", trackProductChanges.ProductId);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

    }
}
