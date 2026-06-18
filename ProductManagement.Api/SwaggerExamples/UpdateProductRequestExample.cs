using ProductManagement.Application.Contracts.Products;
using ProductManagement.Domain.Enums;
using Swashbuckle.AspNetCore.Filters;

namespace ProductManagement.Api.SwaggerExamples;

public sealed class UpdateProductRequestExample : IExamplesProvider<UpdateProductRequest>
{
    public UpdateProductRequest GetExamples()
    {
        return new UpdateProductRequest(
            Name: "Notebook Ultra 14 Pro",
            Sku: "NBK-ULTRA-14",
            Description: "Versão atualizada com processador de última geração.",
            Price: 7499.90m,
            StockQuantity: 28,
            Category: "Eletrônicos",
            Status: ProductStatus.Active);
    }
}
