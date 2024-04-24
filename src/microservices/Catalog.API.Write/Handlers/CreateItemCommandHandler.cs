using Catalog.API.Write.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.API.Write.EventsHandling;
using System.Text.Json;
using Catalog.Infrastructure;

namespace Catalog.API.Write.Handlers
{
	public class CreateItemCommandHandler : IRequestHandler<CreateItemCommand, CatalogItem?>
	{
        private readonly ICatalogRepository _catalogRepository;
        private readonly EventBusCatalogItemCreated _eventBusPublisher;
        private readonly IWebHostEnvironment _environment;

        public CreateItemCommandHandler(ICatalogRepository catalogRepository, EventBusCatalogItemCreated eventBusPublisher, IWebHostEnvironment environment)
        {
            _catalogRepository = catalogRepository;
            _eventBusPublisher = eventBusPublisher;
            _environment = environment;
        }

        public async Task<CatalogItem?> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var catalogItem = await _catalogRepository.CreateItem(request.InputItem, _environment.WebRootPath ?? "");
            if (catalogItem != null)
                _eventBusPublisher.Publish(JsonSerializer.Serialize(catalogItem));

            return catalogItem;
        }
    }
}

