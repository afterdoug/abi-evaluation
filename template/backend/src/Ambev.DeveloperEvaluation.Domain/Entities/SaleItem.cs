using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

/// <summary>
/// Represents an individual item within a sale.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets the product associated with this sale item.
    /// </summary>
    public string Product { get; set; } = string.Empty;

    /// <summary>
    /// Gets the quantity of the product.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets the unit price of the product.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets any discount applied to this item.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets the total amount for this item (Quantity * UnitPrice - Discount).
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets the sale this item belongs to.
    /// </summary>
    public Guid SaleId { get; set; }
    public Sale Sale { get; set; } = null!;

    /// <summary>
    /// Gets the discount percentage applied to this item based on quantity rules.
    /// </summary>
    public decimal DiscountPercentage { get; private set; }

    /// <summary>
    /// Applies the appropriate discount based on business rules:
    /// - 10% discount for 4+ identical items
    /// - 20% discount for 10-20 identical items
    /// - No discount for less than 4 items
    /// - Maximum limit of 20 items per product
    /// </summary>
    /// <returns>True if the discount was applied successfully, false if rules were violated</returns>
    public bool ApplyQuantityBasedDiscount()
    {
        // Check if quantity exceeds maximum allowed
        var maxQuantitySpec = new MaximumQuantitySpecification();
        if (!maxQuantitySpec.IsSatisfiedBy(this))
        {
            throw new BusinessRuleException("Cannot sell more than 20 identical items.");
        }

        // Check discount eligibility
        var discountEligibilitySpec = new DiscountEligibilitySpecification();
        if (!discountEligibilitySpec.IsSatisfiedBy(this) && Discount > 0)
        {
            throw new BusinessRuleException("Purchases below 4 items cannot have a discount.");
        }

        // Apply 20% discount for 10-20 items
        var twentyPercentSpec = new TwentyPercentDiscountSpecification();
        if (twentyPercentSpec.IsSatisfiedBy(this))
        {
            DiscountPercentage = 0.20m;
            Discount = UnitPrice * Quantity * DiscountPercentage;
            CalculateTotalAmount();
            return true;
        }

        // Apply 10% discount for 4-9 items
        var tenPercentSpec = new TenPercentDiscountSpecification();
        if (tenPercentSpec.IsSatisfiedBy(this))
        {
            DiscountPercentage = 0.10m;
            Discount = UnitPrice * Quantity * DiscountPercentage;
            CalculateTotalAmount();
            return true;
        }

        // No discount applies
        DiscountPercentage = 0;
        Discount = 0;
        CalculateTotalAmount();
        return true;
    }

    /// <summary>
    /// Calculates the total amount for this item based on quantity, unit price, and discount.
    /// </summary>
    public void CalculateTotalAmount()
    {
        TotalAmount = (Quantity * UnitPrice) - Discount;
    }
}