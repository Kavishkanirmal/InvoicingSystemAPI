using InvoicingSystem.Models;
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

        [HttpPost("ProductInsert")]
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

        [HttpGet("ProductGet")]
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


        [HttpDelete]
        public IActionResult DeleteProduct()
        {
            return Ok("Product deleted!");
        }


        [HttpPatch]
        public IActionResult ActivateProduct()
        {
            return Ok("Product deleted!");
        }

        [HttpPut]
        public IActionResult UpdateProduct()
        {
            return Ok("Product updated!");
        }


    }

}
