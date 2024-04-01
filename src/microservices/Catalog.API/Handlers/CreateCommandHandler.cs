using Catalog.API.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.API.EventsHandling;
using System.Text.Json;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers
{
	public class CreateCommandHandler : IRequestHandler<CreateItemCommand, CatalogItem?>
	{
        private readonly CatalogRepository _catalogRepository;
        private readonly EventBusCatalogItemCreated _eventBusPublisher;

        public CreateCommandHandler(CatalogRepository catalogRepository, EventBusCatalogItemCreated eventBusPublisher)
        {
            _catalogRepository = catalogRepository;
            _eventBusPublisher = eventBusPublisher;
        }

        public async Task<CatalogItem?> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var catalogItem = await _catalogRepository.CreateItem(request.InputItem);
            if (catalogItem is not null)
                _eventBusPublisher.Publish(JsonSerializer.Serialize(catalogItem));

            return catalogItem;
        }
    }
}

