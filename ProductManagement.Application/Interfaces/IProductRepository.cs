using ProductManagement.Application.Contracts.Common;
using ProductManagement.Application.Contracts.Products;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Interfaces;

public interface IProductRepository
{
    Task<PagedResult<Product>> GetPagedAsync(ProductQueryRequest request, CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsBySkuAsync(string sku, Guid? ignoreId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
