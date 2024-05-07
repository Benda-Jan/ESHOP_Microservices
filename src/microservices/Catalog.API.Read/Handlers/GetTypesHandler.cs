using System;
using Catalog.Infrastructure.Data;
using Catalog.Entities.Models;
using Catalog.API.Read.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Read.Handlers;

public class GetTypesHandler(CatalogContext catalogContext) : IRequestHandler<GetTypesQuery, CatalogType[]>
{
    private readonly CatalogContext _catalogContext = catalogContext;

    public Task<CatalogType[]> Handle(GetTypesQuery request, CancellationToken cancellationToken)
        => _catalogContext.CatalogTypes.AsNoTracking().ToArrayAsync();
}

