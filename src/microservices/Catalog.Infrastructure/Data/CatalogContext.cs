using System;
using Microsoft.EntityFrameworkCore;
using Catalog.Entities.DbSet;

namespace Catalog.Infrastructure.Data;

public class CatalogContext : DbContext
{
    public DbSet<CatalogType> CatalogTypes { get; set; } = null!;
    public DbSet<CatalogBrand> CatalogBrands { get; set; } = null!;
    public DbSet<CatalogItem> CatalogItems { get; set; } = null!;

    public CatalogContext(DbContextOptions<CatalogContext> options) : base(options)
	{
	}

    protected override void OnModelCreating(ModelBuilder builder)
    {
        //builder.Entity<CatalogType>()
        //    .HasIndex(u => u.Name)
        //    .IsUnique();

        //builder.Entity<CatalogBrand>()
        //    .HasIndex(u => u.Name)
        //    .IsUnique();
    }
}

