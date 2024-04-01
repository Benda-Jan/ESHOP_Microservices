using Catalog.Entities.Models;
using Catalog.API.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers;

public class GetAllItemsHandler : IRequestHandler<GetAllItemsQuery, PaginatedItemsViewModel<CatalogItem>>
{
    private readonly CatalogRepository _catalogRepository;

    public GetAllItemsHandler(CatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<PaginatedItemsViewModel<CatalogItem>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        var result = await _catalogRepository.GetAllItems(request.PageSize, request.PageIndex);

        var model = new PaginatedItemsViewModel<CatalogItem>(result.PageIndex, result.PageSize, result.TotalItems, result.Items.ToList());

        return model;
    }
}

