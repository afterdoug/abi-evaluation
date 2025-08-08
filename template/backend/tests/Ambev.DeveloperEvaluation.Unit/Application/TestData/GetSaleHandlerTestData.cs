using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Provides test data for GetSaleHandler tests.
/// </summary>
public static class GetSaleHandlerTestData
{
    /// <summary>
    /// Generates a valid Sale entity and corresponding GetSaleResult for testing.
    /// </summary>
    /// <param name="saleId">The ID to use for the sale</param>
    /// <returns>A tuple containing a Sale entity and a GetSaleResult</returns>
    public static (Sale Sale, GetSaleResult Result) GenerateValidSaleAndResult(Guid saleId)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "TestUser"
        };

        var saleItems = new List<SaleItem>
        {
            new SaleItem
            {
                Id = Guid.NewGuid(),
                Product = "Test Product 1",
                Quantity = 2,
                UnitPrice = 10.0m,
                Discount = 0,
                TotalAmount = 20.0m
            },
            new SaleItem
            {
                Id = Guid.NewGuid(),
                Product = "Test Product 2",
                Quantity = 5,
                UnitPrice = 15.0m,
                Discount = 7.5m,
                TotalAmount = 67.5m
            }
        };

        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = "TEST-001",
            SaleDate = DateTime.UtcNow.AddDays(-1),
            Customer = "Test Customer",
            Branch = "Test Branch",
            TotalAmount = 87.5m,
            CreatedById = user.Id,
            CreatedBy = user,
            Items = saleItems,
            CreatedAt = DateTime.UtcNow.AddDays(-1),
            UpdatedAt = DateTime.UtcNow.AddHours(-1)
        };

        var resultItems = new List<SaleItemResult>();
        foreach (var item in saleItems)
        {
            resultItems.Add(new SaleItemResult
            {
                Id = item.Id,
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                TotalAmount = item.TotalAmount
            });
        }

        var result = new GetSaleResult
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
            CreatedBy = new CreatedByUserResult
            {
                Id = user.Id,
                Username = user.Username
            },
            Items = resultItems
        };

        return (sale, result);
    }

    /// <summary>
    /// Generates a Sale entity with a specified number of items and corresponding GetSaleResult.
    /// </summary>
    /// <param name="saleId">The ID to use for the sale</param>
    /// <param name="itemCount">The number of items to generate</param>
    /// <returns>A tuple containing a Sale entity and a GetSaleResult</returns>
    public static (Sale Sale, GetSaleResult Result) GenerateSaleWithMultipleItems(Guid saleId, int itemCount)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "TestUser"
        };

        var saleItems = new List<SaleItem>();
        for (int i = 0; i < itemCount; i++)
        {
            saleItems.Add(new SaleItem
            {
                Id = Guid.NewGuid(),
                Product = $"Product {i + 1}",
                Quantity = i + 1,
                UnitPrice = 10.0m * (i + 1),
                Discount = i * 5.0m,
                TotalAmount = (10.0m * (i + 1) * (i + 1)) - (i * 5.0m)
            });
        }

        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = "TEST-MULTI",
            SaleDate = DateTime.UtcNow.AddDays(-1),
            Customer = "Multi-Item Customer",
            Branch = "Test Branch",
            TotalAmount = saleItems.Sum(item => item.TotalAmount),
            CreatedById = user.Id,
            CreatedBy = user,
            Items = saleItems
        };

        var resultItems = new List<SaleItemResult>();
        foreach (var item in saleItems)
        {
            resultItems.Add(new SaleItemResult
            {
                Id = item.Id,
                Product = item.Product,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Discount = item.Discount,
                TotalAmount = item.TotalAmount
            });
        }

        var result = new GetSaleResult
        {
            Id = sale.Id,
            SaleNumber = sale.SaleNumber,
            Customer = sale.Customer,
            Branch = sale.Branch,
            TotalAmount = sale.TotalAmount,
            Items = resultItems,
            CreatedBy = new CreatedByUserResult
            {
                Id = user.Id,
                Username = user.Username
            }
        };

        return (sale, result);
    }

    /// <summary>
    /// Generates a Sale entity with a specific creator and corresponding GetSaleResult.
    /// </summary>
    /// <param name="saleId">The ID to use for the sale</param>
    /// <param name="userId">The ID to use for the creator</param>
    /// <param name="username">The username of the creator</param>
    /// <returns>A tuple containing a Sale entity and a GetSaleResult</returns>
    public static (Sale Sale, GetSaleResult Result) GenerateSaleWithCreator(Guid saleId, Guid userId, string username)
    {
        var user = new User
        {
            Id = userId,
            Username = username
        };

        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = "TEST-CREATOR",
            SaleDate = DateTime.UtcNow.AddDays(-1),
            Customer = "Creator Test Customer",
            Branch = "Test Branch",
            TotalAmount = 100.0m,
            CreatedById = userId,
            CreatedBy = user,
            Items = new List<SaleItem>
            {
                new SaleItem
                {
                    Id = Guid.NewGuid(),
                    Product = "Test Product",
                    Quantity = 10,
                    UnitPrice = 10.0m,
                    TotalAmount = 100.0m
                }
            }
        };

        var result = new GetSaleResult
        {
            Id = saleId,
            SaleNumber = sale.SaleNumber,
            Customer = sale.Customer,
            Branch = sale.Branch,
            TotalAmount = sale.TotalAmount,
            Items = new List<SaleItemResult>
            {
                new SaleItemResult
                {
                    Id = sale.Items.First().Id,
                    Product = "Test Product",
                    Quantity = 10,
                    UnitPrice = 10.0m,
                    TotalAmount = 100.0m
                }
            },
            CreatedBy = new CreatedByUserResult
            {
                Id = userId,
                Username = username
            }
        };

        return (sale, result);
    }
}