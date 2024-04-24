using System;
using Catalog.Entities.Dtos;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Write.Commands;

public class DeleteBrandCommand : IRequest<CatalogBrand?>
{
    public string Name { get; }

    public DeleteBrandCommand(string name)
    {
        Name = name;
    }
}

