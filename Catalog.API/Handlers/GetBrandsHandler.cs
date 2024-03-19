using System;
using Catalog.Infrastructure.Data;
using Catalog.Entities.Models;
using Catalog.API.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Handlers;

public class GetBrandsHandler : IRequestHandler<GetBrandsQuery, CatalogBrand[]>
{
    private readonly CatalogContext _catalogContext;

    public GetBrandsHandler(CatalogContext catalogContext)
    {
        _catalogContext = catalogContext;
    }

    public Task<CatalogBrand[]> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        => _catalogContext.CatalogBrands.AsNoTracking().ToArrayAsync();
}

