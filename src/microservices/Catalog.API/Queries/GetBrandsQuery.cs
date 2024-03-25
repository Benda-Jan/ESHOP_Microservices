using System;
using Catalog.Entities.Models;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Queries;

public class GetBrandsQuery : IRequest<CatalogBrand[]>
{
    public GetBrandsQuery()
    {
    }
}

