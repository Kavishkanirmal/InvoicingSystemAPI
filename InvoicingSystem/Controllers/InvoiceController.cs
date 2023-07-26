using InvoicingSystem.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace InvoicingSystem.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : Controller
    {

        private readonly SqlConnection _sqlConnection;

        public InvoiceController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }


        [HttpPost("Insert")]
        public async Task<IActionResult> AddInvoices([FromBody] Invoice invoice)
        {
            var productQuery = "SELECT Quantity FROM Products WHERE ProductName = @ProductName";
            _sqlConnection.Open();
            var productCmd = new SqlCommand(productQuery, _sqlConnection);
            productCmd.Parameters.AddWithValue("@ProductName", invoice.ProductName);
            var availableQuantity = Convert.ToInt32(productCmd.ExecuteScalar());

            if (availableQuantity < invoice.UnitsPerProduct)
            {
                return BadRequest("Insufficient quantity available for the selected product.");
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
                return Ok(result);
            }
        }


        [HttpGet("Get")]
        public IActionResult GetInvoices()
        {
            var query = "SELECT * FROM Invoices";
            _sqlConnection.Open();
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
            return Ok(invoices);
        }


        [HttpGet("GetById/{invoiceNumber}")]
        public IActionResult GetInvoiceById(int invoiceNumber)
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
                return NotFound("Invoice not found.");
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

            return Ok(invoice);
        }


        [HttpGet("GetbyDateAndCustomer/{startDate},{endDate},{customerId}")]
        public IActionResult GetInvoicesy(DateTime startDate, DateTime endDate, int customerId)
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

            return Ok(invoices);
        }


        [HttpDelete("Delete/{invoiceNumber}")]
        public IActionResult DeleteInvoice(int invoiceNumber)
        {
            _sqlConnection.Open();
            var query = "DELETE FROM Invoices WHERE InvoiceNumber = @InvoiceNumber";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok("Invoice Deleted!");
            }
            else
            {
                return NotFound("Invoice not found.");
            }
        }


        [HttpPatch("UpdatePartial/{invoiceNumber}")]
        public IActionResult UpdateInvoicePartial(int invoiceNumber, [FromBody] JsonPatchDocument<Invoice> patchedInvoice)
        {
            if (patchedInvoice == null)
                return BadRequest();


            _sqlConnection.Open();
            var query = "SELECT * FROM Invoices WHERE InvoiceNumber = @InvoiceNumber";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return NotFound("Invoice not found.");
            }

            var existingInvoice = new Invoice()
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

            patchedInvoice.ApplyTo(existingInvoice, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            query = "UPDATE Invoices SET InvoiceDate = @InvoiceDate, CustomerName = @CustomerName, ProductName = @ProductName, UnitsPerProduct = @UnitsPerProduct, UnitPricePerProduct = @UnitPricePerProduct, TotalPricePerProduct = @TotalPricePerProduct, DiscountPerProduct = @DiscountPerProduct, CustomerId = @CustomerId WHERE InvoiceNumber = @InvoiceNumber";
            cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@InvoiceDate", existingInvoice.InvoiceDate);
            cmd.Parameters.AddWithValue("@CustomerName", existingInvoice.CustomerName);
            cmd.Parameters.AddWithValue("@ProductName", existingInvoice.ProductName);
            cmd.Parameters.AddWithValue("@UnitsPerProduct", existingInvoice.UnitsPerProduct);
            cmd.Parameters.AddWithValue("@UnitPricePerProduct", existingInvoice.UnitPricePerProduct);
            cmd.Parameters.AddWithValue("@TotalPricePerProduct", existingInvoice.TotalPricePerProduct);
            cmd.Parameters.AddWithValue("@DiscountPerProduct", existingInvoice.DiscountPerProduct);
            cmd.Parameters.AddWithValue("@CustomerId", existingInvoice.CustomerId);
            cmd.Parameters.AddWithValue("@InvoiceNumber", invoiceNumber);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok("Invoice Updated!");
            }
            else
            {
                return NotFound("Invoice not found.");
            }

        }


        [HttpPut("Update/{invoiceNumber}")]
        public IActionResult UpdateInvoice(int invoiceNumber, [FromBody] Invoice updatedInvoice)
        {
            if (updatedInvoice == null)
            {
                return BadRequest("Invalid data provided for updating the Invoice.");
            }

            _sqlConnection.Open();
            var query = "UPDATE Invoices SET InvoiceDate = @InvoiceDate, CustomerName = @CustomerName, ProductName = @ProductName, UnitsPerProduct = @UnitsPerProduct, UnitPricePerProduct = @UnitPricePerProduct, TotalPricePerProduct = @TotalPricePerProduct, DiscountPerProduct = @DiscountPerProduct, CustomerId = @CustomerId WHERE InvoiceNumber = @InvoiceNumber"; 
            var cmd = new SqlCommand(query, _sqlConnection);
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

            if (rowsAffected > 0)
            {
                return Ok("Invoice Updated!");
            }
            else
            {
                return NotFound("Invoice not found.");
            }
        }

    }
}
