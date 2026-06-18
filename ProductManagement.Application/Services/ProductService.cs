using FluentValidation;
using Mapster;
using ProductManagement.Application.Common.Exceptions;
using ProductManagement.Application.Contracts.Common;
using ProductManagement.Application.Contracts.Products;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Services;

public sealed class ProductService : IProductService
{
    private readonly IProductRepository _repository;
    private readonly IValidator<CreateProductRequest> _createValidator;
    private readonly IValidator<UpdateProductRequest> _updateValidator;
    private readonly IValidator<ProductQueryRequest> _queryValidator;

    public ProductService(
        IProductRepository repository,
        IValidator<CreateProductRequest> createValidator,
        IValidator<UpdateProductRequest> updateValidator,
        IValidator<ProductQueryRequest> queryValidator)
    {
        _repository = repository;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _queryValidator = queryValidator;
    }

    public async Task<PagedResult<ProductDto>> GetPagedAsync(ProductQueryRequest request, CancellationToken cancellationToken = default)
    {
        await _queryValidator.ValidateAndThrowAsync(request, cancellationToken);
        var pagedProducts = await _repository.GetPagedAsync(request, cancellationToken);

        var mappedItems = pagedProducts.Items
            .Select(MapToDto)
            .ToList();

        return new PagedResult<ProductDto>(
            mappedItems,
            pagedProducts.Page,
            pagedProducts.PageSize,
            pagedProducts.TotalItems,
            pagedProducts.TotalPages);
    }

    public async Task<ProductDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{id}' was not found.");

        return MapToDto(product);
    }

    public async Task<ProductDto> CreateAsync(CreateProductRequest request, CancellationToken cancellationToken = default)
    {
        await _createValidator.ValidateAndThrowAsync(request, cancellationToken);

        var skuAlreadyExists = await _repository.ExistsBySkuAsync(request.Sku, null, cancellationToken);
        if (skuAlreadyExists)
        {
            throw new ConflictException($"SKU '{request.Sku}' already exists.");
        }

        var product = Product.Create(
            request.Name,
            request.Sku,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.Category,
            request.Status);

        await _repository.AddAsync(product, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task<ProductDto> UpdateAsync(Guid id, UpdateProductRequest request, CancellationToken cancellationToken = default)
    {
        await _updateValidator.ValidateAndThrowAsync(request, cancellationToken);

        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{id}' was not found.");

        var skuAlreadyExists = await _repository.ExistsBySkuAsync(request.Sku, id, cancellationToken);
        if (skuAlreadyExists)
        {
            throw new ConflictException($"SKU '{request.Sku}' already exists.");
        }

        product.Update(
            request.Name,
            request.Sku,
            request.Description,
            request.Price,
            request.StockQuantity,
            request.Category,
            request.Status);

        await _repository.SaveChangesAsync(cancellationToken);
        return MapToDto(product);
    }

    public async Task InactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{id}' was not found.");

        product.Inactivate();
        await _repository.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await _repository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException($"Product with id '{id}' was not found.");

        product.Activate();
        await _repository.SaveChangesAsync(cancellationToken);
    }

    private static ProductDto MapToDto(Product product)
    {
        return product.Adapt<ProductDto>();
    }
}
