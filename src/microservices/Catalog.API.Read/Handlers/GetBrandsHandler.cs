using Catalog.Infrastructure.Data;
using Catalog.API.Read.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Catalog.API.Read.Handlers;

public class GetBrandsHandler(CatalogContext catalogContext) : IRequestHandler<GetBrandsQuery, CatalogBrand[]>
{
    private readonly CatalogContext _catalogContext = catalogContext;

    public Task<CatalogBrand[]> Handle(GetBrandsQuery request, CancellationToken cancellationToken)
        => _catalogContext.CatalogBrands.AsNoTracking().ToArrayAsync();
}

