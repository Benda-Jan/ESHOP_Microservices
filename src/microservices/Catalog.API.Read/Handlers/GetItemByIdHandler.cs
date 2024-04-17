using Catalog.API.Read.Queries;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Infrastructure;

namespace Catalog.API.Read.Handlers;

public class GetItemByIdHandler : IRequestHandler<GetItemByIdQuery, CatalogItem?>
{
    private readonly ICatalogRepository _catalogRepository;

    public GetItemByIdHandler(ICatalogRepository catalogRepository)
    {
        _catalogRepository = catalogRepository;
    }

    public Task<CatalogItem?> Handle(GetItemByIdQuery request, CancellationToken cancellationToken)
        => _catalogRepository.GetItemById(request.Id);
        
}

