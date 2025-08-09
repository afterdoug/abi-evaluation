using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Handler for SaleCancelled event.
/// </summary>
public class SaleCancelledHandler : INotificationHandler<SaleCancelledEvent>
{
    /// <summary>
    /// Handles the SaleCancelled event.
    /// </summary>
    /// <param name="notification">The SaleCancelled event notification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task Handle(SaleCancelledEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sale with ID {notification.SaleId} has been cancelled.");

        return Task.CompletedTask;
    }
}
