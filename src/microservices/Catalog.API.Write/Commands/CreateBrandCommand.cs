using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Write.Commands;

public class CreateBrandCommand(string name) : IRequest<CatalogBrand?>
{
    public string Name { get; } = name;
}

