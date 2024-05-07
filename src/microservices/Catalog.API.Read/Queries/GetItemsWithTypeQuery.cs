using System;
using Catalog.Entities.DbSet;
using Catalog.Entities.Models;
using MediatR;

namespace Catalog.API.Read.Queries;

public class GetItemsWithTypeQuery(string typeName, int pageSize, int pageIndex) : IRequest<PaginatedItemsViewModel<CatalogItem>>
{
    public string TypeName { get; } = typeName;
    public int PageSize { get; } = pageSize;
    public int PageIndex { get; } = pageIndex;
}

