using System;
using Catalog.Entities.DbSet;
using Catalog.Entities.Models;
using MediatR;

namespace Catalog.API.Queries;

public class GetItemsWithTypeQuery : IRequest<PaginatedItemsViewModel<CatalogItem>>
{
    public string TypeName { get; }
    public int PageSize { get; }
    public int PageIndex { get; }

    public GetItemsWithTypeQuery(string typeName, int pageSize, int pageIndex)
    {
        TypeName = typeName;
        PageSize = pageSize;
        PageIndex = pageIndex;
    }
}

