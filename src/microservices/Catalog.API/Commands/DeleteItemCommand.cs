using System;
using Catalog.Entities.Dtos;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Commands;

public class DeleteItemCommand : IRequest<CatalogItem?>
{
    public string ItemId { get; }

    public DeleteItemCommand(string itemId)
    {
        ItemId = itemId;
    }
}

