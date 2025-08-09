using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.Events;

/// <summary>
/// Handler for SaleCreated event.
/// </summary>
public class SaleCreatedHandler : INotificationHandler<SaleCreatedEvent>
{
    /// <summary>
    /// Handles the SaleCreated event.
    /// </summary>
    /// <param name="notification">The SaleCreated event notification.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task Handle(SaleCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Sale with ID {notification.SaleId} has been created.");

        return Task.CompletedTask;
    }
}