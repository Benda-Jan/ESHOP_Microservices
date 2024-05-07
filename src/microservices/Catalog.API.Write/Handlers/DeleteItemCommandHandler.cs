using Catalog.API.Write.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.API.Write.EventsHandling;
using System.Text.Json;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers
{
	public class DeleteItemCommandHandler(ICatalogRepository catalogRepository, EventBusCatalogItemDeleted eventBusPublisher) : IRequestHandler<DeleteItemCommand, CatalogItem?>
	{
        private readonly ICatalogRepository _catalogRepository = catalogRepository;
        private readonly EventBusCatalogItemDeleted _eventBusPublisher = eventBusPublisher;

        public async Task<CatalogItem?> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var catalogItem = await _catalogRepository.RemoveItem(request.ItemId);

            if (catalogItem != null)
                _eventBusPublisher.Publish(JsonSerializer.Serialize(catalogItem));
                
            return catalogItem;
        }
    }
}

