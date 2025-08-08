using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Repositories;

/// <summary>
/// Repository interface for Sale entity operations
/// </summary>
public interface ISaleRepository
{
    /// <summary>
    /// Creates a new sale in the database
    /// </summary>
    /// <param name="sale">The sale to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale</returns>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, null otherwise</returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its sale number
    /// </summary>
    /// <param name="saleNumber">The sale number to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, null otherwise</returns>
    Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sales for a specific customer
    /// </summary>
    /// <param name="customer">The customer name to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of sales for the customer</returns>
    Task<IEnumerable<Sale>> GetByCustomerAsync(string customer, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sales created by a specific user
    /// </summary>
    /// <param name="userId">The ID of the user who created the sales</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of sales created by the user</returns>
    Task<IEnumerable<Sale>> GetByCreatedByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sales within a date range
    /// </summary>
    /// <param name="startDate">The start date of the range</param>
    /// <param name="endDate">The end date of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of sales within the date range</returns>
    Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sales from a specific branch
    /// </summary>
    /// <param name="branch">The branch name to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of sales from the branch</returns>
    Task<IEnumerable<Sale>> GetByBranchAsync(string branch, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale in the database
    /// </summary>
    /// <param name="sale">The sale with updated information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale</returns>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a sale in the database
    /// </summary>
    /// <param name="id">The unique identifier of the sale to cancel</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was cancelled, false if not found</returns>
    Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all sales with pagination
    /// </summary>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of sales</returns>
    Task<(IEnumerable<Sale> Sales, int TotalCount)> GetAllPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new item to an existing sale
    /// </summary>
    /// <param name="saleId">The ID of the sale</param>
    /// <param name="product">The product name</param>
    /// <param name="quantity">The quantity</param>
    /// <param name="unitPrice">The unit price</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale with the new item</returns>
    Task<Sale?> AddItemAsync(
        Guid saleId,
        string product,
        int quantity,
        decimal unitPrice,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes an item from an existing sale
    /// </summary>
    /// <param name="saleId">The ID of the sale</param>
    /// <param name="itemId">The ID of the item to remove</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the item was removed, false otherwise</returns>
    Task<bool> RemoveItemAsync(
        Guid saleId,
        Guid itemId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the quantity of an item in an existing sale
    /// </summary>
    /// <param name="saleId">The ID of the sale</param>
    /// <param name="itemId">The ID of the item</param>
    /// <param name="quantity">The new quantity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the item was updated, false otherwise</returns>
    Task<bool> UpdateItemQuantityAsync(
        Guid saleId,
        Guid itemId,
        int quantity,
        CancellationToken cancellationToken = default);
}