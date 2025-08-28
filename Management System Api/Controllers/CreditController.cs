using Management_System_Api.Models.DTO;
using Management_System_Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Management_System_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CreditController : ControllerBase
    {
        private readonly CreditService _creditService;

        public CreditController(CreditService creditService)
        {
            _creditService = creditService;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateCreditSale([FromBody] CreateCreditSaleDto dto)
        {
            var sale = await _creditService.CreateCreditSaleAsync(dto);
            return Ok(new { success = true, data = sale });
        }

        [HttpPost("payment")]
        public async Task<IActionResult> RecordPayment([FromBody] RecordPaymentDto dto)
        {
            var sale = await _creditService.RecordPaymentAsync(dto);
            if (sale == null)
                return NotFound(new { success = false, message = "Credit sale not found." });

            return Ok(new { success = true, data = sale });
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCredits()
        {
            var credits = await _creditService.GetAllCreditsAsync();
            return Ok(new { success = true, data = credits });
        }
    }
}
