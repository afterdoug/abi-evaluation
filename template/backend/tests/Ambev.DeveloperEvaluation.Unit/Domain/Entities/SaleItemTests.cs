using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the SaleItem entity class.
/// Tests cover discount rules, total amount calculation, and validation scenarios.
/// </summary>
public class SaleItemTests
{
    /// <summary>
    /// Tests that total amount is calculated correctly.
    /// </summary>
    [Fact(DisplayName = "Total amount should be calculated correctly")]
    public void Given_SaleItem_When_CalculatingTotalAmount_Then_ShouldBeCorrect()
    {
        // Arrange
        var item = new SaleItem
        {
            Product = "Test Product",
            Quantity = 5,
            UnitPrice = 10.0m,
            Discount = 10.0m
        };

        // Act
        item.CalculateTotalAmount();

        // Assert
        Assert.Equal(40.0m, item.TotalAmount); // (5 * 10) - 10 = 40
    }

    /// <summary>
    /// Tests that no discount is applied when quantity is less than 4.
    /// </summary>
    [Fact(DisplayName = "No discount should be applied when quantity is less than 4")]
    public void Given_QuantityLessThan4_When_ApplyingDiscount_Then_NoDiscountShouldBeApplied()
    {
        // Arrange
        var item = new SaleItem
        {
            Product = "Test Product",
            Quantity = 3,
            UnitPrice = 10.0m,
            Discount = 0
        };

        // Act
        item.ApplyQuantityBasedDiscount();

        // Assert
        Assert.Equal(0, item.Discount);
        Assert.Equal(0, item.DiscountPercentage);
        Assert.Equal(30.0m, item.TotalAmount);
    }

    /// <summary>
    /// Tests that 10% discount is applied when quantity is between 4 and 9.
    /// </summary>
    [Fact(DisplayName = "10% discount should be applied when quantity is between 4 and 9")]
    public void Given_QuantityBetween4And9_When_ApplyingDiscount_Then_10PercentDiscountShouldBeApplied()
    {
        // Arrange
        var item = new SaleItem
        {
            Product = "Test Product",
            Quantity = 5,
            UnitPrice = 10.0m,
            Discount = 0
        };

        // Act
        item.ApplyQuantityBasedDiscount();

        // Assert
        Assert.Equal(5.0m, item.Discount); // 5 * 10 * 0.1 = 5
        Assert.Equal(0.10m, item.DiscountPercentage);
        Assert.Equal(45.0m, item.TotalAmount); // (5 * 10) - 5 = 45
    }

    /// <summary>
    /// Tests that 20% discount is applied when quantity is between 10 and 20.
    /// </summary>
    [Fact(DisplayName = "20% discount should be applied when quantity is between 10 and 20")]
    public void Given_QuantityBetween10And20_When_ApplyingDiscount_Then_20PercentDiscountShouldBeApplied()
    {
        // Arrange
        var item = new SaleItem
        {
            Product = "Test Product",
            Quantity = 15,
            UnitPrice = 10.0m,
            Discount = 0
        };

        // Act
        item.ApplyQuantityBasedDiscount();

        // Assert
        Assert.Equal(30.0m, item.Discount); // 15 * 10 * 0.2 = 30
        Assert.Equal(0.20m, item.DiscountPercentage);
        Assert.Equal(120.0m, item.TotalAmount); // (15 * 10) - 30 = 120
    }

    /// <summary>
    /// Tests that an exception is thrown when quantity exceeds 20.
    /// </summary>
    [Fact(DisplayName = "Exception should be thrown when quantity exceeds 20")]
    public void Given_QuantityExceeding20_When_ApplyingDiscount_Then_ShouldThrowException()
    {
        // Arrange
        var item = new SaleItem
        {
            Product = "Test Product",
            Quantity = 21,
            UnitPrice = 10.0m,
            Discount = 0
        };

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleException>(() => item.ApplyQuantityBasedDiscount());
        Assert.Equal("Cannot sell more than 20 identical items.", exception.Message);
    }

    /// <summary>
    /// Tests that an exception is thrown when a discount is applied to a quantity less than 4.
    /// </summary>
    [Fact(DisplayName = "Exception should be thrown when discount is applied to quantity less than 4")]
    public void Given_QuantityLessThan4WithDiscount_When_ApplyingDiscount_Then_ShouldThrowException()
    {
        // Arrange
        var item = new SaleItem
        {
            Product = "Test Product",
            Quantity = 3,
            UnitPrice = 10.0m,
            Discount = 5.0m
        };

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleException>(() => item.ApplyQuantityBasedDiscount());
        Assert.Equal("Purchases below 4 items cannot have a discount.", exception.Message);
    }
}