using FluentAssertions;
using FluentValidation;
using NSubstitute;
using ProductManagement.Application.Common.Exceptions;
using ProductManagement.Application.Contracts.Common;
using ProductManagement.Application.Contracts.Products;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;
using ProductManagement.Application.Validators;
using ProductManagement.Domain.Entities;
using ProductManagement.Domain.Enums;

namespace ProductManagement.Application.Tests.Services;

public sealed class ProductServiceTests
{
    private readonly IProductRepository _repository = Substitute.For<IProductRepository>();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        IValidator<CreateProductRequest> createValidator = new CreateProductRequestValidator();
        IValidator<UpdateProductRequest> updateValidator = new UpdateProductRequestValidator();
        IValidator<ProductQueryRequest> queryValidator = new ProductQueryRequestValidator();

        _service = new ProductService(_repository, createValidator, updateValidator, queryValidator);
    }

    [Fact]
    public async Task CreateAsync_ShouldThrowConflict_WhenSkuAlreadyExists()
    {
        var request = new CreateProductRequest("Produto A", "SKU-001", "Descricao", 10m, 5, "Categoria", ProductStatus.Active);
        _repository.ExistsBySkuAsync(request.Sku, null, Arg.Any<CancellationToken>()).Returns(true);

        var action = async () => await _service.CreateAsync(request, CancellationToken.None);

        await action.Should().ThrowAsync<ConflictException>();
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnMappedProducts()
    {
        var product = Product.Create("Produto A", "SKU-001", "Descricao", 100m, 20, "Eletronicos", ProductStatus.Active);
        var pagedResult = new PagedResult<Product>(new[] { product }, 1, 10, 1, 1);

        _repository.GetPagedAsync(Arg.Any<ProductQueryRequest>(), Arg.Any<CancellationToken>()).Returns(pagedResult);

        var result = await _service.GetPagedAsync(new ProductQueryRequest(), CancellationToken.None);

        result.TotalItems.Should().Be(1);
        result.Items.Should().HaveCount(1);
        result.Items.First().Sku.Should().Be("SKU-001");
    }
}
