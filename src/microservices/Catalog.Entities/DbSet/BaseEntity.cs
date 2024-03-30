using System;

namespace Catalog.Entities.DbSet;

public abstract class BaseEntity
{
	public required string Id { get; set; }
	public DateTime DateAdded { get; set; }
	public DateTime DateUpdated { get; set; }
}

