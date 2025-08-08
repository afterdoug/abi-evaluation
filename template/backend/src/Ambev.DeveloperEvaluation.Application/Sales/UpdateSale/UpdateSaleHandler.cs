using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Handler for UpdateSaleCommand
/// </summary>
public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of UpdateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the UpdateSaleCommand request
    /// </summary>
    /// <param name="command">The UpdateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale details</returns>
    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var existingSale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken) 
            ?? throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        if (existingSale.IsCancelled)
            throw new InvalidOperationException("Cannot update a cancelled sale");

        if (existingSale.SaleNumber != command.SaleNumber)
        {
            var saleWithSameNumber = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
            if (saleWithSameNumber != null && saleWithSameNumber.Id != command.Id)
                throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");
        }

        existingSale.SaleNumber = command.SaleNumber;
        existingSale.SaleDate = command.SaleDate;
        existingSale.Customer = command.Customer;
        existingSale.Branch = command.Branch;

        UpdateSaleItems(existingSale, command.Items);

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        var result = _mapper.Map<UpdateSaleResult>(updatedSale);
        return result;
    }

    private static void UpdateSaleItems(Sale sale, List<UpdateSaleItemCommand> itemCommands)
    {
        var processedItemIds = new List<Guid>();

        foreach (var itemCommand in itemCommands)
        {
            if (itemCommand.Id.HasValue)
            {
                var existingItem = sale.Items.FirstOrDefault(i => i.Id == itemCommand.Id.Value);
                if (existingItem != null)
                {
                    existingItem.Product = itemCommand.Product;
                    existingItem.Quantity = itemCommand.Quantity;
                    existingItem.UnitPrice = itemCommand.UnitPrice;

                    existingItem.ApplyQuantityBasedDiscount();

                    processedItemIds.Add(existingItem.Id);
                }
                else
                {
                    sale.AddItem(itemCommand.Product, itemCommand.Quantity, itemCommand.UnitPrice);
                }
            }
            else
            {
                var newItem = sale.AddItem(itemCommand.Product, itemCommand.Quantity, itemCommand.UnitPrice);
                processedItemIds.Add(newItem.Id);
            }
        }

        var itemsToRemove = sale.Items.Where(i => !processedItemIds.Contains(i.Id)).ToList();
        foreach (var itemToRemove in itemsToRemove)
        {
            sale.RemoveItem(itemToRemove.Id);
        }

        sale.CalculateTotalAmount();
    }
}
