using Catalog.API.Write.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.API.Write.EventsHandling;
using System.Text.Json;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers
{
	public class DeleteTypeCommandHandler : IRequestHandler<DeleteTypeCommand, CatalogType?>
	{
        private readonly ICatalogRepository _catalogRepository;

        public DeleteTypeCommandHandler(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        public Task<CatalogType?> Handle(DeleteTypeCommand request, CancellationToken cancellationToken)
            => _catalogRepository.RemoveType(request.Name);
    }
}

