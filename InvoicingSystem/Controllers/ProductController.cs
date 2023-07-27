using InvoicingSystem.Models;
using InvoicingSystem.Services;
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

        private readonly ProductServices _productService;
        private readonly ProductChangesServices _productChangesServices;

        public ProductController(ProductServices productService, ProductChangesServices productChangesServices)
        {
            _productService = productService;
            _productChangesServices = productChangesServices;
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            bool isAdded = await _productService.AddProduct(product);
            if (isAdded)
            {
                // Here's the updated part to add track changes:
                // Create a new TrackProductChanges object to record the addition of the product.
                var trackProductChanges = new TrackProductChanges
                {
                    Date = DateTime.Now,
                    Operation = "Added", // Set the operation to "Added" for the addition.
                    ProductId = product.ProductId // Assuming you have the ProductId property in the Product model.
                };

                // Call the AddTrackProductChanges method to insert the track record for the addition.
                bool trackChangesResult = await _productChangesServices.AddTrackProductChanges(trackProductChanges);

                return Ok("Product added successfully!");
            }
            else
            {
                return BadRequest("Failed to add the product.");
            }
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _productService.GetProducts();
            return Ok(products);
        }

        [HttpGet("GetById/{productId}")]
        public async Task<IActionResult> GetProductById(int productId)
        {
            var product = await _productService.GetProductById(productId);
            if (product == null)
            {
                return NotFound("Product not found.");
            }

            return Ok(product);
        }

        [HttpDelete("Delete/{productId}")]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            bool isDeleted = await _productService.DeleteProduct(productId);
            if (isDeleted)
            {
                return Ok("Product Deleted!");
            }
            else
            {
                return NotFound("Product not found.");
            }
        }

        [HttpPatch("UpdatePartial/{productId}")]
        public async Task<IActionResult> UpdateProductPartial(int productId, [FromBody] JsonPatchDocument<Product> patchedProduct)
        {
            if (patchedProduct == null || productId <= 0)
            {
                return BadRequest();
            }

            var isUpdated = await _productService.UpdateProductPartial(productId, patchedProduct);
            if (isUpdated)
            {
                // Here's the updated part to add track changes:
                // Create a new TrackProductChanges object to record the update of the product.
                var trackProductChanges = new TrackProductChanges
                {
                    Date = DateTime.Now,
                    Operation = "Patched", // Set the operation to "Updated" for the update.
                    ProductId = productId // Use the productId parameter from the method.
                };

                // Call the AddTrackProductChanges method to insert the track record for the update.
                bool trackChangesResult = await _productChangesServices.AddTrackProductChanges(trackProductChanges);

                return Ok("Product Updated!");
            }
            else
            {
                return NotFound("Product not found or invalid patch data.");
            }
        }

        [HttpPut("Update/{productId}")]
        public async Task<IActionResult> UpdateProduct(int productId, [FromBody] Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                return BadRequest("Invalid data provided for updating the product.");
            }

            var isUpdated = await _productService.UpdateProduct(productId, updatedProduct);
            if (isUpdated)
            {
                // Here's the updated part to add track changes:
                // Create a new TrackProductChanges object to record the update of the product.
                var trackProductChanges = new TrackProductChanges
                {
                    Date = DateTime.Now,
                    Operation = "Updated", // Set the operation to "Updated" for the update.
                    ProductId = productId // Use the productId parameter from the method.
                };

                // Call the AddTrackProductChanges method to insert the track record for the update.
                bool trackChangesResult = await _productChangesServices.AddTrackProductChanges(trackProductChanges);

                return Ok("Product Updated!");
            }
            else
            {
                return NotFound("Product not found.");
            }
        }
    }

}
