using System;
using Catalog.Entities.Models;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Read.Queries;

public class GetAllItemsQuery(int pageSize, int pageIndex) : IRequest<PaginatedItemsViewModel<CatalogItem>>
{
    public int PageSize { get; } = pageSize;
    public int PageIndex { get; } = pageIndex;
}

