using System;
using Catalog.Infrastructure.Data;
using Catalog.API.Queries;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Handlers;

public class GetItemByIdHandler : IRequestHandler<GetItemByIdQuery, CatalogItem?>
{
    private readonly CatalogContext _catalogContext;

    public GetItemByIdHandler(CatalogContext catalogContext)
    {
        _catalogContext = catalogContext;
    }

    public async Task<CatalogItem?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        => await _catalogContext.CatalogItems.FindAsync(request.Id);

}

