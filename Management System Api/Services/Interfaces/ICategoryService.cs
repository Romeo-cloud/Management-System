using Management_System_Api.Models.DTOs;

namespace Management_System_Api.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto> GetByIdAsync(int id);
        Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto);
        Task<CategoryResponseDto> UpdateAsync(int id, CategoryUpdateDto dto);
        Task DeleteAsync(int id);
        Task<IEnumerable<ProductResponseDto>> GetProductsByCategoryAsync(int categoryId);
    }
}
