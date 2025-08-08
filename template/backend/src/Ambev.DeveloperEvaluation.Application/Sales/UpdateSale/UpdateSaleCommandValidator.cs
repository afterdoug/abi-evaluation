using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

/// <summary>
/// Validator for UpdateSaleCommand
/// </summary>
public class UpdateSaleCommandValidator : AbstractValidator<UpdateSaleCommand>
{
    /// <summary>
    /// Initializes validation rules for UpdateSaleCommand
    /// </summary>
    public UpdateSaleCommandValidator()
    {
        RuleFor(sale => sale.Id)
            .NotEmpty().WithMessage("Sale ID is required.");

        RuleFor(sale => sale.SaleNumber)
            .NotEmpty().WithMessage("Sale number is required.")
            .MaximumLength(50).WithMessage("Sale number cannot exceed 50 characters.");

        RuleFor(sale => sale.SaleDate)
            .NotEmpty().WithMessage("Sale date is required.")
            .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Sale date cannot be in the future.");

        RuleFor(sale => sale.Customer)
            .NotEmpty().WithMessage("Customer information is required.")
            .MaximumLength(100).WithMessage("Customer name cannot exceed 100 characters.");

        RuleFor(sale => sale.Branch)
            .NotEmpty().WithMessage("Branch information is required.")
            .MaximumLength(100).WithMessage("Branch name cannot exceed 100 characters.");

        RuleFor(sale => sale.Items)
            .NotEmpty().WithMessage("A sale must have at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new UpdateSaleItemCommandValidator());
    }
}

/// <summary>
/// Validator for UpdateSaleItemCommand
/// </summary>
public class UpdateSaleItemCommandValidator : AbstractValidator<UpdateSaleItemCommand>
{
    /// <summary>
    /// Initializes validation rules for UpdateSaleItemCommand
    /// </summary>
    public UpdateSaleItemCommandValidator()
    {
        RuleFor(item => item.Product)
            .NotEmpty().WithMessage("Product is required.")
            .MaximumLength(100).WithMessage("Product name cannot exceed 100 characters.");

        RuleFor(item => item.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero.")
            .LessThanOrEqualTo(20).WithMessage("Cannot sell more than 20 identical items.");

        RuleFor(item => item.UnitPrice)
            .GreaterThan(0).WithMessage("Unit price must be greater than zero.");
    }
}