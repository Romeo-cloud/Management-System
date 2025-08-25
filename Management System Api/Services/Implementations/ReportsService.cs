using AutoMapper;
using Management_System_Api.Data;
using Management_System_Api.Models.DTOs;
using Management_System_Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;


namespace Management_System_Api.Services.Implementations
{
    public class ReportsService : IReportsService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        public ReportsService(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

        public async Task<decimal> GetTodayTotalAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _db.Sales.Where(s => s.SoldAtUtc >= today && s.SoldAtUtc < today.AddDays(1)).SumAsync(s => (decimal?)s.TotalPrice) ?? 0m;
        }


        public async Task<IEnumerable<(DateTime date, decimal total)>> GetMonthlyBreakdownAsync(int year, int month)
        {
            var start = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddMonths(1);
            var grouped = await _db.Sales.Where(s => s.SoldAtUtc >= start && s.SoldAtUtc < end)
            .GroupBy(s => s.SoldAtUtc.Date)
            .Select(g => new { Date = g.Key, Total = g.Sum(x => x.TotalPrice) })
            .OrderBy(x => x.Date).ToListAsync();
            return grouped.Select(x => (x.Date, x.Total));
        }

        public async Task<IEnumerable<(string productName, int quantity, decimal total)>> GetTopProductsAsync(int topN, DateTime? start, DateTime? end)
        {
            var s = start ?? DateTime.UtcNow.AddDays(-30);
            var e = end ?? DateTime.UtcNow;
            var list = await _db.Sales.Where(x => x.SoldAtUtc >= s && x.SoldAtUtc <= e)
            .Include(x => x.Product)
            .GroupBy(x => new { x.ProductId, x.Product.Name })
            .Select(g => new { g.Key.Name, Qty = g.Sum(x => x.QuantitySold), Total = g.Sum(x => x.TotalPrice) })
            .OrderByDescending(x => x.Total).Take(topN).ToListAsync();
            return list.Select(x => (x.Name, x.Qty, x.Total));
        }


        public async Task<IEnumerable<ProductResponseDto>> GetLowStockAsync(int threshold)
        {
            var items = await _db.Products.Where(p => p.Quantity <= threshold && p.IsActive).ToListAsync();
            return _mapper.Map<IEnumerable<ProductResponseDto>>(items);
        }

    }
}
