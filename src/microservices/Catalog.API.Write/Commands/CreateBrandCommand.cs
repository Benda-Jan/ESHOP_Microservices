using Catalog.Entities.DbSet;
using MediatR;

namespace Catalog.API.Write.Commands;

public class CreateBrandCommand : IRequest<CatalogBrand?>
{
    public string Name { get; }

    public CreateBrandCommand(string name)
    {
        Name = name;
    }
}

