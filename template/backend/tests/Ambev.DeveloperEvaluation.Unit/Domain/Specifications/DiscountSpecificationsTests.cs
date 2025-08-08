using Ambev.DeveloperEvaluation.Domain.Specifications.Sales;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Specifications.Sales;

/// <summary>
/// Contains unit tests for the discount specifications.
/// </summary>
public class DiscountSpecificationsTests
{
    /// <summary>
    /// Tests that TenPercentDiscountSpecification correctly identifies eligible items.
    /// </summary>
    [Theory(DisplayName = "TenPercentDiscountSpecification should correctly identify eligible items")]
    [InlineData(3, false)]  // Less than 4
    [InlineData(4, true)]   // Exactly 4
    [InlineData(7, true)]   // Between 4 and 9
    [InlineData(9, true)]   // Exactly 9
    [InlineData(10, false)] // 10 or more
    public void TenPercentDiscountSpecification_ShouldCorrectlyIdentifyEligibleItems(int quantity, bool expectedResult)
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItemWithQuantity(quantity);
        var specification = new TenPercentDiscountSpecification();

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <summary>
    /// Tests that TwentyPercentDiscountSpecification correctly identifies eligible items.
    /// </summary>
    [Theory(DisplayName = "TwentyPercentDiscountSpecification should correctly identify eligible items")]
    [InlineData(9, false)]   // Less than 10
    [InlineData(10, true)]   // Exactly 10
    [InlineData(15, true)]   // Between 10 and 20
    [InlineData(20, true)]   // Exactly 20
    [InlineData(21, false)]  // More than 20
    public void TwentyPercentDiscountSpecification_ShouldCorrectlyIdentifyEligibleItems(int quantity, bool expectedResult)
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItemWithQuantity(quantity);
        var specification = new TwentyPercentDiscountSpecification();

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <summary>
    /// Tests that MaximumQuantitySpecification correctly identifies items that exceed the maximum quantity.
    /// </summary>
    [Theory(DisplayName = "MaximumQuantitySpecification should correctly identify items that exceed maximum quantity")]
    [InlineData(19, true)]   // Less than 20
    [InlineData(20, true)]   // Exactly 20
    [InlineData(21, false)]  // More than 20
    public void MaximumQuantitySpecification_ShouldCorrectlyIdentifyItemsExceedingMaximum(int quantity, bool expectedResult)
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItemWithQuantity(quantity);
        var specification = new MaximumQuantitySpecification();

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        result.Should().Be(expectedResult);
    }

    /// <summary>
    /// Tests that DiscountEligibilitySpecification correctly identifies items eligible for any discount.
    /// </summary>
    [Theory(DisplayName = "DiscountEligibilitySpecification should correctly identify discount-eligible items")]
    [InlineData(3, false)]   // Less than 4
    [InlineData(4, true)]    // Exactly 4
    [InlineData(10, true)]   // More than 4
    public void DiscountEligibilitySpecification_ShouldCorrectlyIdentifyEligibleItems(int quantity, bool expectedResult)
    {
        // Arrange
        var item = SaleTestData.GenerateSaleItemWithQuantity(quantity);
        var specification = new DiscountEligibilitySpecification();

        // Act
        var result = specification.IsSatisfiedBy(item);

        // Assert
        result.Should().Be(expectedResult);
    }
}