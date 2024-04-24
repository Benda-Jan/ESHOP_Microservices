using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Write.Commands;

public class CreateTypeCommand : IRequest<CatalogType?>
{
    public string Name { get; }

    public CreateTypeCommand(string name)
    {
        Name = name;
    }
}

