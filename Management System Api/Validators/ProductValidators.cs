using FluentValidation;
using Management_System_Api.Models.DTOs;

namespace Management_System_Api.Validators
{
    public class ProductCreateValidator : AbstractValidator<ProductCreateDto>
    {
        public ProductCreateValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().MaximumLength(64).Matches("^[A-Za-z0-9_-]+$").WithMessage("ProductId must be alphanumeric (dashes/underscores allowed)");
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        }
    }


    public class ProductUpdateValidator : AbstractValidator<ProductUpdateDto>
    {
        public ProductUpdateValidator()
        {
            RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
            RuleFor(x => x.Price).GreaterThanOrEqualTo(0);
            RuleFor(x => x.Quantity).GreaterThanOrEqualTo(0);
        }
    }


    public class SaleCreateValidator : AbstractValidator<SaleCreateDto>
    {
        public SaleCreateValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty();
            RuleFor(x => x.Quantity).GreaterThan(0);
        }
    }
}
