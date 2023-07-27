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

        public InvoiceController(InvoiceServices invoiceService)
        {
            _invoiceService = invoiceService;
        }


        [HttpPost("Insert")]
        public async Task<IActionResult> AddInvoices([FromBody] Invoice invoice)
        {
            return await _invoiceService.AddInvoice(invoice);
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
                return Ok("Invoice Updated!");
            }
            else
            {
                return NotFound("Invoice not found.");
            }
        }

    }
}
