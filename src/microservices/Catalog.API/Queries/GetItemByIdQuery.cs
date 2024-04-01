using System;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Queries;

public class GetItemByIdQuery : IRequest<CatalogItem?>
{
    public string Id { get; }

    public GetItemByIdQuery(string id)
    {
        Id = id;
    }
}

