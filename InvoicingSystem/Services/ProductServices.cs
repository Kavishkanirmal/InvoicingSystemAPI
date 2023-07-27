// ProductServices.cs

using InvoicingSystem.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace InvoicingSystem.Services
{
    public class ProductServices
    {
        private readonly SqlConnection _sqlConnection;

        public ProductServices(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("InvoicingSystem2");
            _sqlConnection = new SqlConnection(connectionString);
        }

        public async Task<bool> AddProduct(Product product)
        {
            var query = "INSERT INTO Products (productName, productDescription, purchasePrice, sellingPrice, quantity) VALUES (@productName, @productDescription, @purchasePrice, @sellingPrice, @quantity)";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@productName", product.ProductName);
                cmd.Parameters.AddWithValue("@productDescription", product.ProductDescription);
                cmd.Parameters.AddWithValue("@purchasePrice", product.PurchasePrice);
                cmd.Parameters.AddWithValue("@sellingPrice", product.SellingPrice);
                cmd.Parameters.AddWithValue("@quantity", product.Quantity);

                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

        public async Task<List<Product>> GetProducts()
        {
            var query = "SELECT * FROM Products";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
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
                _sqlConnection.Close();
                return products;
            }
        }

        public async Task<Product> GetProductById(int productId)
        {
            var query = "SELECT * FROM Products WHERE ProductId = @productId";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@productId", productId);
                var adapter = new SqlDataAdapter(cmd);
                var dt = new DataTable();
                adapter.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    return null; // Return null if the product is not found.
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

                _sqlConnection.Close();
                return product;
            }
        }

        public async Task<bool> DeleteProduct(int productId)
        {
            var query = "DELETE FROM Products WHERE ProductId = @ProductId";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@ProductId", productId);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();
                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

        public async Task<bool> UpdateProductPartial(int productId, JsonPatchDocument<Product> patchedProduct)
        {
            var existingProduct = await GetProductById(productId);
            if (existingProduct == null)
            {
                return false; // Product not found.
            }

            // Validate and apply the patch to the existing product.
            if (!TryApplyPatch(patchedProduct, existingProduct, out var errors))
            {
                // Handle errors if necessary.
                return false; // Invalid patch data.
            }

            // Update the product in the database.
            var query = "UPDATE Products SET ProductName = @ProductName, ProductDescription = @ProductDescription, PurchasePrice = @PurchasePrice, SellingPrice = @SellingPrice, Quantity = @Quantity WHERE ProductId = @ProductId";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@ProductName", existingProduct.ProductName);
                cmd.Parameters.AddWithValue("@ProductDescription", existingProduct.ProductDescription);
                cmd.Parameters.AddWithValue("@PurchasePrice", existingProduct.PurchasePrice);
                cmd.Parameters.AddWithValue("@SellingPrice", existingProduct.SellingPrice);
                cmd.Parameters.AddWithValue("@Quantity", existingProduct.Quantity);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

        private bool TryApplyPatch(JsonPatchDocument<Product> patch, Product targetProduct, out List<string> errors)
        {
            errors = new List<string>();
            try
            {
                patch.ApplyTo(targetProduct);
                return true;
            }
            catch (Exception ex)
            {
                errors.Add(ex.Message);
                return false;
            }
        }

        public async Task<bool> UpdateProduct(int productId, Product updatedProduct)
        {
            var existingProduct = await GetProductById(productId);
            if (existingProduct == null)
            {
                return false; // Product not found.
            }

            // Update the product properties from the updatedProduct object.
            existingProduct.ProductName = updatedProduct.ProductName;
            existingProduct.ProductDescription = updatedProduct.ProductDescription;
            existingProduct.PurchasePrice = updatedProduct.PurchasePrice;
            existingProduct.SellingPrice = updatedProduct.SellingPrice;
            existingProduct.Quantity = updatedProduct.Quantity;

            // Update the product in the database.
            var query = "UPDATE Products SET ProductName = @ProductName, ProductDescription = @ProductDescription, PurchasePrice = @PurchasePrice, SellingPrice = @SellingPrice, Quantity = @Quantity WHERE ProductId = @ProductId";
            await _sqlConnection.OpenAsync();
            using (var cmd = new SqlCommand(query, _sqlConnection))
            {
                cmd.Parameters.AddWithValue("@ProductName", existingProduct.ProductName);
                cmd.Parameters.AddWithValue("@ProductDescription", existingProduct.ProductDescription);
                cmd.Parameters.AddWithValue("@PurchasePrice", existingProduct.PurchasePrice);
                cmd.Parameters.AddWithValue("@SellingPrice", existingProduct.SellingPrice);
                cmd.Parameters.AddWithValue("@Quantity", existingProduct.Quantity);
                cmd.Parameters.AddWithValue("@ProductId", productId);
                int rowsAffected = await cmd.ExecuteNonQueryAsync();

                _sqlConnection.Close();
                return rowsAffected > 0;
            }
        }

    }
}
