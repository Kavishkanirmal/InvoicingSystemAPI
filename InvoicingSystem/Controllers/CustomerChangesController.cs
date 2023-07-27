using InvoicingSystem.Models;
using InvoicingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoicingSystem.Controllers
{
    public class CustomerChangesController : Controller
    {

        private readonly CustomerChangesServices _customerChangesServices;

        public CustomerChangesController(IConfiguration configuration)
        {
            _customerChangesServices = new CustomerChangesServices(configuration);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> AddTrackCustomerChanges([FromBody] TrackCustomerChanges trackCustomerChanges)
        {
            if (trackCustomerChanges == null)
            {
                return BadRequest("Invalid data provided for adding track Customer changes.");
            }

            bool result = await _customerChangesServices.AddTrackCustomerChanges(trackCustomerChanges);

            if (result)
            {
                return Ok("Track Customer changes added successfully!");
            }
            else
            {
                return BadRequest("Failed to add track Customer changes.");
            }
        }

    }
}
