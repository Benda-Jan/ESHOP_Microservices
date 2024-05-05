using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Catalog.Entities.Dtos;

public class CatalogItemInputDto
{
    public string Name { get; set; } = String.Empty;
    public string Description { get; set; } = String.Empty;
    public decimal Price { get; set; }
    public IFormFile? File { get; set; }
    public string PictureFilename { get; set; } = String.Empty;
    public string PictureUri { get; set; } = String.Empty;
    public string CatalogTypeName { get; set; } = String.Empty;
    public string CatalogBrandName { get; set; } = String.Empty;
    public int AvailableStock { get; set; }
    public int RestockThreshold { get; set; }
    public int MaxStockThreshold { get; set; }
    public bool OnReorder { get; set; }
}

