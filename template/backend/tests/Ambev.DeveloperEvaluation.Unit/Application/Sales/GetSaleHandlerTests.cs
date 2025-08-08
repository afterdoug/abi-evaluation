using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application;

/// <summary>
/// Contains unit tests for the <see cref="GetSaleHandler"/> class.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    /// <summary>
    /// Initializes a new instance of the <see cref="GetSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    /// <summary>
    /// Tests that a valid sale retrieval request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When retrieving sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        var (sale, result) = GetSaleHandlerTestData.GenerateValidSaleAndResult(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getSaleResult.Should().NotBeNull();
        getSaleResult.Id.Should().Be(sale.Id);
        getSaleResult.SaleNumber.Should().Be(sale.SaleNumber);
        getSaleResult.Customer.Should().Be(sale.Customer);
        getSaleResult.Branch.Should().Be(sale.Branch);
        getSaleResult.TotalAmount.Should().Be(sale.TotalAmount);
        getSaleResult.Items.Should().HaveCount(sale.Items.Count);

        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
        _mapper.Received(1).Map<GetSaleResult>(sale);
    }

    /// <summary>
    /// Tests that an invalid sale retrieval request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid sale ID When retrieving sale Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new GetSaleCommand(Guid.Empty); // Empty GUID will fail validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that retrieving a non-existent sale throws a KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When retrieving sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");
    }

    /// <summary>
    /// Tests that the repository is called with the correct ID.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct ID")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        var (sale, result) = GetSaleHandlerTestData.GenerateValidSaleAndResult(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(
            Arg.Is<Guid>(id => id == saleId),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the mapper is called with the correct sale entity.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then maps sale entity to result")]
    public async Task Handle_ValidRequest_MapsSaleToResult()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        var (sale, result) = GetSaleHandlerTestData.GenerateValidSaleAndResult(saleId);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<GetSaleResult>(
            Arg.Is<Sale>(s =>
                s.Id == saleId &&
                s.SaleNumber == sale.SaleNumber &&
                s.Customer == sale.Customer));
    }

    /// <summary>
    /// Tests that the result includes all sale items.
    /// </summary>
    [Fact(DisplayName = "Given sale with items When retrieving sale Then result includes all items")]
    public async Task Handle_SaleWithItems_ResultIncludesAllItems()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        var (sale, result) = GetSaleHandlerTestData.GenerateSaleWithMultipleItems(saleId, 3);

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getSaleResult.Should().NotBeNull();
        getSaleResult.Items.Should().HaveCount(3);
        for (int i = 0; i < 3; i++)
        {
            getSaleResult.Items[i].Product.Should().Be($"Product {i + 1}");
        }
    }

    /// <summary>
    /// Tests that the result includes creator information.
    /// </summary>
    [Fact(DisplayName = "Given sale with creator When retrieving sale Then result includes creator info")]
    public async Task Handle_SaleWithCreator_ResultIncludesCreatorInfo()
    {
        // Given
        var saleId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var command = new GetSaleCommand(saleId);

        var (sale, result) = GetSaleHandlerTestData.GenerateSaleWithCreator(saleId, userId, "TestCreator");

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var getSaleResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        getSaleResult.Should().NotBeNull();
        getSaleResult.CreatedBy.Should().NotBeNull();
        getSaleResult.CreatedBy.Id.Should().Be(userId);
        getSaleResult.CreatedBy.Username.Should().Be("TestCreator");
    }
}
