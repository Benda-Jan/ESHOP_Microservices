using System;

namespace Catalog.Entities.DbSet;

public class CatalogItem : BaseEntity
{ 
	public string Name { get; set; } = String.Empty;
	public string Description { get; set; } = String.Empty;
	public decimal Price { get; set; }
	public string PictureFilename { get; set; } = String.Empty;
	public string PictureUri { get; set; } = String.Empty;
	public int CatalogTypeId { get; set; }
	public int CatalogBrandId { get; set; }
	public int AvailableStock { get; set; }
	public int RestockThreshold { get; set; }
	public int MaxStockThreshold { get; set; }
	public bool OnReorder { get; set; }

	public virtual CatalogType? CatalogType { get; set; }
    public virtual CatalogBrand? CatalogBrand { get; set; }
}

