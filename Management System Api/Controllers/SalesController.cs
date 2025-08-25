using System.Security.Claims;
using Management_System_Api.Models.DTOs;
using Management_System_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_System_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SalesController : ControllerBase
    {
        private readonly ISalesService _sales;
        private readonly IInvoiceService _invoices;

        public SalesController(ISalesService sales, IInvoiceService invoices)
        {
            _sales = sales;
            _invoices = invoices;
        }

        // Get all sales
        [HttpGet]
        [Authorize(Roles = "Admin,Salesperson")]
        public async Task<IActionResult> GetAll() => Ok(await _sales.GetAllAsync());

        // Get sale by ID
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin,Salesperson")]
        public async Task<IActionResult> Get(int id) => Ok(await _sales.GetByIdAsync(id));

        // Create new sale (captures current user)
        [HttpPost]
        [Authorize(Roles = "Admin,Salesperson")]
        public async Task<IActionResult> Create([FromBody] SaleCreateDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var sale = await _sales.CreateAsync(dto, userId);
            return CreatedAtAction(nameof(Get), new { id = sale.Id }, sale);
        }

        // Generate invoice for a sale
        [HttpPost("{saleId:int}/invoice")]
        [Authorize(Roles = "Admin,Salesperson")]
        public async Task<IActionResult> GenerateInvoice(
            int saleId,
            [FromQuery] string businessName,
            [FromQuery] string businessContact)
        {
            var invoice = await _invoices.GenerateForSaleAsync(saleId, businessName, businessContact);
            return Ok(invoice);
        }
    }
}
