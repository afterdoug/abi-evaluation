using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Domain.Specifications.Sales;

/// <summary>
/// Specification that determines if a sale item exceeds the maximum allowed quantity.
/// The maximum allowed quantity is 20 identical items.
/// </summary>
public class MaximumQuantitySpecification : ISpecification<SaleItem>
{
    public bool IsSatisfiedBy(SaleItem item)
    {
        return item.Quantity <= 20;
    }
}
