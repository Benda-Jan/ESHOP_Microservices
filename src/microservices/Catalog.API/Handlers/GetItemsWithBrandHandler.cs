using Catalog.API.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Entities.Models;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers;

public class GetItemsWithBrandHandler : IRequestHandler<GetItemsWithBrandQuery, PaginatedItemsViewModel<CatalogItem>>
{
    private readonly CatalogRepository _catalogRepository;

    public GetItemsWithBrandHandler(CatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<PaginatedItemsViewModel<CatalogItem>> Handle(GetItemsWithBrandQuery request, CancellationToken cancellationToken)
    {
        var result = await _catalogRepository.GetItemsWithBrand(request.BrandName, request.PageSize, request.PageSize);

        return new PaginatedItemsViewModel<CatalogItem>(result.PageIndex, result.PageSize, result.TotalItems, result.Items.ToList());
    }
        

}

