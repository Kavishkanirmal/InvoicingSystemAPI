using InvoicingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.JsonPatch;
using InvoicingSystem.Services;

namespace InvoicingSystem.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : Controller
    {

        private readonly CustomerServices _customerService;

        public CustomerController(CustomerServices customerService)
        {
            _customerService = customerService;
        }


        [HttpPost("Insert")]
        public async Task<IActionResult> AddCustomer([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Invalid data provided for adding the customer.");
            }

            var isAdded = await _customerService.AddCustomer(customer);
            if (isAdded)
            {
                return Ok("Customer Added!");
            }
            else
            {
                return BadRequest("Failed to add customer.");
            }
        }

        [HttpGet("Get")]
        public async Task<IActionResult> GetCustomers()
        {
            var customers = await _customerService.GetCustomers();
            return Ok(customers);
        }

        [HttpGet("GetById/{customerId}")]
        public async Task<IActionResult> GetCustomerById(int customerId)
        {
            var customer = await _customerService.GetCustomerById(customerId);
            if (customer == null)
            {
                return NotFound("Customer not found.");
            }

            return Ok(customer);
        }

        [HttpDelete("Delete/{customerId}")]
        public async Task<IActionResult> DeleteCustomer(int customerId)
        {
            var isDeleted = await _customerService.DeleteCustomer(customerId);
            if (isDeleted)
            {
                return Ok("Customer Deleted!");
            }
            else
            {
                return NotFound("Customer not found.");
            }
        }

        [HttpPatch("UpdatePartial/{customerId}")]
        public async Task<IActionResult> UpdateCustomerPartial(int customerId, [FromBody] JsonPatchDocument<Customer> patchedCustomer)
        {
            if (patchedCustomer == null || customerId <= 0)
            {
                return BadRequest();
            }

            var isUpdated = await _customerService.UpdateCustomerPartial(customerId, patchedCustomer);
            if (isUpdated)
            {
                return Ok("Customer Updated!");
            }
            else
            {
                return NotFound("Customer not found or invalid patch data.");
            }
        }

        [HttpPut("Update/{customerId}")]
        public async Task<IActionResult> UpdateCustomer(int customerId, [FromBody] Customer updatedCustomer)
        {
            if (updatedCustomer == null)
            {
                return BadRequest("Invalid data provided for updating the customer.");
            }

            var isUpdated = await _customerService.UpdateCustomer(customerId, updatedCustomer);
            if (isUpdated)
            {
                return Ok("Customer Updated!");
            }
            else
            {
                return NotFound("Customer not found.");
            }
        }
    }
}
