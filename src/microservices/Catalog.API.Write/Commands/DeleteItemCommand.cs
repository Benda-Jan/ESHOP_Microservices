using System;
using Catalog.Entities.Dtos;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Write.Commands;

public class DeleteItemCommand(string itemId) : IRequest<CatalogItem?>
{
    public string ItemId { get; } = itemId;
}

