using Catalog.Entities.Models;
using Catalog.API.Read.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Infrastructure;

namespace Catalog.API.Read.Handlers;

public class GetAllItemsHandler : IRequestHandler<GetAllItemsQuery, PaginatedItemsViewModel<CatalogItem>>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetAllItemsHandler(ICatalogRepository catalogRepository)
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

