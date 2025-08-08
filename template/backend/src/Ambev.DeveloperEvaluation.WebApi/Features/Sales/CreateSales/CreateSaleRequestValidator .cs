using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;

/// <summary>
/// Validator for CreateSaleRequest that defines validation rules for sale creation.
/// </summary>
public class CreateSaleRequestValidator : AbstractValidator<CreateSaleRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - SaleNumber: Required, maximum length 50 characters
    /// - SaleDate: Required, cannot be in the future
    /// - Customer: Required, maximum length 100 characters
    /// - Branch: Required, maximum length 100 characters
    /// - CreatedById: Required, must be a valid GUID
    /// - Items: Must have at least one item
    /// </remarks>
    public CreateSaleRequestValidator()
    {
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

        RuleFor(sale => sale.CreatedById)
            .NotEmpty().WithMessage("Creator ID is required.");

        RuleFor(sale => sale.Items)
            .NotEmpty().WithMessage("A sale must have at least one item.");

        RuleForEach(sale => sale.Items).SetValidator(new CreateSaleItemRequestValidator());
    }
}

/// <summary>
/// Validator for CreateSaleItemRequest that defines validation rules for sale items.
/// </summary>
public class CreateSaleItemRequestValidator : AbstractValidator<CreateSaleItemRequest>
{
    /// <summary>
    /// Initializes a new instance of the CreateSaleItemRequestValidator with defined validation rules.
    /// </summary>
    /// <remarks>
    /// Validation rules include:
    /// - Product: Required, maximum length 100 characters
    /// - Quantity: Must be greater than 0 and not exceed 20
    /// - UnitPrice: Must be greater than 0
    /// </remarks>
    public CreateSaleItemRequestValidator()
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