using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

/// <summary>
/// Specification that determines if a sale item is eligible for a 10% discount.
/// Applies to purchases of 4 or more identical items, but less than 10.
/// </summary>
public class TenPercentDiscountSpecification : ISpecification<SaleItem>
{
    public bool IsSatisfiedBy(SaleItem item)
    {
        return item.Quantity >= 4 && item.Quantity < 10;
    }
}
