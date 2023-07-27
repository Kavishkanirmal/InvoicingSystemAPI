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
        private readonly CustomerChangesServices _customerChangesServices;
       
        public CustomerController(CustomerServices customerService, CustomerChangesServices customerChangesServices)
        {
            _customerService = customerService;
            _customerChangesServices = customerChangesServices; // Initialize the CustomerChangesServices field.
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
                // Create a new TrackCustomerChanges object to record the addition of the customer.
                var trackCustomerChanges = new TrackCustomerChanges
                {
                    Date = DateTime.Now,
                    Operation = "Added", // Set the operation to "Added" for the addition.
                    CustomerId = customer.CustomerId 
                };

                // Call the AddTrackCustomerChanges method to insert the track record.
                bool trackChangesResult = await _customerChangesServices.AddTrackCustomerChanges(trackCustomerChanges);

                if (trackChangesResult)
                {
                    return Ok("Customer Added! Track Customer changes added successfully!");
                }
                else
                {
                    return BadRequest("Failed to add track Customer changes.");
                }
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
                // Here's the updated part to add track changes:
                // Create a new TrackCustomerChanges object to record the update of the customer.
                var trackCustomerChanges = new TrackCustomerChanges
                {
                    Date = DateTime.Now,
                    Operation = "Patched", // Set the operation to "Updated" for the update.
                    CustomerId = customerId
                };

                // Call the AddTrackCustomerChanges method to insert the track record for the update.
                bool trackChangesResult = await _customerChangesServices.AddTrackCustomerChanges(trackCustomerChanges);

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
                // Create a new TrackCustomerChanges object to record the update of the customer.
                var trackCustomerChanges = new TrackCustomerChanges
                {
                    Date = DateTime.Now,
                    Operation = "Updated", // Set the operation to "Updated" for the update.
                    CustomerId = customerId
                };

                // Call the AddTrackCustomerChanges method to insert the track record for the update.
                bool trackChangesResult = await _customerChangesServices.AddTrackCustomerChanges(trackCustomerChanges);

                return Ok("Customer Updated!");
            }
            else
            {
                return NotFound("Customer not found.");
            }
        }

    }
}
