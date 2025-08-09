using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Event that is published when a sale is cancelled.
/// </summary>
public class SaleCancelledEvent : INotification
{
    /// <summary>
    /// The unique identifier of the cancelled sale.
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// Initializes a new instance of SaleCancelled.
    /// </summary>
    /// <param name="saleId">The ID of the cancelled sale.</param>
    public SaleCancelledEvent(Guid saleId)
    {
        SaleId = saleId;
    }
}
