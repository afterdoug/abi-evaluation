using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Event that is published when a sale is created.
/// </summary>
public class SaleCreatedEvent : INotification
{
    /// <summary>
    /// The unique identifier of the created sale.
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// Initializes a new instance of SaleCreated.
    /// </summary>
    /// <param name="saleId">The ID of the created sale.</param>
    public SaleCreatedEvent(Guid saleId)
    {
        SaleId = saleId;
    }
}
