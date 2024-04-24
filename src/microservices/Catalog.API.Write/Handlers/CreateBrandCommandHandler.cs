using Catalog.API.Write.Commands;
using Catalog.Entities.DbSet;
using MediatR;
using Catalog.Infrastructure;

namespace Catalog.API.Write.Handlers
{
	public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, CatalogBrand?>
	{
        private readonly ICatalogRepository _catalogRepository;

        public CreateBrandCommandHandler(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        public Task<CatalogBrand?> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
            => _catalogRepository.CreateBrand(request.Name);
    }
}

