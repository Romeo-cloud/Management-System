using AutoMapper;
using Management_System_Api.Data;
using Management_System_Api.Models.Domain;
using Management_System_Api.Models.DTOs;
using Management_System_Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Management_System_Api.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public CategoryService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponseDto>> GetAllAsync()
        {
            var categories = await _db.Categories.ToListAsync();
            return _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        }

        public async Task<CategoryResponseDto> GetByIdAsync(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category is null)
                throw new KeyNotFoundException("Category not found");

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto)
        {
            if (await _db.Categories.AnyAsync(c => c.Name == dto.Name))
                throw new InvalidOperationException("Category already exists.");

            var category = _mapper.Map<Category>(dto);
            _db.Categories.Add(category);
            await _db.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task<CategoryResponseDto> UpdateAsync(int id, CategoryUpdateDto dto)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category is null)
                throw new KeyNotFoundException("Category not found");

            category.Name = dto.Name;
            await _db.SaveChangesAsync();

            return _mapper.Map<CategoryResponseDto>(category);
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _db.Categories.FindAsync(id);
            if (category is null)
                throw new KeyNotFoundException("Category not found");

            _db.Categories.Remove(category);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId)
        {
            var category = await _db.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category is null)
                throw new KeyNotFoundException("Category not found");

            return _mapper.Map<IEnumerable<ProductResponseDto>>(category.Products);
        }
    }
}
