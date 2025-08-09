using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

/// <summary>
/// Contains unit tests for the <see cref="CancelSaleHandler"/> class.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly CancelSaleHandler _handler;
    private readonly IMediator _mediator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CancelSaleHandlerTests"/> class.
    /// Sets up the test dependencies and creates fake data generators.
    /// </summary>
    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _mediator = Substitute.For<IMediator>();
        _handler = new CancelSaleHandler(_saleRepository, _mapper, _mediator);
    }

    /// <summary>
    /// Tests that a valid sale cancellation request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid sale ID When cancelling sale Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = false
        };

        var result = new CancelSaleResult
        {
            Id = saleId,
            IsCancelled = true
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<CancelSaleResult>(sale).Returns(result);

        // When
        var cancelSaleResult = await _handler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        // Then
        cancelSaleResult.Should().NotBeNull();
        cancelSaleResult.Id.Should().Be(saleId);
        cancelSaleResult.IsCancelled.Should().BeTrue();
        await _saleRepository.Received(1).CancelAsync(saleId, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid sale cancellation request throws a KeyNotFoundException.
    /// </summary>
    [Fact(DisplayName = "Given non-existent sale ID When cancelling sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistentSale_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns((Sale)null);

        // When
        var act = () => _handler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"Sale with ID {saleId} not found");
    }

    /// <summary>
    /// Tests that cancelling an already cancelled sale throws an InvalidOperationException.
    /// </summary>
    [Fact(DisplayName = "Given already cancelled sale When cancelling sale Then throws InvalidOperationException")]
    public async Task Handle_AlreadyCancelledSale_ThrowsInvalidOperationException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            IsCancelled = true
        };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        // When
        var act = () => _handler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Sale is already cancelled");
    }

    /// <summary>
    /// Tests that the repository is called with the correct ID.
    /// </summary>
    [Fact(DisplayName = "Given valid command When handling Then calls repository with correct ID")]
    public async Task Handle_ValidRequest_CallsRepositoryWithCorrectId()
    {
        // Given
        var saleId = Guid.NewGuid();
        var sale = new Sale { Id = saleId, IsCancelled = false };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<CancelSaleResult>(sale).Returns(new CancelSaleResult { Id = saleId, IsCancelled = true });

        // When
        await _handler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

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
        var sale = new Sale { Id = saleId, IsCancelled = false };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>())
            .Returns(sale);

        _mapper.Map<CancelSaleResult>(sale).Returns(new CancelSaleResult { Id = saleId, IsCancelled = true });

        // When
        await _handler.Handle(new CancelSaleCommand(saleId), CancellationToken.None);

        // Then
        _mapper.Received(1).Map<CancelSaleResult>(
            Arg.Is<Sale>(s => s.Id == saleId && !s.IsCancelled));
    }
}