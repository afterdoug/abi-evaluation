using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;

/// <summary>
/// Profile for mapping GetSale feature requests to commands
/// </summary>
public class GetSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetSale feature
    /// </summary>
    public GetSaleProfile()
    {
        CreateMap<Guid, Application.Sales.GetSale.GetSaleCommand>()
            .ConstructUsing(id => new Application.Sales.GetSale.GetSaleCommand(id));

        CreateMap<Application.Sales.GetSale.GetSaleResult, GetSaleResponse>();
        CreateMap<Application.Sales.CreateSale.SaleItemResult, SaleItemResponse>();
        CreateMap<Application.Sales.GetSale.CreatedByUserResult, CreatedByUserResponse>();
    }
}