using InvoicingSystem.Models;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;

namespace InvoicingSystem.Services
{
    public class InvoiceServices
    {

        private readonly SqlConnection _sqlConnection;

        public InvoiceServices(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }

        public async Task<IActionResult> AddInvoice(Invoice invoice)
        {
            _sqlConnection.Open();
            var productQuery = "SELECT Quantity FROM Products WHERE ProductName = @ProductName";   
            var productCmd = new SqlCommand(productQuery, _sqlConnection);
            productCmd.Parameters.AddWithValue("@ProductName", invoice.ProductName);
            var availableQuantity = Convert.ToInt32(productCmd.ExecuteScalar());

            if (availableQuantity < invoice.UnitsPerProduct)
            {
                return new BadRequestObjectResult("Insufficient quantity available for the selected product.");
            }
            else
            {
                // Deduct the purchased units from the available quantity
                var updatedQuantity = availableQuantity - invoice.UnitsPerProduct;

                // Update the available quantity in the Products table
                var updateProductQuery = "UPDATE Products SET Quantity = @UpdatedQuantity WHERE ProductName = @ProductName";
                var updateProductCmd = new SqlCommand(updateProductQuery, _sqlConnection);
                updateProductCmd.Parameters.AddWithValue("@UpdatedQuantity", updatedQuantity);
                updateProductCmd.Parameters.AddWithValue("@ProductName", invoice.ProductName);
                await updateProductCmd.ExecuteNonQueryAsync();

                // Insert the invoice into the Invoices table
                var query = "INSERT INTO Invoices (InvoiceNumber, InvoiceDate, CustomerName, ProductName, UnitsPerProduct, UnitPricePerProduct, TotalPricePerProduct, DiscountPerProduct, CustomerId) VALUES (@InvoiceNumber, @InvoiceDate, @CustomerName, @ProductName, @UnitsPerProduct, @UnitPricePerProduct, @TotalPricePerProduct, @DiscountPerProduct, @CustomerId)";
                var cmd = new SqlCommand(query, _sqlConnection);

                cmd.Parameters.AddWithValue("InvoiceNumber", invoice.InvoiceNumber);
                cmd.Parameters.AddWithValue("InvoiceDate", invoice.InvoiceDate);
                cmd.Parameters.AddWithValue("CustomerName", invoice.CustomerName);
                cmd.Parameters.AddWithValue("ProductName", invoice.ProductName);
                cmd.Parameters.AddWithValue("UnitsPerProduct", invoice.UnitsPerProduct);
                cmd.Parameters.AddWithValue("UnitPricePerProduct", invoice.UnitPricePerProduct);
                cmd.Parameters.AddWithValue("TotalPricePerProduct", invoice.TotalPricePerProduct);
                cmd.Parameters.AddWithValue("DiscountPerProduct", invoice.DiscountPerProduct);
                cmd.Parameters.AddWithValue("CustomerId", invoice.CustomerId);

                var result = await cmd.ExecuteNonQueryAsync();
                _sqlConnection.Close();
                return new OkObjectResult(result);
            }
        }

        public Invoice GetInvoiceById(int invoiceNumber)
        {


                _sqlConnection.Open();
                var query = "SELECT * FROM Invoices WHERE InvoiceNumber = @InvoiceNumber";
                var cmd = new SqlCommand(query, _sqlConnection);
                cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    return null; // Return null if the invoice is not found.
                }

                var invoice = new Invoice()
                {
                    InvoiceNumber = Convert.ToInt32(dt.Rows[0]["InvoiceNumber"]),
                    InvoiceDate = Convert.ToDateTime(dt.Rows[0]["InvoiceDate"]),
                    CustomerName = Convert.ToString(dt.Rows[0]["CustomerName"]),
                    ProductName = Convert.ToString(dt.Rows[0]["ProductName"]),
                    UnitsPerProduct = Convert.ToInt32(dt.Rows[0]["UnitsPerProduct"]),
                    UnitPricePerProduct = Convert.ToSingle(dt.Rows[0]["UnitPricePerProduct"]),
                    TotalPricePerProduct = Convert.ToSingle(dt.Rows[0]["TotalPricePerProduct"]),
                    DiscountPerProduct = Convert.ToSingle(dt.Rows[0]["DiscountPerProduct"]),
                    CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"])
                };

            _sqlConnection.Close();
            return invoice;
            

        }

        public List<Invoice> GetInvoices()
        {

            _sqlConnection.Open();
            var query = "SELECT * FROM Invoices";
            var cmd = new SqlCommand(query, _sqlConnection);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            var invoices = new List<Invoice>();
            foreach (DataRow dr in dt.Rows)
            {
                invoices.Add(new Invoice()
                {
                    InvoiceNumber = Convert.ToInt32(dr["InvoiceNumber"]),
                    InvoiceDate = Convert.ToDateTime(dr["InvoiceDate"]),
                    CustomerName = Convert.ToString(dr["CustomerName"]),
                    ProductName = Convert.ToString(dr["ProductName"]),
                    UnitsPerProduct = Convert.ToInt32(dr["UnitsPerProduct"]),
                    UnitPricePerProduct = Convert.ToSingle(dr["UnitPricePerProduct"]),
                    TotalPricePerProduct = Convert.ToSingle(dr["TotalPricePerProduct"]),
                    DiscountPerProduct = Convert.ToSingle(dr["DiscountPerProduct"]),
                    CustomerId = Convert.ToInt32(dr["CustomerId"])
                });
            }
            _sqlConnection.Close();
            return invoices;
        }

        public List<Invoice> GetInvoicesByDateAndCustomer(DateTime startDate, DateTime endDate, int customerId)
        {
            _sqlConnection.Open();
            var query = "SELECT * FROM Invoices WHERE InvoiceDate >= @StartDate AND InvoiceDate <= @EndDate AND CustomerId = @CustomerId";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@StartDate", startDate);
            cmd.Parameters.AddWithValue("@EndDate", endDate);
            cmd.Parameters.AddWithValue("@CustomerId", customerId);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            var invoices = new List<Invoice>();
            foreach (DataRow dr in dt.Rows)
            {
                invoices.Add(new Invoice()
                {
                    InvoiceNumber = Convert.ToInt32(dr["InvoiceNumber"]),
                    InvoiceDate = Convert.ToDateTime(dr["InvoiceDate"]),
                    CustomerName = Convert.ToString(dr["CustomerName"]),
                    ProductName = Convert.ToString(dr["ProductName"]),
                    UnitsPerProduct = Convert.ToInt32(dr["UnitsPerProduct"]),
                    UnitPricePerProduct = Convert.ToSingle(dr["UnitPricePerProduct"]),
                    TotalPricePerProduct = Convert.ToSingle(dr["TotalPricePerProduct"]),
                    DiscountPerProduct = Convert.ToSingle(dr["DiscountPerProduct"]),
                    CustomerId = Convert.ToInt32(dr["CustomerId"])
                });
            }
            _sqlConnection.Close();
            return invoices;
        }

        public bool DeleteInvoice(int invoiceNumber)
        {
            _sqlConnection.Open();
            var query = "DELETE FROM Invoices WHERE InvoiceNumber = @InvoiceNumber";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
            int rowsAffected = cmd.ExecuteNonQuery();
            _sqlConnection.Close();

            _sqlConnection.Close();
            return rowsAffected > 0;
        }

        public bool UpdateInvoice(int invoiceNumber, Invoice updatedInvoice)
        {
            if (updatedInvoice == null)
            {
                return false;
            }
     
                _sqlConnection.Open();
                var query = "UPDATE Invoices SET InvoiceDate = @InvoiceDate, CustomerName = @CustomerName, ProductName = @ProductName, UnitsPerProduct = @UnitsPerProduct, UnitPricePerProduct = @UnitPricePerProduct, TotalPricePerProduct = @TotalPricePerProduct, DiscountPerProduct = @DiscountPerProduct, CustomerId = @CustomerId WHERE InvoiceNumber = @InvoiceNumber";
                using (var cmd = new SqlCommand(query, _sqlConnection))
                {
                    cmd.Parameters.AddWithValue("@InvoiceDate", updatedInvoice.InvoiceDate);
                    cmd.Parameters.AddWithValue("@CustomerName", updatedInvoice.CustomerName);
                    cmd.Parameters.AddWithValue("@ProductName", updatedInvoice.ProductName);
                    cmd.Parameters.AddWithValue("@UnitsPerProduct", updatedInvoice.UnitsPerProduct);
                    cmd.Parameters.AddWithValue("@UnitPricePerProduct", updatedInvoice.UnitPricePerProduct);
                    cmd.Parameters.AddWithValue("@TotalPricePerProduct", updatedInvoice.TotalPricePerProduct);
                    cmd.Parameters.AddWithValue("@DiscountPerProduct", updatedInvoice.DiscountPerProduct);
                    cmd.Parameters.AddWithValue("@CustomerId", updatedInvoice.CustomerId);
                    cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
                    int rowsAffected = cmd.ExecuteNonQuery();

                _sqlConnection.Close();
                return rowsAffected > 0;
                }
            
        }

        public async Task<bool> UpdateInvoicePartial(int invoiceNumber, JsonPatchDocument<Invoice> patchedInvoice)
        {
            var existingInvoice = await GetInvoiceByNumber(invoiceNumber);
            if (existingInvoice == null)
            {
                return false; // Invoice not found.
            }

            // Validate and apply the patch to the existing invoice.
            if (!TryApplyPatch(patchedInvoice, existingInvoice, out var errors))
            {
                // Handle errors if necessary.
                return false; // Invalid patch data.
            }

            // Update the invoice in the database.
            var query = "UPDATE Invoices SET InvoiceDate = @InvoiceDate, CustomerName = @CustomerName, ProductName = @ProductName, UnitsPerProduct = @UnitsPerProduct, UnitPricePerProduct = @UnitPricePerProduct, TotalPricePerProduct = @TotalPricePerProduct, DiscountPerProduct = @DiscountPerProduct, CustomerId = @CustomerId WHERE InvoiceNumber = @InvoiceNumber";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@InvoiceDate", existingInvoice.InvoiceDate);
                cmd.Parameters.AddWithValue("@CustomerName", existingInvoice.CustomerName);
                cmd.Parameters.AddWithValue("@ProductName", existingInvoice.ProductName);
                cmd.Parameters.AddWithValue("@UnitsPerProduct", existingInvoice.UnitsPerProduct);
                cmd.Parameters.AddWithValue("@UnitPricePerProduct", existingInvoice.UnitPricePerProduct);
                cmd.Parameters.AddWithValue("@TotalPricePerProduct", existingInvoice.TotalPricePerProduct);
                cmd.Parameters.AddWithValue("@DiscountPerProduct", existingInvoice.DiscountPerProduct);
                cmd.Parameters.AddWithValue("@CustomerId", existingInvoice.CustomerId);
                cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

        private async Task<Invoice> GetInvoiceByNumber(int invoiceNumber)
        {
            var query = "SELECT * FROM Invoices WHERE InvoiceNumber = @invoiceNumber";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@invoiceNumber", invoiceNumber);
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    return null; // Return null if the invoice is not found.
                }

                var invoice = new Invoice()
                {
                    InvoiceNumber = Convert.ToInt32(dt.Rows[0]["InvoiceNumber"]),
                    InvoiceDate = Convert.ToDateTime(dt.Rows[0]["InvoiceDate"]),
                    CustomerName = Convert.ToString(dt.Rows[0]["CustomerName"]),
                    ProductName = Convert.ToString(dt.Rows[0]["ProductName"]),
                    UnitsPerProduct = Convert.ToInt32(dt.Rows[0]["UnitsPerProduct"]),
                    UnitPricePerProduct = Convert.ToSingle(dt.Rows[0]["UnitPricePerProduct"]),
                    TotalPricePerProduct = Convert.ToSingle(dt.Rows[0]["TotalPricePerProduct"]),
                    DiscountPerProduct = Convert.ToSingle(dt.Rows[0]["DiscountPerProduct"]),
                    CustomerId = Convert.ToInt32(dt.Rows[0]["CustomerId"])
                };

                _sqlConnection.Close();
                return invoice;
            }
        }

        private bool TryApplyPatch(JsonPatchDocument<Invoice> patch, Invoice targetInvoice, out string[] errors)
        {
            errors = null;
            try
            {
                patch.ApplyTo(targetInvoice);
                return true;
            }
            catch (Exception ex)
            {
                errors = new string[] { ex.Message };
                return false;
            }
        }

    }
}
