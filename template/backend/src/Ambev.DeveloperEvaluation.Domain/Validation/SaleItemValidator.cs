using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Domain.Validation;

/// <summary>
/// Validator for the SaleItem entity.
/// Defines validation rules for sale item data.
/// </summary>
public class SaleItemValidator : AbstractValidator<SaleItem>
{
    public SaleItemValidator()
    {
        RuleFor(i => i.Product)
            .NotEmpty().WithMessage("Product is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(i => i.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 identical items.");

        RuleFor(i => i.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than zero.");

        RuleFor(i => i.Discount)
            .GreaterThanOrEqualTo(0).WithMessage("Discount cannot be negative.")
            .LessThanOrEqualTo(i => i.Quantity * i.UnitPrice).WithMessage("Discount cannot be greater than the total item value.");

        // Validate discount rules
        RuleFor(i => i.Discount)
            .Must((item, discount) => {
                if (item.Quantity < 4 && discount > 0)
                    return false;
                return true;
            })
            .WithMessage("Purchases below 4 items cannot have a discount.");

        // Validate discount percentage based on quantity
        RuleFor(i => i.DiscountPercentage)
            .Must((item, discountPercentage) => {
                if (item.Quantity >= 10 && item.Quantity <= 20)
                    return Math.Abs(discountPercentage - 0.20m) < 0.001m;
                else if (item.Quantity >= 4 && item.Quantity < 10)
                    return Math.Abs(discountPercentage - 0.10m) < 0.001m;
                else
                    return discountPercentage == 0;
            })
            .WithMessage("Incorrect discount percentage applied based on quantity rules.");

        RuleFor(i => i.TotalAmount)
            .Equal(i => (i.Quantity * i.UnitPrice) - i.Discount)
            .WithMessage("Total amount calculation is incorrect.");
    }
}