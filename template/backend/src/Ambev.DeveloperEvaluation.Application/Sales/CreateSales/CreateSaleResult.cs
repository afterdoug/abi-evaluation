namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Represents the response returned after successfully creating a new sale.
/// </summary>
public class CreateSaleResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the newly created sale.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the unique sale number/identifier.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets or sets the customer associated with this sale.
    /// </summary>
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the total amount of the sale.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets or sets the branch/store where the sale was made.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of items included in this sale.
    /// </summary>
    public List<SaleItemResult> Items { get; set; } = new List<SaleItemResult>();

    /// <summary>
    /// Gets or sets whether the sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the sale was created in the system.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// Represents an item in a sale result
/// </summary>
public class SaleItemResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the item.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product associated with this sale item.
    /// </summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets any discount applied to this item.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied.
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the total amount for this item.
    /// </summary>
    public decimal TotalAmount { get; set; }
}