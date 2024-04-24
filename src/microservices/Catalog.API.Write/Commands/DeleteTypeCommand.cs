using System;
using Catalog.Entities.Dtos;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Write.Commands;

public class DeleteTypeCommand : IRequest<CatalogType?>
{
    public string Name { get; }

    public DeleteTypeCommand(string name)
    {
        Name = name;
    }
}

