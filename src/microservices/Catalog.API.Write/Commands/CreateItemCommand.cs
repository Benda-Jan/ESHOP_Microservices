using System;
using Catalog.Entities.Dtos;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Write.Commands;

public class CreateItemCommand(CatalogItemInputDto inputItem) : IRequest<CatalogItem?>
{
    public CatalogItemInputDto InputItem { get; } = inputItem;
}

