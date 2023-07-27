using InvoicingSystem.Models;
using InvoicingSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace InvoicingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductChangesController : Controller
    {

        private readonly ProductChangesServices _productChangesServices;

        public ProductChangesController(IConfiguration configuration)
        {
            _productChangesServices = new ProductChangesServices(configuration);
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> AddTrackProductChanges([FromBody] TrackProductChanges trackProductChanges)
        {
            if (trackProductChanges == null)
            {
                return BadRequest("Invalid data provided for adding track product changes.");
            }

            bool result = await _productChangesServices.AddTrackProductChanges(trackProductChanges);

            if (result)
            {
                return Ok("Track product changes added successfully!");
            }
            else
            {
                return BadRequest("Failed to add track product changes.");
            }
        }

    }
}
