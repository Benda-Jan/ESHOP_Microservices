using Catalog.API.Write.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.API.Write.EventsHandling;
using System.Text.Json;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers
{
	public class DeleteBrandCommandHandler(ICatalogRepository catalogRepository) : IRequestHandler<DeleteBrandCommand, CatalogBrand?>
	{
        private readonly ICatalogRepository _catalogRepository = catalogRepository;
        
        public Task<CatalogBrand?> Handle(DeleteBrandCommand request, CancellationToken cancellationToken)
            => _catalogRepository.RemoveBrand(request.Name);
    }
}

