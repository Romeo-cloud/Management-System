using Management_System_Api.Models.DTOs;

namespace Management_System_Api.Services.Interfaces
{
    public interface ISalesService
    {
        Task<SaleResponseDto> CreateAsync(SaleCreateDto dto, string userId);
        Task<SaleResponseDto> GetByIdAsync(int id);
        Task<IEnumerable<SaleResponseDto>> GetAllAsync();
        Task<IEnumerable<SaleResponseDto>> GetByUserAsync(string userId);
    }
}
