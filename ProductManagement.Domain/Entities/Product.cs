using ProductManagement.Domain.Common;
using ProductManagement.Domain.Enums;

namespace ProductManagement.Domain.Entities;

public class Product : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Sku { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public ProductStatus Status { get; private set; } = ProductStatus.Active;
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAtUtc { get; private set; }

    public static Product Create(
        string name,
        string sku,
        string description,
        decimal price,
        int stockQuantity,
        string category,
        ProductStatus status)
    {
        return new Product
        {
            Name = name.Trim(),
            Sku = sku.Trim().ToUpperInvariant(),
            Description = description.Trim(),
            Price = price,
            StockQuantity = stockQuantity,
            Category = category.Trim(),
            Status = status,
            IsDeleted = false
        };
    }

    public void Update(
        string name,
        string sku,
        string description,
        decimal price,
        int stockQuantity,
        string category,
        ProductStatus status)
    {
        Name = name.Trim();
        Sku = sku.Trim().ToUpperInvariant();
        Description = description.Trim();
        Price = price;
        StockQuantity = stockQuantity;
        Category = category.Trim();
        Status = status;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Inactivate()
    {
        Status = ProductStatus.Inactive;
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Activate()
    {
        Status = ProductStatus.Active;
        IsDeleted = false;
        DeletedAtUtc = null;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
