using Catalog.API.Write.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.API.Write.EventsHandling;
using System.Text.Json;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers
{
	public class DeleteItemCommandHandler : IRequestHandler<DeleteItemCommand, CatalogItem?>
	{
        private readonly ICatalogRepository _catalogRepository;
        private readonly EventBusCatalogItemRemoved _eventBusPublisher;

        public DeleteItemCommandHandler(ICatalogRepository catalogRepository, EventBusCatalogItemRemoved eventBusPublisher)
        {
            _catalogRepository = catalogRepository;
            _eventBusPublisher = eventBusPublisher;
        }

        public async Task<CatalogItem?> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var catalogItem = await _catalogRepository.RemoveItem(request.ItemId);

            if (catalogItem != null)
                _eventBusPublisher.Publish(JsonSerializer.Serialize(catalogItem));
                
            return catalogItem;
        }
    }
}

