using Catalog.API.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Entities.Models;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers;

public class GetItemsWithTypeHandler : IRequestHandler<GetItemsWithTypeQuery, PaginatedItemsViewModel<CatalogItem>>
{
    private readonly CatalogRepository _catalogRepository;

    public GetItemsWithTypeHandler(CatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<PaginatedItemsViewModel<CatalogItem>> Handle(GetItemsWithTypeQuery request, CancellationToken cancellationToken)
    {
        var result = await _catalogRepository.GetItemsWithType(request.TypeName, request.PageSize, request.PageSize);

        return new PaginatedItemsViewModel<CatalogItem>(result.PageIndex, result.PageSize, result.TotalItems, result.Items.ToList());
    }


}

