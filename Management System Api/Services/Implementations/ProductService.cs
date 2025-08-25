using AutoMapper;
using Management_System_Api.Models.Domain;
using Management_System_Api.Services.Interfaces;
using Management_System_Api.Models.DTOs;
using Management_System_Api.Data;
using Management_System_Api.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Management_System_Api.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        //  Get all products including category
        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync() =>
            _mapper.Map<IEnumerable<ProductResponseDto>>(
                await _db.Products
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .OrderBy(p => p.ProductId)
                    .ToListAsync()
            );

        // Get by id including category
        public async Task<ProductResponseDto> GetByIdAsync(string productId)
        {
            var entity = await _db.Products
                .Include(p => p.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == productId);

            if (entity is null)
                throw new NotFoundException($"Product with ID '{productId}' not found.");

            return _mapper.Map<ProductResponseDto>(entity);
        }

        //  Create product with category validation
        public async Task<string> CreateAsync(ProductCreateDto dto)
        {
            if (await _db.Products.AnyAsync(p => p.ProductId == dto.ProductId))
                throw new BadRequestException($"Product ID '{dto.ProductId}' already exists.");

            var category = await _db.Categories
                .FirstOrDefaultAsync(c => c.Id == dto.CategoryId);

            if (category == null)
                throw new BadRequestException($"Category with ID '{dto.CategoryId}' not found.");

            var entity = _mapper.Map<Product>(dto);
            entity.CategoryId = dto.CategoryId;
            entity.CreatedAtUtc = DateTime.UtcNow;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            _db.Products.Add(entity);
            await _db.SaveChangesAsync();

            return entity.ProductId;
        }

        //  Update product and category (with audit)
        public async Task UpdateAsync(string productId, ProductUpdateDto dto)
        {
            var entity = await _db.Products.FindAsync(productId)
                ?? throw new NotFoundException($"Product with ID '{productId}' not found.");

            // Category validation
            if (dto.CategoryId != null)
            {
                var category = await _db.Categories
                    .FirstOrDefaultAsync(c => c.Id == dto.CategoryId.Value);

                if (category == null)
                    throw new BadRequestException($"Category with ID '{dto.CategoryId.Value}' not found.");

                entity.CategoryId = dto.CategoryId.Value;
            }

            // Map updated fields
            _mapper.Map(dto, entity);
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();
        }

        //  Get products by category
        public async Task<IEnumerable<ProductResponseDto>> GetByCategoryAsync(int categoryId)
        {
            var products = await _db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p => p.CategoryId == categoryId)
                .OrderBy(p => p.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        //  Enhanced Search (case-insensitive, ProductId or Name)
        public async Task<IEnumerable<ProductResponseDto>> SearchAsync(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<ProductResponseDto>();

            query = query.Trim();

            var products = await _db.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Where(p =>
                    EF.Functions.Like(p.ProductId, $"%{query}%") ||
                    EF.Functions.Like(p.Name, $"%{query}%"))
                .OrderBy(p => p.Name)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
        }

        // Delete product safely
        public async Task DeleteAsync(string productId)
        {
            var entity = await _db.Products.FindAsync(productId)
                ?? throw new NotFoundException($"Product with ID '{productId}' not found.");

            _db.Products.Remove(entity);
            await _db.SaveChangesAsync();
        }
    }
}
