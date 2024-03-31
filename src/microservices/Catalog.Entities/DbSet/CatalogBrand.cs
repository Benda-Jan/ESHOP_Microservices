using System;

namespace Catalog.Entities.DbSet;

public class CatalogBrand : BaseEntity
{
	public string Name { get; set; } = string.Empty;

	public static implicit operator CatalogBrand(string name) => new CatalogBrand { Name = name };
}

