using ProductManagement.Domain.Enums;

namespace ProductManagement.Application.Contracts.Products;

public sealed class ProductQueryRequest
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
    public string? Name { get; init; }
    public string? Category { get; init; }
    public ProductStatus? Status { get; init; }
    public string? SortBy { get; init; } = "createdAtUtc";
    public string? SortDirection { get; init; } = "desc";
}
