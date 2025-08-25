using Management_System_Api.Models.DTOs;
using Management_System_Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Management_System_Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET: api/categories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        // GET: api/categories/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            return Ok(category);
        }

        // POST: api/categories
        [HttpPost]
        public async Task<ActionResult<CategoryResponseDto>> Create(CategoryCreateDto dto)
        {
            var created = await _categoryService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // PUT: api/categories/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<CategoryResponseDto>> Update(int id, CategoryUpdateDto dto)
        {
            var updated = await _categoryService.UpdateAsync(id, dto);
            return Ok(updated);
        }

        // DELETE: api/categories/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _categoryService.DeleteAsync(id);
            return NoContent();
        }

        // GET: api/categories/{id}/products
        [HttpGet("{id}/products")]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProductsByCategory(int id)
        {
            var products = await _categoryService.GetProductsByCategoryAsync(id);
            return Ok(products);
        }
    }
}
