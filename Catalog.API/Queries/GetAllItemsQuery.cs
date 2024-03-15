using System;
using Catalog.Entities.Models;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Queries;

public class GetAllItemsQuery : IRequest<PaginatedItemsViewModel<CatalogItem>>
{
    public int PageSize { get; }
    public int PageIndex { get; }

    public GetAllItemsQuery(int pageSize, int pageIndex)
    {
        PageSize = pageSize;
        PageIndex = pageIndex;
    }
}

