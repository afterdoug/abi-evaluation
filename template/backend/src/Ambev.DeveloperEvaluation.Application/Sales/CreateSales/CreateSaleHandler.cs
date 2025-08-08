using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

/// <summary>
/// Handler for CreateSaleCommand
/// </summary>
public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="userRepository">The user repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public CreateSaleHandler(ISaleRepository saleRepository, IUserRepository userRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the CreateSaleCommand request
    /// </summary>
    /// <param name="command">The CreateSale command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        // Check if sale number already exists
        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale != null)
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");

        // Check if user exists
        var user = await _userRepository.GetByIdAsync(command.CreatedById, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with ID {command.CreatedById} not found");

        // Create sale entity
        var sale = new Sale
        {
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            Customer = command.Customer,
            Branch = command.Branch,
            CreatedById = command.CreatedById,
            CreatedBy = user
        };

        // Add items to sale
        foreach (var itemCommand in command.Items)
        {
            sale.AddItem(itemCommand.Product, itemCommand.Quantity, itemCommand.UnitPrice);
        }

        // Save sale to repository
        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        // Map to result
        var result = _mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }
}