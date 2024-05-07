using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Write.Commands;

public class CreateTypeCommand(string name) : IRequest<CatalogType?>
{
    public string Name { get; } = name;
}

