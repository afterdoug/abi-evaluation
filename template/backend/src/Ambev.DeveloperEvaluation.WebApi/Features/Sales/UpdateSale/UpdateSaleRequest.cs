using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;

/// <summary>
/// Request model for updating an existing sale
/// </summary>
public class UpdateSaleRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the sale to update.
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
    /// Gets or sets the branch/store where the sale was made.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the collection of items included in this sale.
    /// </summary>
    public List<UpdateSaleItemRequest> Items { get; set; } = [];
}

/// <summary>
/// Request model for updating a sale item
/// </summary>
public class UpdateSaleItemRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the item.
    /// </summary>
    public Guid? Id { get; set; }

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
}