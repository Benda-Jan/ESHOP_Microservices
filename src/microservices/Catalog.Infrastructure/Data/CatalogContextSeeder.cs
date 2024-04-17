using System;
using System.Linq;
using Catalog.Entities.DbSet;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data;

public class CatalogContextSeeder
{
	private readonly CatalogContext _context;

    public CatalogContextSeeder(CatalogContext context)
    {
        _context = context;
    }

    public async Task SeedBrands()
    {
        var brands = new CatalogBrand[]{ "Apple", "Lenovo", "Dell" };
        if (await _context.CatalogBrands.AnyAsync())
        {
            await _context.CatalogBrands.ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }
        await _context.CatalogBrands.AddRangeAsync(brands);
        await _context.SaveChangesAsync();
    }

    public async Task SeedTypes()
    {
        var types = new CatalogType[] { "Laptop", "Phone", "Tablet" };
        if (await _context.CatalogTypes.AnyAsync())
        {
            await _context.CatalogTypes.ExecuteDeleteAsync();
            await _context.SaveChangesAsync();
        }
        await _context.CatalogTypes.AddRangeAsync(types);
        await _context.SaveChangesAsync();
    }

}

