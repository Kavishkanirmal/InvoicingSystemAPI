using InvoicingSystem.Models;
using InvoicingSystem.Services;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Microsoft.AspNetCore.Mvc.ModelBinding;


namespace InvoicingSystem.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : Controller
    {

        private readonly InvoiceServices _invoiceService;
        private readonly InvoiceChangesServices _invoiceChangesServices;

        public InvoiceController(InvoiceServices invoiceService, InvoiceChangesServices invoiceChangesServices)
        {
            _invoiceService = invoiceService;
            _invoiceChangesServices = invoiceChangesServices;
        }

        [HttpPost("Insert")]
        public async Task<IActionResult> AddInvoices([FromBody] Invoice invoice)
        {
            var result = await _invoiceService.AddInvoice(invoice);

            // Here's the updated part to add track changes:
            if (result is OkObjectResult okResult && okResult.Value is int rowsAffected && rowsAffected > 0)
            {
                // Create a new TrackInvoiceChanges object to record the addition of the invoice.
                var trackInvoiceChanges = new TrackInvoiceChanges
                {
                    Date = DateTime.Now,
                    Operation = "Added", // Set the operation to "Added" for the addition.
                    InvoiceNumber = invoice.InvoiceNumber // Use the invoice number from the added invoice.
                };

                // Call the AddTrackInvoiceChanges method to insert the track record for the addition.
                bool trackChangesResult = await _invoiceChangesServices.AddTrackInvoiceChanges(trackInvoiceChanges);
            }

            return result;
        }

        [HttpGet("Get")] 
        public IActionResult GetInvoices()
        {
            var invoices = _invoiceService.GetInvoices();
            return Ok(invoices);
        }

        [HttpGet("GetById/{invoiceNumber}")]
        public IActionResult GetInvoiceById(int invoiceNumber)
        {
            var invoice = _invoiceService.GetInvoiceById(invoiceNumber);

            if (invoice == null)
            {
                return NotFound("Invoice not found.");
            }

            return Ok(invoice);
        }

        [HttpGet("GetbyDateAndCustomer/{startDate},{endDate},{customerId}")]
        public IActionResult GetInvoicesByDateAndCustomer(DateTime startDate, DateTime endDate, int customerId)
        {
            var invoices = _invoiceService.GetInvoicesByDateAndCustomer(startDate, endDate, customerId);
            return Ok(invoices);
        }

        [HttpDelete("Delete/{invoiceNumber}")]
        public IActionResult DeleteInvoice(int invoiceNumber)
        {
            bool isDeleted = _invoiceService.DeleteInvoice(invoiceNumber);

            if (isDeleted)
            {
                return Ok("Invoice Deleted!");
            }
            else
            {
                return NotFound("Invoice not found.");
            }
        }

        [HttpPatch("UpdatePartial/{invoiceNumber}")]
        public async Task<IActionResult> UpdateInvoicePartial(int invoiceNumber, [FromBody] JsonPatchDocument<Invoice> patchedInvoice)
        {
            if (patchedInvoice == null)
            {
                return BadRequest();
            }

            var isUpdated = await _invoiceService.UpdateInvoicePartial(invoiceNumber, patchedInvoice);
            if (isUpdated)
            {
                // Here's the updated part to add track changes:
                var trackInvoiceChanges = new TrackInvoiceChanges
                {
                    Date = DateTime.Now,
                    Operation = "Patched", // Set the operation to "Updated" for the update.
                    InvoiceNumber = invoiceNumber // Use the invoice number being updated.
                };

                // Call the AddTrackInvoiceChanges method to insert the track record for the update.
                bool trackChangesResult = await _invoiceChangesServices.AddTrackInvoiceChanges(trackInvoiceChanges);

                return Ok("Invoice Updated!");
            }
            else
            {
                return NotFound("Invoice not found or invalid patch data.");
            }
        }     

        [HttpPut("Update/{invoiceNumber}")]
        public IActionResult UpdateInvoice(int invoiceNumber, [FromBody] Invoice updatedInvoice)
        {
            bool isUpdated = _invoiceService.UpdateInvoice(invoiceNumber, updatedInvoice);
            if (isUpdated)
            {
                // Here's the updated part to add track changes:
                var trackInvoiceChanges = new TrackInvoiceChanges
                {
                    Date = DateTime.Now,
                    Operation = "Updated", // Set the operation to "Updated" for the update.
                    InvoiceNumber = invoiceNumber // Use the invoice number being updated.
                };

                // Call the AddTrackInvoiceChanges method asynchronously to insert the track record for the update.
                _ = _invoiceChangesServices.AddTrackInvoiceChanges(trackInvoiceChanges);

                return Ok("Invoice Updated!");
            }
            else
            {
                return NotFound("Invoice not found.");
            }
        }

    }
}
