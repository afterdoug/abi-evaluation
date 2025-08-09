using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Event that is published when a sale is modified.
/// </summary>
public class SaleModifiedEvent : INotification
{
    /// <summary>
    /// The unique identifier of the modified sale.
    /// </summary>
    public Guid SaleId { get; }

    /// <summary>
    /// Initializes a new instance of SaleModified.
    /// </summary>
    /// <param name="saleId">The ID of the modified sale.</param>
    public SaleModifiedEvent(Guid saleId)
    {
        SaleId = saleId;
    }
}