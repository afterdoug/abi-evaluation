using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Exceptions;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Contains unit tests for the Sale entity class.
/// Tests cover item management, cancellation, and validation scenarios.
/// </summary>
public class SaleTests
{
    /// <summary>
    /// Tests that when a sale is cancelled, its IsCancelled property becomes true.
    /// </summary>
    [Fact(DisplayName = "Sale should be marked as cancelled when cancelled")]
    public void Given_ValidSale_When_Cancelled_Then_ShouldBeMarkedAsCancelled()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        sale.Cancel();

        // Assert
        Assert.True(sale.IsCancelled);
    }

    /// <summary>
    /// Tests that adding an item to a sale correctly updates the total amount.
    /// </summary>
    [Fact(DisplayName = "Adding item should update total amount")]
    public void Given_ValidSale_When_ItemAdded_Then_TotalAmountShouldBeUpdated()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "TEST-001",
            Customer = "Test Customer",
            Branch = "Test Branch"
        };

        // Act
        var item = sale.AddItem("Test Product", 2, 10.0m);

        // Assert
        Assert.Equal(20.0m, sale.TotalAmount);
        Assert.Contains(item, sale.Items);
    }

    /// <summary>
    /// Tests that removing an item from a sale correctly updates the total amount.
    /// </summary>
    [Fact(DisplayName = "Removing item should update total amount")]
    public void Given_SaleWithItems_When_ItemRemoved_Then_TotalAmountShouldBeUpdated()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "TEST-001",
            Customer = "Test Customer",
            Branch = "Test Branch"
        };
        var item1 = sale.AddItem("Product 1", 2, 10.0m);        ;
        var item2 = sale.AddItem("Product 2", 1, 20.0m);
        item1.Id = Guid.NewGuid();
        item2.Id = Guid.NewGuid();

        // Act
        var result = sale.RemoveItem(item1.Id);

        // Assert
        Assert.True(result);
        Assert.Equal(20.0m, sale.TotalAmount);
        Assert.DoesNotContain(sale.Items, i => i.Id == item1.Id);
        Assert.Contains(sale.Items, i => i.Id == item2.Id);
    }

    /// <summary>
    /// Tests that attempting to remove a non-existent item returns false.
    /// </summary>
    [Fact(DisplayName = "Removing non-existent item should return false")]
    public void Given_Sale_When_RemovingNonExistentItem_Then_ShouldReturnFalse()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var result = sale.RemoveItem(Guid.NewGuid());

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that updating an item quantity correctly updates the total amount.
    /// </summary>
    [Fact(DisplayName = "Updating item quantity should update total amount")]
    public void Given_SaleWithItem_When_ItemQuantityUpdated_Then_TotalAmountShouldBeUpdated()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "TEST-001",
            Customer = "Test Customer",
            Branch = "Test Branch"
        };
        var item = sale.AddItem("Test Product", 2, 10.0m);

        // Act
        sale.UpdateItemQuantity(item.Id, 3);

        // Assert
        Assert.Equal(3, item.Quantity);
        Assert.Equal(30.0m, sale.TotalAmount);
    }

    /// <summary>
    /// Tests that attempting to update a non-existent item returns false.
    /// </summary>
    [Fact(DisplayName = "Updating non-existent item should return false")]
    public void Given_Sale_When_UpdatingNonExistentItem_Then_ShouldReturnFalse()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var result = sale.UpdateItemQuantity(Guid.NewGuid(), 5);

        // Assert
        Assert.False(result);
    }

    /// <summary>
    /// Tests that attempting to add an item with quantity exceeding 20 throws an exception.
    /// </summary>
    [Fact(DisplayName = "Adding item with quantity exceeding 20 should throw exception")]
    public void Given_Sale_When_AddingItemWithExcessiveQuantity_Then_ShouldThrowException()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "TEST-001",
            Customer = "Test Customer",
            Branch = "Test Branch"
        };

        // Act & Assert
        var exception = Assert.Throws<BusinessRuleException>(() =>
            sale.AddItem("Test Product", 21, 10.0m));
        Assert.Equal("Cannot sell more than 20 identical items.", exception.Message);
    }

    /// <summary>
    /// Tests that validation passes when all sale properties are valid.
    /// </summary>
    [Fact(DisplayName = "Validation should pass for valid sale data")]
    public void Given_ValidSaleData_When_Validated_Then_ShouldReturnValid()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var result = sale.Validate();

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    /// <summary>
    /// Tests that validation fails when sale properties are invalid.
    /// </summary>
    [Fact(DisplayName = "Validation should fail for invalid sale data")]
    public void Given_InvalidSaleData_When_Validated_Then_ShouldReturnInvalid()
    {
        // Arrange
        var sale = new Sale
        {
            SaleNumber = "", // Invalid: empty
            SaleDate = SaleTestData.GenerateInvalidSaleDate(), // Invalid: future date
            Customer = "", // Invalid: empty
            Branch = "", // Invalid: empty
            Items = new List<SaleItem>() // Invalid: no items
        };

        // Act
        var result = sale.Validate();

        // Assert
        Assert.False(result.IsValid);
        Assert.NotEmpty(result.Errors);
    }
}