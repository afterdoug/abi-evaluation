using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of SaleRepository
    /// </summary>
    /// <param name="context">The database context</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new sale in the database
    /// </summary>
    /// <param name="sale">The sale to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created sale</returns>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Retrieves a sale by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, null otherwise</returns>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves a sale by its sale number
    /// </summary>
    /// <param name="saleNumber">The sale number to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The sale if found, null otherwise</returns>
    public async Task<Sale?> GetBySaleNumberAsync(string saleNumber, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Include(s => s.CreatedBy)
            .FirstOrDefaultAsync(s => s.SaleNumber == saleNumber, cancellationToken);
    }

    /// <summary>
    /// Retrieves all sales for a specific customer
    /// </summary>
    /// <param name="customer">The customer name to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of sales for the customer</returns>
    public async Task<IEnumerable<Sale>> GetByCustomerAsync(string customer, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.Customer == customer)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all sales created by a specific user
    /// </summary>
    /// <param name="userId">The ID of the user who created the sales</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of sales created by the user</returns>
    public async Task<IEnumerable<Sale>> GetByCreatedByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.CreatedById == userId)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all sales within a date range
    /// </summary>
    /// <param name="startDate">The start date of the range</param>
    /// <param name="endDate">The end date of the range</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of sales within the date range</returns>
    public async Task<IEnumerable<Sale>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.SaleDate >= startDate && s.SaleDate <= endDate)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Retrieves all sales from a specific branch
    /// </summary>
    /// <param name="branch">The branch name to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A list of sales from the branch</returns>
    public async Task<IEnumerable<Sale>> GetByBranchAsync(string branch, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .Where(s => s.Branch == branch)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Updates an existing sale in the database
    /// </summary>
    /// <param name="sale">The sale with updated information</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale</returns>
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Entry(sale).State = EntityState.Modified;

        foreach (var item in sale.Items)
        {
            item.SaleId = sale.Id;
            _context.Set<SaleItem>().Add(item);
        }

        await _context.SaveChangesAsync(cancellationToken);

        return sale;
    }

    /// <summary>
    /// Cancels a sale in the database
    /// </summary>
    /// <param name="id">The unique identifier of the sale to cancel</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was cancelled, false if not found</returns>
    public async Task<bool> CancelAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        sale.Cancel();
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    /// <summary>
    /// Retrieves all sales with pagination
    /// </summary>
    /// <param name="pageNumber">The page number (1-based)</param>
    /// <param name="pageSize">The number of items per page</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A paginated list of sales</returns>
    public async Task<(IEnumerable<Sale> Sales, int TotalCount)> GetAllPaginatedAsync(
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await _context.Sales.CountAsync(cancellationToken);

        var sales = await _context.Sales
            .Include(s => s.Items)
            .OrderByDescending(s => s.SaleDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (sales, totalCount);
    }

    /// <summary>
    /// Adds a new item to an existing sale
    /// </summary>
    /// <param name="saleId">The ID of the sale</param>
    /// <param name="product">The product name</param>
    /// <param name="quantity">The quantity</param>
    /// <param name="unitPrice">The unit price</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated sale with the new item</returns>
    public async Task<Sale?> AddItemAsync(
        Guid saleId,
        string product,
        int quantity,
        decimal unitPrice,
        CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(saleId, cancellationToken);
        if (sale == null)
            return null;

        sale.AddItem(product, quantity, unitPrice);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Removes all item from an existing sale
    /// </summary>
    /// <param name="sale">Existing sale</param>
    /// <param name="cancellationToken">Cancellation token</param>
    public async Task RemoveItemsAsync(
        Sale sale,
        CancellationToken cancellationToken = default)
    {
        _context.Entry(sale).State = EntityState.Modified;

        var existingItems = await _context.Set<SaleItem>()
                .Where(i => i.SaleId == sale.Id)
                .ToListAsync(cancellationToken);

        existingItems.ForEach(i => _context.Entry(i).State = EntityState.Deleted);
        sale.Items.Clear();

        await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates the quantity of an item in an existing sale
    /// </summary>
    /// <param name="saleId">The ID of the sale</param>
    /// <param name="itemId">The ID of the item</param>
    /// <param name="quantity">The new quantity</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the item was updated, false otherwise</returns>
    public async Task<bool> UpdateItemQuantityAsync(
        Guid saleId,
        Guid itemId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(saleId, cancellationToken);
        if (sale == null)
            return false;

        var result = sale.UpdateItemQuantity(itemId, quantity);
        if (result)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
        return result;
    }
}