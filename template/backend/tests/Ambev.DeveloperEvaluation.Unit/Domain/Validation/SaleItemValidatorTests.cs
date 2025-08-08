using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Validation;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Validation;

/// <summary>
/// Contains unit tests for the SaleItemValidator class.
/// </summary>
public class SaleItemValidatorTests
{
    private readonly SaleItemValidator _validator;

    public SaleItemValidatorTests()
    {
        _validator = new SaleItemValidator();
    }

    /// <summary>
    /// Tests that validation passes for a valid sale item.
    /// </summary>
    [Fact(DisplayName = "Valid sale item should pass all validation rules")]
    public void Given_ValidSaleItem_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    /// <summary>
    /// Tests that validation fails when product is empty.
    /// </summary>
    [Fact(DisplayName = "Empty product should fail validation")]
    public void Given_EmptyProduct_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();
        item.Product = "";

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Product);
    }

    /// <summary>
    /// Tests that validation fails when quantity is zero or negative.
    /// </summary>
    [Theory(DisplayName = "Zero or negative quantity should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_ZeroOrNegativeQuantity_When_Validated_Then_ShouldHaveError(int quantity)
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();
        item.Quantity = quantity;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    /// <summary>
    /// Tests that validation fails when quantity exceeds 20.
    /// </summary>
    [Fact(DisplayName = "Quantity exceeding 20 should fail validation")]
    public void Given_QuantityExceeding20_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();
        item.Quantity = 21;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Quantity);
    }

    /// <summary>
    /// Tests that validation fails when unit price is zero or negative.
    /// </summary>
    [Theory(DisplayName = "Zero or negative unit price should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Given_ZeroOrNegativeUnitPrice_When_Validated_Then_ShouldHaveError(decimal unitPrice)
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();
        item.UnitPrice = unitPrice;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UnitPrice);
    }

    /// <summary>
    /// Tests that validation fails when discount is negative.
    /// </summary>
    [Fact(DisplayName = "Negative discount should fail validation")]
    public void Given_NegativeDiscount_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();
        item.Discount = -10;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Discount);
    }

    /// <summary>
    /// Tests that validation fails when discount exceeds total value.
    /// </summary>
    [Fact(DisplayName = "Discount exceeding total value should fail validation")]
    public void Given_DiscountExceedingTotalValue_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();
        item.Discount = (item.Quantity * item.UnitPrice) + 1;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Discount);
    }

    /// <summary>
    /// Tests that validation fails when discount is applied to quantity less than 4.
    /// </summary>
    [Fact(DisplayName = "Discount applied to quantity less than 4 should fail validation")]
    public void Given_DiscountAppliedToQuantityLessThan4_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItemWithQuantity(3);
        item.Discount = 5;

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Discount);
    }

    /// <summary>
    /// Tests that validation fails when incorrect discount percentage is applied.
    /// </summary>
    [Theory(DisplayName = "Incorrect discount percentage should fail validation")]
    [InlineData(3, 0.10)]  // Less than 4 items with 10% discount
    [InlineData(5, 0.20)]  // 4-9 items with 20% discount
    [InlineData(15, 0.10)] // 10-20 items with 10% discount
    public void Given_IncorrectDiscountPercentage_When_Validated_Then_ShouldHaveError(int quantity, decimal discountPercentage)
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItemWithQuantity(quantity);

        // Use reflection to set the private property for testing
        typeof(SaleItem).GetProperty("DiscountPercentage")
            .SetValue(item, discountPercentage, null);

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.DiscountPercentage);
    }

    /// <summary>
    /// Tests that validation fails when total amount calculation is incorrect.
    /// </summary>
    [Fact(DisplayName = "Incorrect total amount calculation should fail validation")]
    public void Given_IncorrectTotalAmount_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var item = SaleTestData.GenerateValidSaleItem();
        item.TotalAmount = item.Quantity * item.UnitPrice; // Incorrect (doesn't subtract discount)

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.TotalAmount);
    }
}