using AutoMapper;
using Management_System_Api.Models.Domain;
using Management_System_Api.Models.DTOs;

namespace Management_System_Api.Mapping
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //  Product → ProductResponseDto with Category mapping
            CreateMap<Product, ProductResponseDto>()
                .ForMember(d => d.CategoryName,
                    o => o.MapFrom(s => s.Category != null ? s.Category.Name : string.Empty));

            //  ProductCreateDto → Product
            CreateMap<ProductCreateDto, Product>();

            //  ProductUpdateDto → Product
            CreateMap<ProductUpdateDto, Product>();


            //Sale → SaleResponseDto
            CreateMap<Sale, SaleResponseDto>()
                .ForMember(d => d.ProductName, o => o.MapFrom(s => s.Product.Name))
                .ForMember(d => d.ProductId, o => o.MapFrom(s => s.ProductId))
                .ForMember(d => d.Quantity, o => o.MapFrom(s => s.QuantitySold))
                .ForMember(d => d.InvoiceId, o => o.MapFrom(s => s.Invoice != null ? s.Invoice.Id : (int?)null))
                .ForMember(d => d.InvoiceNumber, o => o.MapFrom(s => s.Invoice != null ? s.Invoice.InvoiceNumber : null));

            //  Invoice → InvoiceResponseDto
            CreateMap<Invoice, InvoiceResponseDto>();


            // Category mappings
            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();
        }
    }
}
