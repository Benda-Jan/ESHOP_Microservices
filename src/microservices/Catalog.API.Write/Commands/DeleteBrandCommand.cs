using System;
using Catalog.Entities.Dtos;
using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Write.Commands;

public class DeleteBrandCommand(string name) : IRequest<CatalogBrand?>
{
    public string Name { get; } = name;
}

