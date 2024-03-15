using System;
using Catalog.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class MigrationExtension
{
	public static void ApplyMigrations(this IApplicationBuilder app)
	{
		using IServiceScope scope = app.ApplicationServices.CreateScope();

		using CatalogContext dbContext = scope.ServiceProvider.GetRequiredService<CatalogContext>();

		dbContext.Database.Migrate();
	}
}

