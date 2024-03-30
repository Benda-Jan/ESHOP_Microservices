using System;
using Catalog.API.Commands;
using Catalog.Infrastructure.Data;
using Catalog.Entities.DbSet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using EventBus;
using Catalog.API.EventsHandling;
using System.Text.Json;

namespace Catalog.API.Handlers
{
	public class DeleteCommandHandler : IRequestHandler<DeleteItemCommand, CatalogItem?>
	{
        private readonly CatalogContext _catalogContext;
        private readonly EventBusCatalogItemRemoved _eventBusPublisher;

        public DeleteCommandHandler(CatalogContext catalogContext, EventBusCatalogItemRemoved eventBusPublisher)
        {
            _catalogContext = catalogContext;
            _eventBusPublisher = eventBusPublisher;
        }

        public async Task<CatalogItem?> Handle(DeleteItemCommand request, CancellationToken cancellationToken)
        {
            var catalogItem = await _catalogContext.CatalogItems.SingleOrDefaultAsync(x => x.Id == request.ItemId);

            if (catalogItem is not null)
            {
                _eventBusPublisher.Publish(JsonSerializer.Serialize(catalogItem));
                _catalogContext.Remove(catalogItem);
                await _catalogContext.SaveChangesAsync();
            }
            return catalogItem;
        }
    }
}

