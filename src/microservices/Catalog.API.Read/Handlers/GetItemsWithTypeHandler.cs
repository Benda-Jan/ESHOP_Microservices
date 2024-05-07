using Catalog.API.Read.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Entities.Models;
using Catalog.Infrastructure;

namespace Catalog.API.Read.Handlers;

public class GetItemsWithTypeHandler(ICatalogRepository catalogRepository) : IRequestHandler<GetItemsWithTypeQuery, PaginatedItemsViewModel<CatalogItem>>
{
    private readonly ICatalogRepository _catalogRepository = catalogRepository;

    public async Task<PaginatedItemsViewModel<CatalogItem>> Handle(GetItemsWithTypeQuery request, CancellationToken cancellationToken)
    {
        var result = await _catalogRepository.GetItemsByType(request.TypeName, request.PageSize, request.PageIndex);

        return new PaginatedItemsViewModel<CatalogItem>(result.PageIndex, result.PageSize, result.TotalItems, result.Items.ToList());
    }


}

