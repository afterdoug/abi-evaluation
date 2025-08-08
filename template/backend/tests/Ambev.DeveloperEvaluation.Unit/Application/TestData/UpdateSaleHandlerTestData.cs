using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides test data for UpdateSaleHandler tests.
/// </summary>
public static class UpdateSaleHandlerTestData
{
    /// <summary>
    /// Generates a valid UpdateSaleCommand for testing.
    /// </summary>
    /// <returns>A valid UpdateSaleCommand instance.</returns>
    public static UpdateSaleCommand GenerateValidCommand()
    {
        return new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = $"UPD-{Guid.NewGuid().ToString().Substring(0, 8)}",
            SaleDate = DateTime.UtcNow.AddHours(-1),
            Customer = "Updated Customer",
            Branch = "Updated Branch",
            Items = new List<UpdateSaleItemCommand>
            {
                new UpdateSaleItemCommand
                {
                    Product = "Updated Product 1",
                    Quantity = 3,
                    UnitPrice = 15.99m
                }
            }
        };
    }

    /// <summary>
    /// Generates an existing Sale entity for testing.
    /// </summary>
    /// <param name="id">The ID to use for the sale</param>
    /// <returns>A Sale entity</returns>
    public static Sale GenerateExistingSale(Guid id)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "TestUser"
        };

        var sale = new Sale
        {
            Id = id,
            SaleNumber = "OLD-001",
            SaleDate = DateTime.UtcNow.AddDays(-2),
            Customer = "Original Customer",
            Branch = "Original Branch",
            CreatedById = user.Id,
            CreatedBy = user,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    Product = "Original Product",
                    Quantity = 2,
                    UnitPrice = 10.0m,
                    Discount = 0,
                    TotalAmount = 20.0m
                }
            },
            TotalAmount = 20.0m
        };

        return sale;
    }

    /// <summary>
    /// Generates an updated Sale entity based on the command.
    /// </summary>
    /// <param name="command">The update command</param>
    /// <returns>An updated Sale entity</returns>
    public static Sale GenerateUpdatedSale(UpdateSaleCommand command)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "TestUser"
        };

        var sale = new Sale
        {
            Id = command.Id,
            SaleNumber = command.SaleNumber,
            SaleDate = command.SaleDate,
            Customer = command.Customer,
            Branch = command.Branch,
            CreatedById = user.Id,
            CreatedBy = user,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            UpdatedAt = DateTime.UtcNow,
            Items = new List<SaleItem>()
        };

        foreach (var itemCommand in command.Items)
        {
            var item = new SaleItem
            {
                Id = itemCommand.Id ?? Guid.NewGuid(),
                Product = itemCommand.Product,
                Quantity = itemCommand.Quantity,
                UnitPrice = itemCommand.UnitPrice,
                TotalAmount = itemCommand.Quantity * itemCommand.UnitPrice
            };
            sale.Items.Add(item);
        }

        sale.TotalAmount = sale.Items.Sum(i => i.TotalAmount);

        return sale;
    }

    /// <summary>
    /// Generates an UpdateSaleResult based on a Sale entity.
    /// </summary>
    /// <param name="sale">The sale entity</param>
    /// <returns>An UpdateSaleResult</returns>
    public static UpdateSaleResult GenerateUpdateResult(Sale sale)
    {
        var result = new UpdateSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            SaleDate = sale.SaleDate,
            Customer = sale.Customer,
            Branch = sale.Branch,
            TotalAmount = sale.TotalAmount,
            IsCancelled = sale.IsCancelled,
            CreatedAt = sale.CreatedAt,
            UpdatedAt = sale.UpdatedAt,
            Items = new List<UpdateSaleItemResult>()
        };

        foreach (var item in sale.Items)
        {
            result.Items.Add(new UpdateSaleItemResult
            {
                Id = item.Id,
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                DiscountPercentage = item.DiscountPercentage,
                TotalAmount = item.TotalAmount
            });
        }

        return result;
    }

    /// <summary>
    /// Generates an UpdateSaleCommand with an existing item.
    /// </summary>
    /// <param name="existingItemId">The ID of the existing item</param>
    /// <returns>An UpdateSaleCommand with an existing item</returns>
    public static UpdateSaleCommand GenerateCommandWithExistingItem(Guid existingItemId)
    {
        return new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = $"UPD-{Guid.NewGuid().ToString().Substring(0, 8)}",
            SaleDate = DateTime.UtcNow.AddHours(-1),
            Customer = "Updated Customer",
            Branch = "Updated Branch",
            Items = new List<UpdateSaleItemCommand>
            {
                new UpdateSaleItemCommand
                {
                    Id = existingItemId,
                    Product = "Updated Existing Product",
                    Quantity = 5,
                    UnitPrice = 12.99m
                }
            }
        };
    }

    /// <summary>
    /// Generates a Sale entity with an existing item.
    /// </summary>
    /// <param name="saleId">The ID of the sale</param>
    /// <param name="existingItemId">The ID of the existing item</param>
    /// <returns>A Sale entity with an existing item</returns>
    public static Sale GenerateSaleWithExistingItem(Guid saleId, Guid existingItemId)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "TestUser"
        };

        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = "OLD-001",
            SaleDate = DateTime.UtcNow.AddDays(-2),
            Customer = "Original Customer",
            Branch = "Original Branch",
            CreatedById = user.Id,
            CreatedBy = user,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    Id = existingItemId,
                    Product = "Original Product",
                    Quantity = 2,
                    UnitPrice = 10.0m,
                    Discount = 0,
                    TotalAmount = 20.0m
                }
            },
            TotalAmount = 20.0m
        };

        return sale;
    }

    /// <summary>
    /// Generates an UpdateSaleCommand with a new item.
    /// </summary>
    /// <returns>An UpdateSaleCommand with a new item</returns>
    public static UpdateSaleCommand GenerateCommandWithNewItem()
    {
        return new UpdateSaleCommand
        {
            Id = Guid.NewGuid(),
            SaleNumber = $"UPD-{Guid.NewGuid().ToString().Substring(0, 8)}",
            SaleDate = DateTime.UtcNow.AddHours(-1),
            Customer = "Updated Customer",
            Branch = "Updated Branch",
            Items = new List<UpdateSaleItemCommand>
            {
                new UpdateSaleItemCommand
                {
                    Product = "New Product",
                    Quantity = 3,
                    UnitPrice = 9.99m
                }
            }
        };
    }

    /// <summary>
    /// Generates a Sale entity with an extra item that is not in the update command.
    /// </summary>
    /// <param name="saleId">The ID of the sale</param>
    /// <returns>A Sale entity with an extra item</returns>
    public static Sale GenerateSaleWithExtraItem(Guid saleId)
    {
        var sale = GenerateExistingSale(saleId);

        // Add an extra item that should be removed during update
        sale.Items.Add(new SaleItem
        {
            Id = Guid.NewGuid(),
            Product = "Extra Product To Remove",
            Quantity = 1,
            UnitPrice = 5.0m,
            Discount = 0,
            TotalAmount = 5.0m
        });

        sale.TotalAmount = sale.Items.Sum(i => i.TotalAmount);

        return sale;
    }
}