using System;
using Catalog.Entities.Dtos;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Write.Commands;

public class UpdateItemCommand(string id, CatalogItemInputDto inputItem) : IRequest<CatalogItem?>
{
    public string Id { get; } = id;
    public CatalogItemInputDto InputItem { get; } = inputItem;
}

