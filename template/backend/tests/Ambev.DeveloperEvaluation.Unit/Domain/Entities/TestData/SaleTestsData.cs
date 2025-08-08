using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides test data for Sale entity tests.
/// </summary>
public static class SaleTestData
{
    private static readonly Faker _faker = new Faker();

    /// <summary>
    /// Generates a valid Sale entity with random data.
    /// </summary>
    /// <returns>A valid Sale entity.</returns>
    public static Sale GenerateValidSale()
    {
        var sale = new Sale
        {
            SaleNumber = _faker.Random.AlphaNumeric(10),
            SaleDate = DateTime.UtcNow.AddDays(-1),
            Customer = _faker.Person.FullName,
            Branch = _faker.Company.CompanyName(),
            CreatedById = Guid.NewGuid()
        };

        // Add 1-3 valid items
        var itemCount = _faker.Random.Int(1, 3);
        for (int i = 0; i < itemCount; i++)
        {
            sale.AddItem(
                _faker.Commerce.ProductName(),
                _faker.Random.Int(1, 20),
                _faker.Random.Decimal(10, 100)
            );
        }

        return sale;
    }

    /// <summary>
    /// Generates an invalid sale number (too long).
    /// </summary>
    /// <returns>An invalid sale number.</returns>
    public static string GenerateInvalidSaleNumber()
    {
        return _faker.Random.String2(60);
    }

    /// <summary>
    /// Generates an invalid sale date (future date).
    /// </summary>
    /// <returns>An invalid sale date.</returns>
    public static DateTime GenerateInvalidSaleDate()
    {
        return DateTime.UtcNow.AddDays(10);
    }

    /// <summary>
    /// Generates a valid SaleItem entity with random data.
    /// </summary>
    /// <returns>A valid SaleItem entity.</returns>
    public static SaleItem GenerateValidSaleItem()
    {
        var quantity = _faker.Random.Int(1, 10);
        var unitPrice = _faker.Random.Decimal(10, 100);

        var item = new SaleItem
        {
            Product = _faker.Commerce.ProductName(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            Discount = quantity < 4 ? 0 : unitPrice * 0.9m
        };
        item.ApplyQuantityBasedDiscount();
        item.CalculateTotalAmount();
        return item;
    }

    /// <summary>
    /// Generates a SaleItem with a specific quantity.
    /// </summary>
    /// <param name="quantity">The quantity to set.</param>
    /// <returns>A SaleItem with the specified quantity.</returns>
    public static SaleItem GenerateSaleItemWithQuantity(int quantity)
    {
        var unitPrice = _faker.Random.Decimal(10, 100);

        var item = new SaleItem
        {
            Product = _faker.Commerce.ProductName(),
            Quantity = quantity,
            UnitPrice = unitPrice,
            Discount = 0
        };

        item.CalculateTotalAmount();
        return item;
    }

    /// <summary>
    /// Generate a randon quantity with 10% discount.
    /// </summary>
    /// <returns>Randon quantity (4 to 10)</returns>
    public static int QuantityDiscountRandon() => _faker.Random.Int(4, 9);
}