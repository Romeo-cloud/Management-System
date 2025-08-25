using Management_System_Api.Models.DTOs;
using Management_System_Api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Management_System_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _svc;
        public ProductsController(IProductService svc) => _svc = svc;

        // Get all products (with category included)
        [HttpGet]
        [Authorize(Roles = "Admin,Operator,Salesperson")]
        public async Task<IActionResult> GetAll() => Ok(await _svc.GetAllAsync());

        // Get a single product (with category included)
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Operator,Salesperson")]
        public async Task<IActionResult> Get(string id) => Ok(await _svc.GetByIdAsync(id));

        // Get products by category
        [HttpGet("category/{categoryId:int}")]
        [Authorize(Roles = "Admin,Operator,Salesperson")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            var products = await _svc.GetByCategoryAsync(categoryId);
            return Ok(products);
        }

        // Create product (Admin only)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            var id = await _svc.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, new { id });
        }

        // Update product (Admin + Operator only — Salesperson removed)
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Operator")]
        public async Task<IActionResult> Update(string id, [FromBody] ProductUpdateDto dto)
        {
            await _svc.UpdateAsync(id, dto);
            return NoContent();
        }

        // Search product (by ID or name)
        [HttpGet("search")]
        [Authorize(Roles = "Admin,Operator,Salesperson")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var results = await _svc.SearchAsync(query);
            return Ok(results);
        }

        // Delete product (Admin only)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            await _svc.DeleteAsync(id);
            return NoContent();
        }
    }
}
