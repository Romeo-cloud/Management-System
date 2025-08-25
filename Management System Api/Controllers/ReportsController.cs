using Management_System_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Management_System_Api.Controllers
{ 
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Salesperson")]
public class ReportsController : ControllerBase
{
    private readonly IReportsService _svc;
    public ReportsController(IReportsService svc) => _svc = svc;


    [HttpGet("daily-sales")]
    public async Task<IActionResult> DailySales() => Ok(new { total = await _svc.GetTodayTotalAsync() });


    [HttpGet("monthly-sales")]
    public async Task<IActionResult> MonthlySales([FromQuery] int year, [FromQuery] int month)
    {
        var data = await _svc.GetMonthlyBreakdownAsync(year, month);
        return Ok(data.Select(x => new { date = x.date, total = x.total }));
    }


    [HttpGet("top-products")]
    public async Task<IActionResult> TopProducts([FromQuery] int top = 5, [FromQuery] DateTime? start = null, [FromQuery] DateTime? end = null)
    {
        var data = await _svc.GetTopProductsAsync(top, start, end);
        return Ok(data.Select(x => new { product = x.productName, quantity = x.quantity, total = x.total }));
    }


    [HttpGet("low-stock")]
    public async Task<IActionResult> LowStock([FromQuery] int threshold = 5)
    { var data = await _svc.GetLowStockAsync(threshold); return Ok(data); }
}
}
