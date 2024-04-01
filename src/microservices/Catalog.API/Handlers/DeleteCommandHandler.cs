using Catalog.API.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.API.EventsHandling;
using System.Text.Json;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers
{
	public class DeleteCommandHandler : IRequestHandler<DeleteItemCommand, CatalogItem?>
	{
        private readonly CatalogRepository _catalogRepository;
        private readonly EventBusCatalogItemRemoved _eventBusPublisher;

        public DeleteCommandHandler(CatalogRepository catalogRepository, EventBusCatalogItemRemoved eventBusPublisher)
        {
            _catalogRepository = catalogRepository;
            _eventBusPublisher = eventBusPublisher;
        }

        public async Task<CatalogItem?> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var catalogItem = await _catalogRepository.RemoveItem(request.ItemId);

            if (catalogItem is not null)
            {
                _eventBusPublisher.Publish(JsonSerializer.Serialize(catalogItem));
                return catalogItem;
            }
            else
            {
                return null;
            }
        }
    }
}

