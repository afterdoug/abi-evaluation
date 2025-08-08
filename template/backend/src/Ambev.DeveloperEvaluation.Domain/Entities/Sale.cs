using Ambev.DeveloperEvaluation.Common.Validation;
using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Microsoft.Extensions.Logging.Abstractions;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents a sales transaction in the system.
/// This entity follows domain-driven design principles and includes business rules validation.
/// </summary>
public class Sale : BaseEntity
{
    /// <summary>
    /// Gets the unique sale number/identifier.
    /// Used for reference and tracking purposes.
    /// </summary>
    public string SaleNumber { get; set; } = string.Empty;

    /// <summary>
    /// Gets the date and time when the sale was made.
    /// </summary>
    public DateTime SaleDate { get; set; }

    /// <summary>
    /// Gets the customer associated with this sale.
    /// </summary>
    public string Customer { get; set; } = string.Empty;

    /// <summary>
    /// Gets the total amount of the sale.
    /// This is the sum of all item totals minus any overall discounts.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets the branch/store where the sale was made.
    /// </summary>
    public string Branch { get; set; } = string.Empty;

    /// <summary>
    /// Gets the collection of items included in this sale.
    /// </summary>
    public ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

    /// <summary>
    /// Gets or sets whether the sale has been cancelled.
    /// </summary>
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Gets the date and time when the sale was created in the system.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets the user who created this sale.
    /// </summary>
    public Guid CreatedById { get; set; }
    public User CreatedBy { get; set; }

    /// <summary>
    /// Initializes a new instance of the Sale class.
    /// </summary>
    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
        SaleDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Performs validation of the sale entity using the SaleValidator rules.
    /// </summary>
    /// <returns>
    /// A <see cref="ValidationResultDetail"/> containing:
    /// - IsValid: Indicates whether all validation rules passed
    /// - Errors: Collection of validation errors if any rules failed
    /// </returns>
    /// <remarks>
    /// <listheader>The validation includes checking:</listheader>
    /// <list type="bullet">Sale number format and uniqueness</list>
    /// <list type="bullet">Sale date validity</list>
    /// <list type="bullet">Customer information</list>
    /// <list type="bullet">Branch information</list>
    /// <list type="bullet">Items collection (must have at least one item)</list>
    /// <list type="bullet">Total amount calculation accuracy</list>
    /// </remarks>
    public ValidationResultDetail Validate()
    {
        var validator = new SaleValidator();
        var result = validator.Validate(this);
        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }

    /// <summary>
    /// Calculates the total amount of the sale based on all items.
    /// </summary>
    public void CalculateTotalAmount()
    {
        TotalAmount = Items.Sum(item => item.TotalAmount);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Adds a new item to the sale and applies appropriate quantity-based discounts.
    /// </summary>
    /// <param name="product">The product being sold</param>
    /// <param name="quantity">The quantity of the product</param>
    /// <param name="unitPrice">The unit price of the product</param>
    /// <returns>The newly created sale item</returns>
    /// <exception cref="BusinessRuleException">Thrown when business rules are violated</exception>
    public SaleItem AddItem(string product, int quantity, decimal unitPrice)
    {
        // Validate maximum quantity
        if (quantity > 20)
        {
            throw new BusinessRuleException("Cannot sell more than 20 identical items.");
        }

        var item = new SaleItem
        {
            Product = product,
            Quantity = quantity,
            UnitPrice = unitPrice,
            Discount = 0
        };

        // Apply business rules for discounts
        item.ApplyQuantityBasedDiscount();

        Items.Add(item);
        CalculateTotalAmount();
        UpdatedAt = DateTime.UtcNow;

        return item;
    }

    /// <summary>
    /// Updates an existing item in the sale.
    /// </summary>
    /// <param name="itemId">The ID of the item to update</param>
    /// <param name="quantity">The new quantity</param>
    /// <returns>True if the item was found and updated, false otherwise</returns>
    /// <exception cref="BusinessRuleException">Thrown when business rules are violated</exception>
    public bool UpdateItemQuantity(Guid itemId, int quantity)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            return false;

        // Validate maximum quantity
        if (quantity > 20)
        {
            throw new BusinessRuleException("Cannot sell more than 20 identical items.");
        }

        item.Quantity = quantity;
        item.ApplyQuantityBasedDiscount();
        CalculateTotalAmount();
        UpdatedAt = DateTime.UtcNow;

        return true;
    }

    /// <summary>
    /// Removes an item from the sale.
    /// </summary>
    /// <param name="itemId">The ID of the item to remove</param>
    /// <returns>True if the item was found and removed, false otherwise</returns>
    public bool RemoveItem(Guid itemId)
    {
        var item = Items.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            return false;

        Items.Remove(item);
        CalculateTotalAmount();
        UpdatedAt = DateTime.UtcNow;
        return true;
    }

    /// <summary>
    /// Cancels the sale.
    /// </summary>
    /// <param name="cancellationReason">The reason for cancellation</param>
    public void Cancel(string cancellationReason = "")
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }
}
