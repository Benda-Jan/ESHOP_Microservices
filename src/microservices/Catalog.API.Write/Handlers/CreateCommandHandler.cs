using Catalog.API.Write.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.API.Write.EventsHandling;
using System.Text.Json;
using Catalog.Infrastructure;

namespace Catalog.API.Write.Handlers
{
	public class CreateCommandHandler : IRequestHandler<CreateItemCommand, CatalogItem?>
	{
        private readonly ICatalogRepository _catalogRepository;
        private readonly EventBusCatalogItemCreated _eventBusPublisher;

        public CreateCommandHandler(ICatalogRepository catalogRepository, EventBusCatalogItemCreated eventBusPublisher)
        {
            _catalogRepository = catalogRepository;
            _eventBusPublisher = eventBusPublisher;
        }

        public async Task<CatalogItem?> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var catalogItem = await _catalogRepository.CreateItem(request.InputItem);
            if (catalogItem != null)
                _eventBusPublisher.Publish(JsonSerializer.Serialize(catalogItem));

            return catalogItem;
        }
    }
}

