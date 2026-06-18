using ProductManagement.Domain.Enums;

namespace ProductManagement.Application.Contracts.Products;

public sealed record CreateProductRequest(
    string Name,
    string Sku,
    string Description,
    decimal Price,
    int StockQuantity,
    string Category,
    ProductStatus Status);
