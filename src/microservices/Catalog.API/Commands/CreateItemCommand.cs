using System;
using Catalog.Entities.Dtos;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Commands;

public class CreateItemCommand : IRequest<CatalogItem>
{
    public CatalogItemInputDto InputItem { get; }

    public CreateItemCommand(CatalogItemInputDto inputItem)
    {
        InputItem = inputItem;
    }
}

