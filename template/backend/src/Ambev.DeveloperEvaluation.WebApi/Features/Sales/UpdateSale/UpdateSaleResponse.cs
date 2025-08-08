namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// API response model for UpdateSale operation
/// </summary>
public class UpdateSaleResponse
{
    /// <summary>
    /// The unique identifier of the updated sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The unique sale number/identifier
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the sale was made
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// The customer associated with this sale
    /// </summary>
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// The total amount of the sale
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// The branch/store where the sale was made
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// The collection of items included in this sale
    /// </summary>
    public List<UpdateSaleItemResponse> Items { get; set; } = new List<UpdateSaleItemResponse>();

    /// <summary>
    /// Whether the sale has been cancelled
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// The date and time when the sale was created in the system
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The date and time of the last update to the sale information
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// Response model for a sale item in the update operation
/// </summary>
public class UpdateSaleItemResponse
{
    /// <summary>
    /// The unique identifier of the item
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The product associated with this sale item
    /// </summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>
    /// The quantity of the product
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// The unit price of the product
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Any discount applied to this item
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// The discount percentage applied
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// The total amount for this item
    /// </summary>
    public decimal TotalAmount { get; set; }
}
