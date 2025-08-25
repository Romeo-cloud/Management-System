using Management_System_Api.Models.DTOs;
namespace Management_System_Api.Services.Interfaces
{
    public interface IReportsService
    {
        Task<decimal> GetTodayTotalAsync();
        Task<IEnumerable<(DateTime date, decimal total)>> GetMonthlyBreakdownAsync(int year, int month);
        Task<IEnumerable<(string productName, int quantity, decimal total)>> GetTopProductsAsync(int topN, DateTime? start, DateTime? end);
        Task<IEnumerable<ProductResponseDto>> GetLowStockAsync(int threshold);

    }
}
