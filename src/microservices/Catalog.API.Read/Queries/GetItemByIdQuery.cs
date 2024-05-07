using System;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Read.Queries;

public class GetItemByIdQuery(string id) : IRequest<CatalogItem?>
{
    public string Id { get; } = id;
}

