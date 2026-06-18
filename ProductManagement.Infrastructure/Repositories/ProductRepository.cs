using Microsoft.EntityFrameworkCore;
using ProductManagement.Application.Contracts.Common;
using ProductManagement.Application.Contracts.Products;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;
using ProductManagement.Infrastructure.Persistence;

namespace ProductManagement.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private static readonly Dictionary<string, Func<IQueryable<Product>, bool, IQueryable<Product>>> SortMap =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ["name"] = (query, asc) => asc ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name),
            ["sku"] = (query, asc) => asc ? query.OrderBy(x => x.Sku) : query.OrderByDescending(x => x.Sku),
            ["price"] = (query, asc) => asc ? query.OrderBy(x => x.Price) : query.OrderByDescending(x => x.Price),
            ["stockQuantity"] = (query, asc) => asc ? query.OrderBy(x => x.StockQuantity) : query.OrderByDescending(x => x.StockQuantity),
            ["category"] = (query, asc) => asc ? query.OrderBy(x => x.Category) : query.OrderByDescending(x => x.Category),
            ["status"] = (query, asc) => asc ? query.OrderBy(x => x.Status) : query.OrderByDescending(x => x.Status),
            ["createdAtUtc"] = (query, asc) => asc ? query.OrderBy(x => x.CreatedAtUtc) : query.OrderByDescending(x => x.CreatedAtUtc)
        };

    private readonly ProductManagementDbContext _dbContext;

    public ProductRepository(ProductManagementDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<Product>> GetPagedAsync(ProductQueryRequest request, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var normalizedName = request.Name.Trim();
            query = query.Where(x => x.Name.Contains(normalizedName));
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            var normalizedCategory = request.Category.Trim();
            query = query.Where(x => x.Category.Contains(normalizedCategory));
        }

        if (request.Status.HasValue)
        {
            query = query.Where(x => x.Status == request.Status.Value);
        }

        var sortBy = string.IsNullOrWhiteSpace(request.SortBy) ? "createdAtUtc" : request.SortBy.Trim();
        var sortAscending = request.SortDirection?.Equals("asc", StringComparison.OrdinalIgnoreCase) ?? false;

        if (!SortMap.TryGetValue(sortBy, out var applySort))
        {
            applySort = SortMap["createdAtUtc"];
        }

        query = applySort(query, sortAscending);

        var totalItems = await query.CountAsync(cancellationToken);
        var totalPages = totalItems == 0 ? 0 : (int)Math.Ceiling(totalItems / (double)request.PageSize);

        var items = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Product>(items, request.Page, request.PageSize, totalItems, totalPages);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> ExistsBySkuAsync(string sku, Guid? ignoreId = null, CancellationToken cancellationToken = default)
    {
        var normalizedSku = sku.Trim().ToUpperInvariant();

        return _dbContext.Products
            .AnyAsync(x => x.Sku == normalizedSku && (!ignoreId.HasValue || x.Id != ignoreId), cancellationToken);
    }

    public Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products.AddAsync(product, cancellationToken).AsTask();
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
