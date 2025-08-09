namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

/// <summary>
/// Response model for CancelSale operation
/// </summary>
public class CancelSaleResult
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