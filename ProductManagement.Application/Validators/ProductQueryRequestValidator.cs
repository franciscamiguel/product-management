using FluentValidation;
using ProductManagement.Application.Contracts.Products;

namespace ProductManagement.Application.Validators;

public sealed class ProductQueryRequestValidator : AbstractValidator<ProductQueryRequest>
{
    public ProductQueryRequestValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).GreaterThan(0).LessThanOrEqualTo(100);
        RuleFor(x => x.SortDirection)
            .Must(x => x is null || x.Equals("asc", StringComparison.OrdinalIgnoreCase) || x.Equals("desc", StringComparison.OrdinalIgnoreCase))
            .WithMessage("SortDirection must be 'asc' or 'desc'.");
    }
}
