using InvoicingSystem.Models;
using System.Data.SqlClient;

namespace InvoicingSystem.Services
{
    public class InvoiceChangesServices
    {

        private readonly SqlConnection _sqlConnection;

        public InvoiceChangesServices(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }

        public async Task<bool> AddTrackInvoiceChanges(TrackInvoiceChanges trackInvoiceChanges)
        {
            var query = "INSERT INTO TrackInvoiceChanges (Date, Operation, InvoiceNumber) VALUES (@Date, @Operation, @InvoiceNumber)";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@Date", trackInvoiceChanges.Date);
                cmd.Parameters.AddWithValue("@Operation", trackInvoiceChanges.Operation);
                cmd.Parameters.AddWithValue("@InvoiceNumber", trackInvoiceChanges.InvoiceNumber);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

    }
}
