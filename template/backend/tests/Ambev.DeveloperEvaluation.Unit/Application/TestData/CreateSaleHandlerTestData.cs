using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides test data for CreateSaleHandler tests.
/// </summary>
public static class CreateSaleHandlerTestData
{
    /// <summary>
    /// Generates a valid CreateSaleCommand for testing.
    /// </summary>
    /// <returns>A valid CreateSaleCommand instance.</returns>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return new CreateSaleCommand
        {
            SaleNumber = $"TEST-{Guid.NewGuid().ToString().Substring(0, 8)}",
            SaleDate = DateTime.UtcNow.AddHours(-1),
            Customer = "Test Customer",
            Branch = "Test Branch",
            CreatedById = Guid.NewGuid(),
            Items = new List<CreateSaleItemCommand>
            {
                new CreateSaleItemCommand
                {
                    Product = "Test Product 1",
                    Quantity = 2,
                    UnitPrice = 10.99m
                },
                new CreateSaleItemCommand
                {
                    Product = "Test Product 2",
                    Quantity = 5,
                    UnitPrice = 5.99m
                }
            }
        };
    }
}