using Catalog.API.Write.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Infrastructure;

namespace Catalog.API.Write.Handlers
{
	public class CreateItemCommandHandler(ICatalogRepository catalogRepository, IWebHostEnvironment environment) : IRequestHandler<CreateItemCommand, CatalogItem?>
	{
        private readonly ICatalogRepository _catalogRepository = catalogRepository;
        private readonly IWebHostEnvironment _environment = environment;

        public Task<CatalogItem?> Handle(CreateItemCommand request, CancellationToken cancellationToken)
            => _catalogRepository.CreateItem(request.InputItem, _environment.WebRootPath ?? "");
    }
}

