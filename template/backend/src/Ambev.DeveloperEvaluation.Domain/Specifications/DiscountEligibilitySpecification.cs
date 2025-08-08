using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

/// <summary>
/// Specification that determines if a sale item is eligible for any discount.
/// Items with quantities below 4 are not eligible for discounts.
/// </summary>
public class DiscountEligibilitySpecification : ISpecification<SaleItem>
{
    public bool IsSatisfiedBy(SaleItem item)
    {
        return item.Quantity >= 4;
    }
}