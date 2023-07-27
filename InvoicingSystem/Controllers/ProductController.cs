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

        public ProductController(ProductServices productService)
        {
            _productService = productService;
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> AddProduct([FromBody] Product product)
        {
            bool isAdded = await _productService.AddProduct(product);
            if (isAdded)
            {
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
                return Ok("Product Updated!");
            }
            else
            {
                return NotFound("Product not found.");
            }
        }

    }

}
