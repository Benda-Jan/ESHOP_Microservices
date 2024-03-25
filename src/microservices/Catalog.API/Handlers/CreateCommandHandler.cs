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
	public class CreateCommandHandler : IRequestHandler<CreateItemCommand, CatalogItem>
	{
        private readonly CatalogContext _catalogContext;
        private readonly EventBusCatalogItemCreated _eventBusPublisher;

        public CreateCommandHandler(CatalogContext catalogContext, EventBusCatalogItemCreated eventBusPublisher)
        {
            _catalogContext = catalogContext;
            _eventBusPublisher = eventBusPublisher;
        }

        public async Task<CatalogItem> Handle(CreateItemCommand request, CancellationToken cancellationToken)
        {
            var alreadyExists = await _catalogContext.CatalogItems.AnyAsync(x => x.Name == request.InputItem.Name);
            if (alreadyExists is not false)
                throw new BadHttpRequestException("Catalog item already exists");

            var catalogType = await _catalogContext.CatalogTypes.Where(x => x.Name == request.InputItem.CatalogTypeName).FirstOrDefaultAsync()
                ?? throw new BadHttpRequestException($"Catalog type {request.InputItem.CatalogTypeName} does not exist");

            var catalogBrand = await _catalogContext.CatalogBrands.Where(x => x.Name == request.InputItem.CatalogBrandName).FirstOrDefaultAsync()
                ?? throw new BadHttpRequestException($"Catalog brand {request.InputItem.CatalogBrandName} does not exist");

            var catalogItem = new CatalogItem
            {
                Name = request.InputItem.Name,
                Description = request.InputItem.Description,
                Price = request.InputItem.Price,
                PictureFilename = request.InputItem.PictureFilename,
                PictureUri = request.InputItem.PictureUri,
                CatalogTypeId = catalogType.Id,
                CatalogType = catalogType,
                CatalogBrandId = catalogBrand.Id,
                CatalogBrand = catalogBrand,
                AvailableStock = request.InputItem.AvailableStock,
                RestockThreshold = request.InputItem.RestockThreshold,
                MaxStockThreshold = request.InputItem.MaxStockThreshold,
                OnReorder = request.InputItem.OnReorder
            };

            await _catalogContext.AddAsync(catalogItem);
            await _catalogContext.SaveChangesAsync();

            _eventBusPublisher.Publish(JsonSerializer.Serialize(catalogItem));

            return catalogItem;
        }
    }
}

