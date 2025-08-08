using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// API response model for GetSale operation
/// </summary>
public class GetSaleResponse
{
    /// <summary>
    /// The unique identifier of the sale
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
    public List<SaleItemResponse> Items { get; set; } = new List<SaleItemResponse>();

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

    /// <summary>
    /// Information about the user who created this sale
    /// </summary>
    public CreatedByUserResponse CreatedBy { get; set; } = new CreatedByUserResponse();
}

/// <summary>
/// Information about the user who created the sale
/// </summary>
public class CreatedByUserResponse
{
    /// <summary>
    /// The unique identifier of the user
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The username of the user
    /// </summary>
    public string Username { get; set; } = string.Empty;
}
