using Catalog.API.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers;

public class GetItemByIdHandler : IRequestHandler<GetItemByIdQuery, CatalogItem?>
{
    private readonly CatalogRepository _catalogRepository;

    public GetItemByIdHandler(CatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public async Task<CatalogItem?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
    {
        var result = await _catalogRepository.GetItemById(request.Id);

        return result;
    }
        

}

