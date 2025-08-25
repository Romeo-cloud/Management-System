using AutoMapper;
using Management_System_Api.Models.Domain;
using Management_System_Api.Services.Interfaces;
using Management_System_Api.Models.DTOs;
using Management_System_Api.Data;
using Microsoft.EntityFrameworkCore;
using Management_System_Api.Exceptions;

namespace Management_System_Api.Services.Implementations
{
    public class SalesService: ISalesService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        public SalesService(AppDbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

        public async Task<SaleResponseDto> CreateAsync(SaleCreateDto dto, string userId)
        {
            var product = await _db.Products.FirstOrDefaultAsync(p => p.ProductId == dto.ProductId && p.IsActive)
            ?? throw new NotFoundException($"Product {dto.ProductId} not found or inactive");
            if (dto.Quantity <= 0) throw new BadRequestException("Quantity must be > 0");
            if (product.Quantity < dto.Quantity) throw new BadRequestException("Insufficient stock");


            product.Quantity -= dto.Quantity;


            var sale = new Sale
            {
                ProductId = product.ProductId,
                QuantitySold = dto.Quantity,
                UnitPrice = product.Price,
                TotalPrice = product.Price * dto.Quantity,
                SoldByUserId = userId,
                CustomerName = dto.CustomerName,
                CustomerContact = dto.CustomerContact
            };
            _db.Sales.Add(sale);
            await _db.SaveChangesAsync();


            // Load navigation for mapping
            sale = await _db.Sales.Include(s => s.Product).Include(s => s.Invoice).FirstAsync(s => s.Id == sale.Id);
            return _mapper.Map<SaleResponseDto>(sale);
        }
        public async Task<SaleResponseDto> GetByIdAsync(int id)
        {
            var sale = await _db.Sales.Include(s => s.Product).Include(s => s.Invoice).AsNoTracking().FirstOrDefaultAsync(s => s.Id == id)
            ?? throw new NotFoundException($"Sale {id} not found");
            return _mapper.Map<SaleResponseDto>(sale);
        }


        public async Task<IEnumerable<SaleResponseDto>> GetAllAsync()
        {
            var list = await _db.Sales.Include(s => s.Product).Include(s => s.Invoice).AsNoTracking().OrderByDescending(s => s.SoldAtUtc).ToListAsync();
            return _mapper.Map<IEnumerable<SaleResponseDto>>(list);
        }


        public async Task<IEnumerable<SaleResponseDto>> GetByUserAsync(string userId)
        {
            var list = await _db.Sales.Include(s => s.Product).Include(s => s.Invoice)
            .Where(s => s.SoldByUserId == userId).AsNoTracking().OrderByDescending(s => s.SoldAtUtc).ToListAsync();
            return _mapper.Map<IEnumerable<SaleResponseDto>>(list);
        }
    }

}
