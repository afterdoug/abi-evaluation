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

        await _saleRepository.RemoveItemsAsync(existingSale, cancellationToken);
        UpdateSaleItems(existingSale, command.Items);

        var updatedSale = await _saleRepository.UpdateAsync(existingSale, cancellationToken);

        var result = _mapper.Map<UpdateSaleResult>(updatedSale);
        return result;
    }

    private static void UpdateSaleItems(Sale sale, List<UpdateSaleItemCommand> itemCommands)
    {
        sale.Items.Clear();
        foreach (var itemCommand in itemCommands)
        {
            var newItem = new SaleItem
            {
                Id = itemCommand.Id.HasValue && itemCommand.Id.Value != Guid.Empty
                    ? itemCommand.Id.Value
                    : Guid.NewGuid(),
                Product = itemCommand.Product,
                Quantity = itemCommand.Quantity,
                UnitPrice = itemCommand.UnitPrice
            };

            newItem.ApplyQuantityBasedDiscount();
            sale.Items.Add(newItem);
        }

        sale.CalculateTotalAmount();
    }
}
