using Management_System_Api.Models.DTOs;

namespace Management_System_Api.Services.Interfaces
{
    public interface IProductService
    {
        /// <summary>
        /// Get all products including their categories.
        /// </summary>
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();

        /// <summary>
        /// Get a single product by ID including its category.
        /// </summary>
        Task<ProductResponseDto> GetByIdAsync(string productId);

        /// <summary>
        /// Create a new product with a valid CategoryId.
        /// </summary>
        Task<string> CreateAsync(ProductCreateDto dto);

        /// <summary>
        /// Update an existing product (optionally including category).
        /// </summary>
        Task UpdateAsync(string productId, ProductUpdateDto dto);

        /// <summary>
        /// Delete a product by its ProductId.
        /// </summary>
        Task DeleteAsync(string productId);

        /// <summary>
        /// Get all products that belong to a given category.
        /// </summary>
        Task<IEnumerable<ProductResponseDto>> GetByCategoryAsync(int categoryId);

        /// <summary>
        /// Search products by ProductId or Name (case-insensitive).
        /// </summary>
        Task<IEnumerable<ProductResponseDto>> SearchAsync(string query);
    }
}
