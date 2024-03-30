﻿using System;
using Catalog.Entities.Dtos;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Commands;

public class UpdateItemCommand : IRequest<bool>
{
    public string Id { get; }
    public CatalogItemInputDto InputItem { get; }

    public UpdateItemCommand(string id, CatalogItemInputDto inputItem)
    {
        Id = id;
        InputItem = inputItem;
    }
}
