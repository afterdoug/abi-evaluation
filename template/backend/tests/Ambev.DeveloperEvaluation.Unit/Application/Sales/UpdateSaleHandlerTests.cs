using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="UpdateSaleHandler"/> class.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly UpdateSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid sale update request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale data When updating sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = UpdateSaleHandlerTestData.GenerateExistingSale(command.Id);
        var updatedSale = UpdateSaleHandlerTestData.GenerateUpdatedSale(command);
        var result = UpdateSaleHandlerTestData.GenerateUpdateResult(updatedSale);

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.RemoveItemsAsync(existingSale, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(updatedSale);
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(result);

        // When
        var updateSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateSaleResult.Should().NotBeNull();
        updateSaleResult.Id.Should().Be(command.Id);
        updateSaleResult.SaleNumber.Should().Be(command.SaleNumber);
        updateSaleResult.Customer.Should().Be(command.Customer);
        updateSaleResult.Branch.Should().Be(command.Branch);
        await _saleRepository.Received(1).RemoveItemsAsync(existingSale, Arg.Any<CancellationToken>());
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale update request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale data When updating sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new UpdateSaleCommand(); // Empty command will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that updating a non-existent sale throws a KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When updating sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {command.Id} not found");
    }

    /// <summary>
    /// Tests that updating a cancelled sale throws an InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given cancelled sale When updating sale Then throws InvalidOperationException")]
    public async Task Handle_CancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = UpdateSaleHandlerTestData.GenerateExistingSale(command.Id);
        existingSale.IsCancelled = true;

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Cannot update a cancelled sale");
    }

    /// <summary>
    /// Tests that updating a sale with a duplicate sale number throws an InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given duplicate sale number When updating sale Then throws InvalidOperationException")]
    public async Task Handle_DuplicateSaleNumber_ThrowsInvalidOperationException()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = UpdateSaleHandlerTestData.GenerateExistingSale(command.Id);
        existingSale.SaleNumber = "OLD-NUMBER"; // Different from command.SaleNumber

        var conflictingSale = new Sale
        {
            Id = Guid.NewGuid(), // Different ID
            SaleNumber = command.SaleNumber
        };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.GetBySaleNumberAsync(command.SaleNumber, Arg.Any<CancellationToken>())
            .Returns(conflictingSale);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Sale with number {command.SaleNumber} already exists");
    }

    /// <summary>
    /// Tests that updating a sale correctly updates its properties.
    /// </summary>
    [Fact(DisplayName = "Given valid update data When updating sale Then updates sale properties")]
    public async Task Handle_ValidRequest_UpdatesSaleProperties()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = UpdateSaleHandlerTestData.GenerateExistingSale(command.Id);

        Sale capturedSale = null;
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.RemoveItemsAsync(existingSale, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                capturedSale = callInfo.Arg<Sale>();
                return capturedSale;
            });
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo => new UpdateSaleResult { Id = callInfo.Arg<Sale>().Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        capturedSale.SaleNumber.Should().Be(command.SaleNumber);
        capturedSale.SaleDate.Should().Be(command.SaleDate);
        capturedSale.Customer.Should().Be(command.Customer);
        capturedSale.Branch.Should().Be(command.Branch);
    }

    /// <summary>
    /// Tests that updating a sale correctly updates existing items.
    /// </summary>
    [Fact(DisplayName = "Given existing items When updating sale Then updates existing items")]
    public async Task Handle_ExistingItems_UpdatesExistingItems()
    {
        // Given
        var existingItemId = Guid.NewGuid();
        var command = UpdateSaleHandlerTestData.GenerateCommandWithExistingItem(existingItemId);
        var existingSale = UpdateSaleHandlerTestData.GenerateSaleWithExistingItem(command.Id, existingItemId);

        Sale capturedSale = null;
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.RemoveItemsAsync(existingSale, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                capturedSale = callInfo.Arg<Sale>();
                return capturedSale;
            });
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo => new UpdateSaleResult { Id = callInfo.Arg<Sale>().Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        capturedSale.Items.Should().ContainSingle(i => i.Id == existingItemId);
        var updatedItem = capturedSale.Items.First(i => i.Id == existingItemId);
        updatedItem.Product.Should().Be(command.Items.First().Product);
        updatedItem.Quantity.Should().Be(command.Items.First().Quantity);
        updatedItem.UnitPrice.Should().Be(command.Items.First().UnitPrice);
    }

    /// <summary>
    /// Tests that updating a sale correctly adds new items.
    /// </summary>
    [Fact(DisplayName = "Given new items When updating sale Then adds new items")]
    public async Task Handle_NewItems_AddsNewItems()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateCommandWithNewItem();
        var existingSale = UpdateSaleHandlerTestData.GenerateExistingSale(command.Id);
        var initialItemCount = existingSale.Items.Count;

        foreach (var existingItem in existingSale.Items)
        {
            command.Items.Add(new UpdateSaleItemCommand
            {
                Id = existingItem.Id,
                Product = existingItem.Product,
                Quantity = existingItem.Quantity,
                UnitPrice = existingItem.UnitPrice
            });
        }

        Sale capturedSale = null;
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.RemoveItemsAsync(existingSale, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                capturedSale = callInfo.Arg<Sale>();
                return capturedSale;
            });
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo => new UpdateSaleResult { Id = callInfo.Arg<Sale>().Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        capturedSale.Items.Should().HaveCount(initialItemCount + 1);
        capturedSale.Items.Should().Contain(i => i.Product == "New Product");
    }

    /// <summary>
    /// Tests that updating a sale correctly removes items that are not in the update request.
    /// </summary>
    [Fact(DisplayName = "Given removed items When updating sale Then removes items")]
    public async Task Handle_RemovedItems_RemovesItems()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = UpdateSaleHandlerTestData.GenerateSaleWithExtraItem(command.Id);
        var extraItemId = existingSale.Items.Last().Id;

        Sale capturedSale = null;
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.RemoveItemsAsync(existingSale, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                capturedSale = callInfo.Arg<Sale>();
                return capturedSale;
            });
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo => new UpdateSaleResult { Id = callInfo.Arg<Sale>().Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        capturedSale.Items.Should().NotContain(i => i.Id == extraItemId);
    }

    /// <summary>
    /// Tests that updating a sale correctly recalculates the total amount.
    /// </summary>
    [Fact(DisplayName = "Given updated items When updating sale Then recalculates total amount")]
    public async Task Handle_UpdatedItems_RecalculatesTotalAmount()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();
        var existingSale = UpdateSaleHandlerTestData.GenerateExistingSale(command.Id);
        var originalTotalAmount = existingSale.TotalAmount;

        Sale capturedSale = null;
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(existingSale);
        _saleRepository.RemoveItemsAsync(existingSale, Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo =>
            {
                capturedSale = callInfo.Arg<Sale>();
                return capturedSale;
            });
        _mapper.Map<UpdateSaleResult>(Arg.Any<Sale>())
            .Returns(callInfo => new UpdateSaleResult { Id = callInfo.Arg<Sale>().Id });

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        capturedSale.Should().NotBeNull();
        // The total amount should be recalculated based on the updated items
        capturedSale.TotalAmount.Should().NotBe(originalTotalAmount);
        capturedSale.TotalAmount.Should().Be(capturedSale.Items.Sum(i => i.TotalAmount));
    }
}