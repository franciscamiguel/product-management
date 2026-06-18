using ProductManagement.Application.Contracts.Products;
using ProductManagement.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace ProductManagement.Api.SwaggerExamples;

public sealed class CreateProductRequestExample : IExamplesProvider<CreateProductRequest>
{
    public CreateProductRequest GetExamples()
    {
        return new CreateProductRequest(
            Name: "Notebook Ultra 14",
            Sku: "NBK-ULTRA-14",
            Description: "Notebook ultrafino com 16GB RAM e SSD 512GB.",
            Price: 6999.90m,
            StockQuantity: 35,
            Category: "Eletrônicos",
            Status: ProductStatus.Active);
    }
}
