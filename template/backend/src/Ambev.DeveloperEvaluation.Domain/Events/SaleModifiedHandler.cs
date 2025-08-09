using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Handler for SaleModified event.
/// </summary>
public class SaleModifiedHandler : INotificationHandler<SaleModifiedEvent>
{
    /// <summary>
    /// Handles the SaleModified event.
    /// </summary>
    /// <param name="notification">The SaleModified event notification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task Handle(SaleModifiedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sale with ID {notification.SaleId} has been modified.");

        return Task.CompletedTask;
    }
}