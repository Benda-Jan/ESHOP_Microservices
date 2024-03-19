using System;
using Catalog.Infrastructure.Data;
using Catalog.Entities.Models;
using Catalog.API.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Handlers;

public class GetTypesHandler : IRequestHandler<GetTypesQuery, CatalogType[]>
{
    private readonly CatalogContext _catalogContext;

    public GetTypesHandler(CatalogContext catalogContext)
    {
        _catalogContext = catalogContext;
    }

    public Task<CatalogType[]> Handle(GetTypesQuery request, CancellationToken cancellationToken)
        => _catalogContext.CatalogTypes.AsNoTracking().ToArrayAsync();
}

