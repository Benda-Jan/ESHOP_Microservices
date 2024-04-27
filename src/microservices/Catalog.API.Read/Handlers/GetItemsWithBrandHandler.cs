using Catalog.API.Read.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Entities.Models;
using Catalog.Infrastructure;

namespace Catalog.API.Read.Handlers;

public class GetItemsWithBrandHandler : IRequestHandler<GetItemsWithBrandQuery, PaginatedItemsViewModel<CatalogItem>>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetItemsWithBrandHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<PaginatedItemsViewModel<CatalogItem>> Handle(GetItemsWithBrandQuery request, CancellationToken cancellationToken)
    {
        var result = await _catalogRepository.GetItemsByBrand(request.BrandName, request.PageSize, request.PageIndex);

        return new PaginatedItemsViewModel<CatalogItem>(result.PageIndex, result.PageSize, result.TotalItems, result.Items.ToList());
    }
        

}

