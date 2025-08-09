using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="CreateSaleHandler"/> class.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly CreateSaleHandler _handler;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CreateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CreateSaleHandler(_saleRepository, _userRepository, _mapper, _mediator);
    }

    /// <summary>
    /// Tests that a valid sale creation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When creating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var user = new User
        {
            Id = command.CreatedById,
            Username = "TestUser",
            Email = "test@example.com"
        };

        var sale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            Customer = command.Customer,
            Branch = command.Branch,
            CreatedById = command.CreatedById,
            CreatedBy = user,
            Items = new List<SaleItem>()
        };

        // Add items to the sale
        foreach (var itemCommand in command.Items)
        {
            sale.AddItem(itemCommand.Product, itemCommand.Quantity, itemCommand.UnitPrice);
        }

        var result = new CreateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Customer = sale.Customer,
            Branch = sale.Branch,
            TotalAmount = sale.TotalAmount,
            Items = sale.Items.Select(i => new SaleItemResult
            {
                Id = i.Id,
                Product = i.Product,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                Discount = i.Discount,
                TotalAmount = i.TotalAmount
            }).ToList()
        };

        _userRepository.GetByIdAsync(command.CreatedById, Arg.Any<CancellationToken>())
            .Returns(user);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<CreateSaleResult>(sale).Returns(result);

        // When
        var createSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createSaleResult.Should().NotBeNull();
        createSaleResult.Id.Should().Be(sale.Id);
        createSaleResult.SaleNumber.Should().Be(sale.SaleNumber);
        createSaleResult.Customer.Should().Be(sale.Customer);
        createSaleResult.Branch.Should().Be(sale.Branch);
        createSaleResult.Items.Should().HaveCount(command.Items.Count);

        await _saleRepository.Received(1).CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale creation request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When creating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateSaleCommand(); // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that creating a sale with an existing sale number throws an exception.
    /// </summary>
    [Fact(DisplayName = "Given existing sale number When creating sale Then throws exception")]
    public async Task Handle_ExistingSaleNumber_ThrowsException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = new Sale
        {
            Id = Guid.NewGuid(),
            SaleNumber = command.SaleNumber
        };

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with number {command.SaleNumber} already exists");
    }

    /// <summary>
    /// Tests that creating a sale with a non-existent user throws an exception.
    /// </summary>
    [Fact(DisplayName = "Given non-existent user When creating sale Then throws exception")]
    public async Task Handle_NonExistentUser_ThrowsException()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        _userRepository.GetByIdAsync(command.CreatedById, Arg.Any<CancellationToken>())
            .Returns((User)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"User with ID {command.CreatedById} not found");
    }

    /// <summary>
    /// Tests that items are correctly added to the sale.
    /// </summary>
    [Fact(DisplayName = "Given sale with items When creating sale Then items are added correctly")]
    public async Task Handle_SaleWithItems_AddsItemsCorrectly()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var user = new User { Id = command.CreatedById };

        _userRepository.GetByIdAsync(command.CreatedById, Arg.Any<CancellationToken>())
            .Returns(user);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo => new CreateSaleResult
            {
                Id = callInfo.Arg<Sale>().Id,
                Items = callInfo.Arg<Sale>().Items.Select(i => new SaleItemResult
                {
                    Product = i.Product,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            });

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).CreateAsync(
            Arg.Is<Sale>(s =>
                s.Items.Count == command.Items.Count &&
                s.Items.All(item => command.Items.Any(ci =>
                    ci.Product == item.Product &&
                    ci.Quantity == item.Quantity &&
                    ci.UnitPrice == item.UnitPrice))),
            Arg.Any<CancellationToken>());

        result.Items.Should().HaveCount(command.Items.Count);
    }

    /// <summary>
    /// Tests that the discount rules are applied correctly to items.
    /// </summary>
    [Fact(DisplayName = "Given items with different quantities When creating sale Then discount rules are applied correctly")]
    public async Task Handle_ItemsWithDifferentQuantities_AppliesDiscountRulesCorrectly()
    {
        // Given
        var command = new CreateSaleCommand
        {
            SaleNumber = "TEST-001",
            SaleDate = DateTime.UtcNow,
            Customer = "Test Customer",
            Branch = "Test Branch",
            CreatedById = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand { Product = "Product1", Quantity = 3, UnitPrice = 10 }, // No discount
                new CreateSaleItemCommand { Product = "Product2", Quantity = 5, UnitPrice = 10 }, // 10% discount
                new CreateSaleItemCommand { Product = "Product3", Quantity = 15, UnitPrice = 10 } // 20% discount
            }
        };

        var user = new User { Id = command.CreatedById };

        _userRepository.GetByIdAsync(command.CreatedById, Arg.Any<CancellationToken>())
            .Returns(user);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        Sale capturedSale = null;
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                capturedSale = callInfo.Arg<Sale>();
                return capturedSale;
            });

        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo => new CreateSaleResult
            {
                Id = callInfo.Arg<Sale>().Id,
                Items = callInfo.Arg<Sale>().Items.Select(i => new SaleItemResult
                {
                    Product = i.Product,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Discount = i.Discount,
                    DiscountPercentage = i.DiscountPercentage,
                    TotalAmount = i.TotalAmount
                }).ToList()
            });

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        capturedSale.Items.Should().HaveCount(3);

        // Check Product1 (no discount)
        var item1 = capturedSale.Items.First(i => i.Product == "Product1");
        item1.DiscountPercentage.Should().Be(0);
        item1.Discount.Should().Be(0);
        item1.TotalAmount.Should().Be(30); // 3 * 10

        // Check Product2 (10% discount)
        var item2 = capturedSale.Items.First(i => i.Product == "Product2");
        item2.DiscountPercentage.Should().Be(0.10m);
        item2.Discount.Should().Be(5); // 5 * 10 * 0.1
        item2.TotalAmount.Should().Be(45); // 5 * 10 - 5

        // Check Product3 (20% discount)
        var item3 = capturedSale.Items.First(i => i.Product == "Product3");
        item3.DiscountPercentage.Should().Be(0.20m);
        item3.Discount.Should().Be(30); // 15 * 10 * 0.2
        item3.TotalAmount.Should().Be(120); // 15 * 10 - 30
    }

    /// <summary>
    /// Tests that the total amount is calculated correctly.
    /// </summary>
    [Fact(DisplayName = "Given sale with multiple items When creating sale Then total amount is calculated correctly")]
    public async Task Handle_SaleWithMultipleItems_CalculatesTotalAmountCorrectly()
    {
        // Given
        var command = CreateSaleHandlerTestData.GenerateValidCommand();
        var user = new User { Id = command.CreatedById };

        _userRepository.GetByIdAsync(command.CreatedById, Arg.Any<CancellationToken>())
            .Returns(user);

        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        Sale capturedSale = null;
        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                capturedSale = callInfo.Arg<Sale>();
                return capturedSale;
            });

        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo => new CreateSaleResult
            {
                Id = callInfo.Arg<Sale>().Id,
                TotalAmount = callInfo.Arg<Sale>().TotalAmount
            });

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();

        // Calculate expected total amount
        decimal expectedTotal = 0;
        foreach (var item in capturedSale.Items)
        {
            expectedTotal += item.TotalAmount;
        }

        capturedSale.TotalAmount.Should().Be(expectedTotal);
        result.TotalAmount.Should().Be(expectedTotal);
    }
}