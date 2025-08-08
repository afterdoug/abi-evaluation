using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

/// <summary>
/// Specification that determines if a sale item is eligible for a 20% discount.
/// Applies to purchases of 10 to 20 identical items.
/// </summary>
public class TwentyPercentDiscountSpecification : ISpecification<SaleItem>
{
    public bool IsSatisfiedBy(SaleItem item)
    {
        return item.Quantity >= 10 && item.Quantity <= 20;
    }
}
