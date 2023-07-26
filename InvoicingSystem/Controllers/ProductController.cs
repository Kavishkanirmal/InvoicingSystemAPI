using InvoicingSystem.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace InvoicingSystem.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : Controller
    {

        private readonly SqlConnection _sqlConnection;

        public ProductController(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            var query = "INSERT INTO Products (productName, productDescription, purchasePrice, sellingPrice, quantity) VALUES (@productName, @productDescription, @purchasePrice, @sellingPrice, @quantity)";
            _sqlConnection.Open();
            var cmd = new SqlCommand(query, _sqlConnection);
          
            cmd.Parameters.AddWithValue("ProductName", product.ProductName);
            cmd.Parameters.AddWithValue("ProductDescription", product.ProductDescription);
            cmd.Parameters.AddWithValue("PurchasePrice", product.PurchasePrice);
            cmd.Parameters.AddWithValue("SellingPrice", product.SellingPrice);
            cmd.Parameters.AddWithValue("Quantity", product.Quantity);

            var result = await cmd.ExecuteNonQueryAsync();
            return Ok(result);

        }

        [HttpGet("Get")]
        public IActionResult GetProducts()
        {
            var query = "SELECT * FROM Products";
            _sqlConnection.Open();
            var cmd = new SqlCommand(query, _sqlConnection);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);
            var products = new List<Product>();
            foreach (DataRow dr in dt.Rows)
            {
                products.Add(new Product()
                {
                    ProductId = Convert.ToInt32(dr["productID"]),
                    ProductName = Convert.ToString(dr["productName"]),
                    ProductDescription = Convert.ToString(dr["productDescription"]),
                    PurchasePrice = (float)Convert.ToDouble(dr["purchasePrice"]),
                    SellingPrice = (float)Convert.ToDouble(dr["sellingPrice"]),
                    Quantity = Convert.ToInt32(dr["quantity"])
                });
            }
            return Ok(products);
        }


        [HttpGet("GetById/{productId}")]
        public IActionResult GetCustomerById(int productId)
        {
            _sqlConnection.Open();
            var query = "SELECT * FROM Products WHERE ProductId = @productId";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return NotFound("Product not found.");
            }

            var product = new Product()
            {
                ProductId = Convert.ToInt32(dt.Rows[0]["ProductId"]),
                ProductName = Convert.ToString(dt.Rows[0]["ProductName"]),
                ProductDescription = Convert.ToString(dt.Rows[0]["ProductDescription"]),
                PurchasePrice = Convert.ToSingle(dt.Rows[0]["PurchasePrice"]),
                SellingPrice = Convert.ToSingle(dt.Rows[0]["SellingPrice"]),
                Quantity = Convert.ToInt32(dt.Rows[0]["Quantity"])
            };

            return Ok(product);
        }


        [HttpDelete("Delete/{productId}")]
        public IActionResult DeleteProduct(int productId)
        {
            _sqlConnection.Open();
            var query = "DELETE FROM Products WHERE ProductId = @ProductId";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok("Product Deleted!");
            }
            else
            {
                return NotFound("Product not found.");
            }
        }


        [HttpPatch("UpdatePartial/{productId}")]
        public IActionResult UpdateProductPartial(int productId, [FromBody] JsonPatchDocument<Product> patchedProduct)
        {
            if (patchedProduct == null || productId <= 0)
                return BadRequest();


            _sqlConnection.Open();
            var query = "SELECT * FROM Products WHERE ProductId = @ProductId";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            var adapter = new SqlDataAdapter(cmd);
            var dt = new DataTable();
            adapter.Fill(dt);

            if (dt.Rows.Count == 0)
            {
                return NotFound("Product not found.");
            }

            var existingProduct = new Product()
            {
                ProductId = Convert.ToInt32(dt.Rows[0]["ProductId"]),
                ProductName = Convert.ToString(dt.Rows[0]["ProductName"]),
                ProductDescription = Convert.ToString(dt.Rows[0]["ProductDescription"]),
                PurchasePrice = Convert.ToSingle(dt.Rows[0]["PurchasePrice"]),
                SellingPrice = Convert.ToSingle(dt.Rows[0]["SellingPrice"]),
                Quantity = Convert.ToInt32(dt.Rows[0]["Quantity"])
            };

            patchedProduct.ApplyTo(existingProduct, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            query = "UPDATE Products SET ProductName = @ProductName, ProductDescription = @ProductDescription, PurchasePrice = @PurchasePrice, SellingPrice = @SellingPrice, Quantity = @Quantity WHERE ProductId = @ProductId";
            cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@ProductName", existingProduct.ProductName);
            cmd.Parameters.AddWithValue("@ProductDescription", existingProduct.ProductDescription);
            cmd.Parameters.AddWithValue("@PurchasePrice", existingProduct.PurchasePrice);
            cmd.Parameters.AddWithValue("@SellingPrice", existingProduct.SellingPrice);
            cmd.Parameters.AddWithValue("@Quantity", existingProduct.Quantity);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok("Product Updated!");
            }
            else
            {
                return NotFound("Product not found.");
            }

        }


        [HttpPut("Update/{productId}")]
        public IActionResult UpdateProduct(int productId, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                return BadRequest("Invalid data provided for updating the product.");
            }

            _sqlConnection.Open();
            var query = "UPDATE Products SET ProductName = @ProductName, ProductDescription = @ProductDescription, PurchasePrice = @PurchasePrice, SellingPrice = @SellingPrice, Quantity = @Quantity WHERE ProductId = @ProductId";
            var cmd = new SqlCommand(query, _sqlConnection);
            cmd.Parameters.AddWithValue("@ProductName", updatedProduct.ProductName);
            cmd.Parameters.AddWithValue("@ProductDescription", updatedProduct.ProductDescription);
            cmd.Parameters.AddWithValue("@PurchasePrice", updatedProduct.PurchasePrice);
            cmd.Parameters.AddWithValue("@SellingPrice", updatedProduct.SellingPrice);
            cmd.Parameters.AddWithValue("@Quantity", updatedProduct.Quantity);
            cmd.Parameters.AddWithValue("@ProductId", productId);
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return Ok("Product Updated!");
            }
            else
            {
                return NotFound("Product not found.");
            }
        }


    }

}
