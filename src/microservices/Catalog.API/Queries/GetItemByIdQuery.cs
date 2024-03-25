using System;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Queries;

public class GetItemByIdQuery : IRequest<CatalogItem?>
{
    public int Id { get; }

    public GetItemByIdQuery(int id)
    {
        Id = id;
    }
}

