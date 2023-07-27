using InvoicingSystem.Models;
using InvoicingSystem.Services;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace InvoicingSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {

        private readonly AdminServices _adminService;

        public AdminController(AdminServices adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("GetByUsername/{username}")]
        public async Task<IActionResult> GetAdmin(string username)
        {
            var admin = await _adminService.GetAdminByUsername(username);
            if (admin == null)
            {
                return NotFound("User not found.");
            }

            return Ok(admin);
        }

    }
}
