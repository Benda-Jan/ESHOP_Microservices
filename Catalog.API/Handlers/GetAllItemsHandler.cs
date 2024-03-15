using System;
using Catalog.Infrastructure.Data;
using Catalog.Entities.Models;
using Catalog.API.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Handlers;

public class GetAllItemsHandler : IRequestHandler<GetAllItemsQuery, PaginatedItemsViewModel<CatalogItem>>
{
    private readonly CatalogContext _catalogContext;

    public GetAllItemsHandler(CatalogContext catalogContext)
    {
        _catalogContext = catalogContext;
    }

    public async Task<PaginatedItemsViewModel<CatalogItem>> Handle(GetAllItemsQuery request, CancellationToken cancellationToken)
    {
        var totalItems = await _catalogContext.CatalogItems.LongCountAsync();

        var itemsOnPage = await _catalogContext.CatalogItems
            .OrderBy(x => x.Name)
            .Skip(request.PageSize * request.PageIndex)
            .Take(request.PageSize)
            .ToListAsync();

        var model = new PaginatedItemsViewModel<CatalogItem>(request.PageIndex, request.PageSize, totalItems, itemsOnPage);

        return model;
    }
}

