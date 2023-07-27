using InvoicingSystem.Models;
using InvoicingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoicingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceChangesController : Controller
    {
        private readonly InvoiceChangesServices _invoiceChangesServices;

        public InvoiceChangesController(IConfiguration configuration)
        {
            _invoiceChangesServices = new InvoiceChangesServices(configuration);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> AddTrackInvoiceChanges([FromBody] TrackInvoiceChanges trackInvoiceChanges)
        {
            if (trackInvoiceChanges == null)
            {
                return BadRequest("Invalid data provided for adding track Invoice changes.");
            }

            bool result = await _invoiceChangesServices.AddTrackInvoiceChanges(trackInvoiceChanges);

            if (result)
            {
                return Ok("Track Invoice changes added successfully!");
            }
            else
            {
                return BadRequest("Failed to add track Invoice changes.");
            }
        }


    }
}
