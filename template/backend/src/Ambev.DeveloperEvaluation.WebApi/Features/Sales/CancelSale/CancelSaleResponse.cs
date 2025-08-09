namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

/// <summary>
/// API response model for CancelSale operation
/// </summary>
public class CancelSaleResponse
{
    /// <summary>
    /// The unique identifier of the cancelled sale
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Indicates whether the sale was successfully cancelled
    /// </summary>
    public bool IsCancelled { get; set; }
}