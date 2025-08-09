using Ambev.DeveloperEvaluation.Application.Sales.Events;
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
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of CreateSaleHandler
    /// </summary>
    /// <param name="saleRepository">The sale repository</param>
    /// <param name="userRepository">The user repository</param>
    /// <param name="mapper">The AutoMapper instance</param>
    /// <param name="mediator">The mediator to handle event</param>
    public CreateSaleHandler(ISaleRepository saleRepository, IUserRepository userRepository, IMapper mapper, IMediator mediator)
    {
        _saleRepository = saleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _mediator = mediator;
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

        var existingSale = await _saleRepository.GetBySaleNumberAsync(command.SaleNumber, cancellationToken);
        if (existingSale != null)
            throw new InvalidOperationException($"Sale with number {command.SaleNumber} already exists");

        var user = await _userRepository.GetByIdAsync(command.CreatedById, cancellationToken);
        if (user == null)
            throw new InvalidOperationException($"User with ID {command.CreatedById} not found");

        var sale = new Sale
        {
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            Customer = command.Customer,
            Branch = command.Branch,
            CreatedById = command.CreatedById,
            CreatedBy = user
        };

        foreach (var itemCommand in command.Items)
        {
            sale.AddItem(itemCommand.Product, itemCommand.Quantity, itemCommand.UnitPrice);
        }

        var createdSale = await _saleRepository.CreateAsync(sale, cancellationToken);

        await _mediator.Publish(new SaleCreatedEvent(createdSale.Id), cancellationToken);

        var result = _mapper.Map<CreateSaleResult>(createdSale);
        return result;
    }
}