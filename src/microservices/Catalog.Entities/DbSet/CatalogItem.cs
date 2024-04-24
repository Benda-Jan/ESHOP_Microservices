using System;

namespace Catalog.Entities.DbSet;

public class CatalogItem : BaseEntity
{ 
	public required string Name { get; set; } = String.Empty;
	public required string Description { get; set; } = String.Empty;
	public decimal Price { get; set; }
	public string? PictureFilename { get; set; }
	public string? PictureUri { get; set; }
	public required string CatalogTypeId { get; set; }
	public required string CatalogBrandId { get; set; }
	public int AvailableStock { get; set; }
	public int RestockThreshold { get; set; }
	public int MaxStockThreshold { get; set; }
	public bool OnReorder { get; set; }

	public virtual CatalogType? CatalogType { get; set; }
    public virtual CatalogBrand? CatalogBrand { get; set; }
}

public class CatalogItemDeserializer
{
	public required string CatalogItemId { get; set; }
	public required int Quantity { get; set; }
}

public class CatalogItemSerializer
{
	public required string Id { get; set; }
	public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
}