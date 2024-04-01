using Catalog.API.Commands;
using MediatR;
using Catalog.API.EventsHandling;
using System.Text.Json;
using Catalog.Infrastructure;

namespace Catalog.API.Handlers
{
	public class UpdateCommandHandler : IRequestHandler<UpdateItemCommand, bool>
	{
        private readonly CatalogRepository _catalogRepository;
        private readonly EventBusCatalogItemUpdated _eventBusPublisher;

        public UpdateCommandHandler(CatalogRepository catalogRepository, EventBusCatalogItemUpdated eventBusPublisher)
        {
            _catalogRepository = catalogRepository;
            _eventBusPublisher = eventBusPublisher;
        }

        public async Task<bool> Handle(UpdateItemCommand request, CancellationToken cancellationToken)
        {
            var result = await _catalogRepository.UpdateItem(request.Id, request.InputItem);

            if (result is not null)
            {
                _eventBusPublisher.Publish(JsonSerializer.Serialize(result));
            }

            return result is not null;
        }
    }
}

