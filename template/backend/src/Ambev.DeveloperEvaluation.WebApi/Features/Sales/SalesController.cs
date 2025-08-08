using MediatR;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
//using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
//using Ambev.DeveloperEvaluation.Application.Sales.GetSalesByCustomer;
//using Ambev.DeveloperEvaluation.Application.Sales.GetSalesByDateRange;
//using Ambev.DeveloperEvaluation.Application.Sales.GetSalesByBranch;
//using Ambev.DeveloperEvaluation.Application.Sales.GetSalesByCreatedBy;
//using Ambev.DeveloperEvaluation.Application.Sales.GetAllSalesPaginated;
//using Ambev.DeveloperEvaluation.Application.Sales.AddSaleItem;
//using Ambev.DeveloperEvaluation.Application.Sales.RemoveSaleItem;
//using Ambev.DeveloperEvaluation.Application.Sales.UpdateSaleItemQuantity;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSale;
//using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;
//using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSalesByCustomer;
//using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSalesByDateRange;
//using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSalesByBranch;
//using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSalesByCreatedBy;
//using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetAllSalesPaginated;
//using Ambev.DeveloperEvaluation.WebApi.Features.Sales.AddSaleItem;
//using Ambev.DeveloperEvaluation.WebApi.Features.Sales.RemoveSaleItem;
//using Ambev.DeveloperEvaluation.WebApi.Features.Sales.UpdateSaleItemQuantity;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales;

/// <summary>
/// Controller for managing sales operations
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SalesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of SalesController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public SalesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new sale
    /// </summary>
    /// <param name="request">The sale creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateSaleResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateSale([FromBody] CreateSaleRequest request, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateSaleResponse>
        {
            Success = true,
            Message = "Sale created successfully",
            Data = _mapper.Map<CreateSaleResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves a sale by its ID
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale details if found</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSale([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var request = new GetSaleRequest { Id = id };
        var validator = new GetSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetSaleCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(_mapper.Map<GetSaleResponse>(response));
    }

    /// <summary>
    /// Updates an existing sale
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="request">The sale update request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale details</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateSaleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateSale([FromRoute] Guid id, [FromBody] UpdateSaleRequest request, CancellationToken cancellationToken)
    {
        if (id != request.Id)
            return BadRequest("ID in route must match ID in request body");

        var validator = new UpdateSaleRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateSaleCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(_mapper.Map<UpdateSaleResponse>(response));
    }

    ///// <summary>
    ///// Cancels a sale by its ID
    ///// </summary>
    ///// <param name="id">The unique identifier of the sale to cancel</param>
    ///// <param name="cancellationToken">Cancellation token</param>
    ///// <returns>Success response if the sale was cancelled</returns>
    //[HttpPatch("{id}/cancel")]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> CancelSale([FromRoute] Guid id, CancellationToken cancellationToken)
    //{
    //    var request = new CancelSaleRequest { Id = id };
    //    var validator = new CancelSaleRequestValidator();
    //    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    //    if (!validationResult.IsValid)
    //        return BadRequest(validationResult.Errors);

    //    var command = _mapper.Map<CancelSaleCommand>(request.Id);
    //    await _mediator.Send(command, cancellationToken);

    //    return Ok(new ApiResponse
    //    {
    //        Success = true,
    //        Message = "Sale cancelled successfully"
    //    });
    //}

    ///// <summary>
    ///// Retrieves all sales for a specific customer
    ///// </summary>
    ///// <param name="customer">The customer name</param>
    ///// <param name="cancellationToken">Cancellation token</param>
    ///// <returns>List of sales for the customer</returns>
    //[HttpGet("by-customer/{customer}")]
    //[ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<GetSaleResponse>>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> GetSalesByCustomer([FromRoute] string customer, CancellationToken cancellationToken)
    //{
    //    var request = new GetSalesByCustomerRequest { Customer = customer };
    //    var validator = new GetSalesByCustomerRequestValidator();
    //    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    //    if (!validationResult.IsValid)
    //        return BadRequest(validationResult.Errors);

    //    var command = _mapper.Map<GetSalesByCustomerCommand>(request);
    //    var response = await _mediator.Send(command, cancellationToken);

    //    return Ok(new ApiResponseWithData<IEnumerable<GetSaleResponse>>
    //    {
    //        Success = true,
    //        Message = "Sales retrieved successfully",
    //        Data = _mapper.Map<IEnumerable<GetSaleResponse>>(response)
    //    });
    //}

    ///// <summary>
    ///// Retrieves all sales within a date range
    ///// </summary>
    ///// <param name="request">The date range request</param>
    ///// <param name="cancellationToken">Cancellation token</param>
    ///// <returns>List of sales within the date range</returns>
    //[HttpGet("by-date-range")]
    //[ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<GetSaleResponse>>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> GetSalesByDateRange([FromQuery] GetSalesByDateRangeRequest request, CancellationToken cancellationToken)
    //{
    //    var validator = new GetSalesByDateRangeRequestValidator();
    //    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    //    if (!validationResult.IsValid)
    //        return BadRequest(validationResult.Errors);

    //    var command = _mapper.Map<GetSalesByDateRangeCommand>(request);
    //    var response = await _mediator.Send(command, cancellationToken);

    //    return Ok(new ApiResponseWithData<IEnumerable<GetSaleResponse>>
    //    {
    //        Success = true,
    //        Message = "Sales retrieved successfully",
    //        Data = _mapper.Map<IEnumerable<GetSaleResponse>>(response)
    //    });
    //}

    ///// <summary>
    ///// Retrieves all sales from a specific branch
    ///// </summary>
    ///// <param name="branch">The branch name</param>
    ///// <param name="cancellationToken">Cancellation token</param>
    ///// <returns>List of sales from the branch</returns>
    //[HttpGet("by-branch/{branch}")]
    //[ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<GetSaleResponse>>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> GetSalesByBranch([FromRoute] string branch, CancellationToken cancellationToken)
    //{
    //    var request = new GetSalesByBranchRequest { Branch = branch };
    //    var validator = new GetSalesByBranchRequestValidator();
    //    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    //    if (!validationResult.IsValid)
    //        return BadRequest(validationResult.Errors);

    //    var command = _mapper.Map<GetSalesByBranchCommand>(request);
    //    var response = await _mediator.Send(command, cancellationToken);

    //    return Ok(new ApiResponseWithData<IEnumerable<GetSaleResponse>>
    //    {
    //        Success = true,
    //        Message = "Sales retrieved successfully",
    //        Data = _mapper.Map<IEnumerable<GetSaleResponse>>(response)
    //    });
    //}

    ///// <summary>
    ///// Retrieves all sales created by a specific user
    ///// </summary>
    ///// <param name="userId">The ID of the user who created the sales</param>
    ///// <param name="cancellationToken">Cancellation token</param>
    ///// <returns>List of sales created by the user</returns>
    //[HttpGet("by-creator/{userId}")]
    //[ProducesResponseType(typeof(ApiResponseWithData<IEnumerable<GetSaleResponse>>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> GetSalesByCreatedBy([FromRoute] Guid userId, CancellationToken cancellationToken)
    //{
    //    var request = new GetSalesByCreatedByRequest { UserId = userId };
    //    var validator = new GetSalesByCreatedByRequestValidator();
    //    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    //    if (!validationResult.IsValid)
    //        return BadRequest(validationResult.Errors);

    //    var command = _mapper.Map<GetSalesByCreatedByCommand>(request);
    //    var response = await _mediator.Send(command, cancellationToken);

    //    return Ok(new ApiResponseWithData<IEnumerable<GetSaleResponse>>
    //    {
    //        Success = true,
    //        Message = "Sales retrieved successfully",
    //        Data = _mapper.Map<IEnumerable<GetSaleResponse>>(response)
    //    });
    //}

    ///// <summary>
    ///// Retrieves all sales with pagination
    ///// </summary>
    ///// <param name="request">The pagination request</param>
    ///// <param name="cancellationToken">Cancellation token</param>
    ///// <returns>Paginated list of sales</returns>
    //[HttpGet]
    //[ProducesResponseType(typeof(ApiResponseWithPagination<IEnumerable<GetSaleResponse>>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //public async Task<IActionResult> GetAllSalesPaginated([FromQuery] GetAllSalesPaginatedRequest request, CancellationToken cancellationToken)
    //{
    //    var validator = new GetAllSalesPaginatedRequestValidator();
    //    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    //    if (!validationResult.IsValid)
    //        return BadRequest(validationResult.Errors);

    //    var command = _mapper.Map<GetAllSalesPaginatedCommand>(request);
    //    var response = await _mediator.Send(command, cancellationToken);

    //    return Ok(new ApiResponseWithPagination<IEnumerable<GetSaleResponse>>
    //    {
    //        Success = true,
    //        Message = "Sales retrieved successfully",
    //        Data = _mapper.Map<IEnumerable<GetSaleResponse>>(response.Sales),
    //        Pagination = new PaginationInfo
    //        {
    //            CurrentPage = request.PageNumber,
    //            PageSize = request.PageSize,
    //            TotalCount = response.TotalCount,
    //            TotalPages = (int)Math.Ceiling(response.TotalCount / (double)request.PageSize)
    //        }
    //    });
    //}

    ///// <summary>
    ///// Adds a new item to an existing sale
    ///// </summary>
    ///// <param name="saleId">The ID of the sale</param>
    ///// <param name="request">The item details</param>
    ///// <param name="cancellationToken">Cancellation token</param>
    ///// <returns>The updated sale with the new item</returns>
    //[HttpPost("{saleId}/items")]
    //[ProducesResponseType(typeof(ApiResponseWithData<GetSaleResponse>), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> AddSaleItem([FromRoute] Guid saleId, [FromBody] AddSaleItemRequest request, CancellationToken cancellationToken)
    //{
    //    if (saleId != request.SaleId)
    //        return BadRequest("Sale ID in route must match Sale ID in request body");

    //    var validator = new AddSaleItemRequestValidator();
    //    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    //    if (!validationResult.IsValid)
    //        return BadRequest(validationResult.Errors);

    //    var command = _mapper.Map<AddSaleItemCommand>(request);
    //    var response = await _mediator.Send(command, cancellationToken);

    //    return Ok(new ApiResponseWithData<GetSaleResponse>
    //    {
    //        Success = true,
    //        Message = "Item added successfully",
    //        Data = _mapper.Map<GetSaleResponse>(response)
    //    });
    //}

    ///// <summary>
    ///// Removes an item from an existing sale
    ///// </summary>
    ///// <param name="saleId">The ID of the sale</param>
    ///// <param name="itemId">The ID of the item to remove</param>
    ///// <param name="cancellationToken">Cancellation token</param>
    ///// <returns>Success response if the item was removed</returns>
    //[HttpDelete("{saleId}/items/{itemId}")]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> RemoveSaleItem([FromRoute] Guid saleId, [FromRoute] Guid itemId, CancellationToken cancellationToken)
    //{
    //    var request = new RemoveSaleItemRequest { SaleId = saleId, ItemId = itemId };
    //    var validator = new RemoveSaleItemRequestValidator();
    //    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    //    if (!validationResult.IsValid)
    //        return BadRequest(validationResult.Errors);

    //    var command = _mapper.Map<RemoveSaleItemCommand>(request);
    //    var result = await _mediator.Send(command, cancellationToken);

    //    if (!result)
    //        return NotFound(new ApiResponse { Success = false, Message = "Item not found" });

    //    return Ok(new ApiResponse
    //    {
    //        Success = true,
    //        Message = "Item removed successfully"
    //    });
    //}

    ///// <summary>
    ///// Updates the quantity of an item in an existing sale
    ///// </summary>
    ///// <param name="saleId">The ID of the sale</param>
    ///// <param name="itemId">The ID of the item</param>
    ///// <param name="request">The update request</param>
    ///// <param name="cancellationToken">Cancellation token</param>
    ///// <returns>Success response if the item was updated</returns>
    //[HttpPatch("{saleId}/items/{itemId}")]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    //public async Task<IActionResult> UpdateSaleItemQuantity(
    //    [FromRoute] Guid saleId,
    //    [FromRoute] Guid itemId,
    //    [FromBody] UpdateSaleItemQuantityRequest request,
    //    CancellationToken cancellationToken)
    //{
    //    if (saleId != request.SaleId || itemId != request.ItemId)
    //        return BadRequest("IDs in route must match IDs in request body");

    //    var validator = new UpdateSaleItemQuantityRequestValidator();
    //    var validationResult = await validator.ValidateAsync(request, cancellationToken);

    //    if (!validationResult.IsValid)
    //        return BadRequest(validationResult.Errors);

    //    var command = _mapper.Map<UpdateSaleItemQuantityCommand>(request);
    //    var result = await _mediator.Send(command, cancellationToken);

    //    if (!result)
    //        return NotFound(new ApiResponse { Success = false, Message = "Item not found" });

    //    return Ok(new ApiResponse
    //    {
    //        Success = true,
    //        Message = "Item quantity updated successfully"
    //    });
    //}
}