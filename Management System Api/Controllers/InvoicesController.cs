using Management_System_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Management_System_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceService _svc;
        public InvoicesController(IInvoiceService svc) => _svc = svc;


        [HttpGet("{invoiceId:int}/pdf")]
        [Authorize(Roles = "Admin,Salesperson")]
        public async Task<IActionResult> GetPdf(int invoiceId)
        {
            var bytes = await _svc.GetPdfAsync(invoiceId);
            return File(bytes, "application/pdf", $"invoice-{invoiceId}.pdf");
        }
    }
}
