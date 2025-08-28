using Management_System_Api.Data;
using Management_System_Api.Models.Domain;
using Management_System_Api.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace Management_System_Api.Services
{
    public class CreditService
    {
        private readonly AppDbContext _context;

        public CreditService(AppDbContext context)
        {
            _context = context;
        }

        // 🔹 Create a new credit sale
        public async Task<CreditSaleResponseDto> CreateCreditSaleAsync(CreateCreditSaleDto dto)
        {
            var creditSale = new CreditSale
            {
                CustomerName = dto.CustomerName,
                Address = dto.Address,
                Telephone = dto.Telephone,
                AmountOwed = dto.AmountOwed,
                AmountPaid = 0
            };

            _context.CreditSales.Add(creditSale);
            await _context.SaveChangesAsync();

            return MapToResponseDto(creditSale);
        }

        // 🔹 Record a payment
        public async Task<CreditSaleResponseDto?> RecordPaymentAsync(RecordPaymentDto dto)
        {
            var sale = await _context.CreditSales
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.Id == dto.CreditSaleId);

            if (sale == null) return null;

            var payment = new CreditPayment
            {
                CreditSaleId = sale.Id,
                Amount = dto.Amount
            };

            sale.AmountPaid += dto.Amount;
            sale.Payments.Add(payment);

            await _context.SaveChangesAsync();

            return MapToResponseDto(sale);
        }

        // 🔹 Get all credits with history
        public async Task<IEnumerable<CreditSaleResponseDto>> GetAllCreditsAsync()
        {
            var credits = await _context.CreditSales
                .Include(c => c.Payments)
                .ToListAsync();

            return credits.Select(MapToResponseDto);
        }

        // 🔹 Utility: map domain model to response DTO
        private CreditSaleResponseDto MapToResponseDto(CreditSale sale)
        {
            return new CreditSaleResponseDto
            {
                Id = sale.Id,
                CustomerName = sale.CustomerName,
                Address = sale.Address,
                Telephone = sale.Telephone,
                AmountOwed = sale.AmountOwed,
                AmountPaid = sale.AmountPaid,
                Balance = sale.Balance,
                CreatedAt = sale.CreatedAt,
                Payments = sale.Payments.Select(p => new CreditPaymentResponseDto
                {
                    Id = p.Id,
                    Amount = p.Amount,
                    PaidAt = p.PaidAt
                }).ToList()
            };
        }
    }
}
