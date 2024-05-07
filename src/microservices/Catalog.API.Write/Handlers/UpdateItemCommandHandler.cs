using Catalog.API.Write.Commands;
using MediatR;
using Catalog.API.Write.EventsHandling;
using System.Text.Json;
using Catalog.Infrastructure;
using Catalog.Entities.DbSet;

namespace Catalog.API.Handlers
{
	public class UpdateItemCommandHandler(ICatalogRepository catalogRepository, EventBusCatalogItemUpdated eventBusPublisher) : IRequestHandler<UpdateItemCommand, CatalogItem?>
	{
        private readonly ICatalogRepository _catalogRepository = catalogRepository;
        private readonly EventBusCatalogItemUpdated _eventBusPublisher = eventBusPublisher;

        public async Task<CatalogItem?> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var result = await _catalogRepository.UpdateItem(request.Id, request.InputItem);

            if (result != null)
                _eventBusPublisher.Publish(JsonSerializer.Serialize(result));

            return result;
        }
    }
}

