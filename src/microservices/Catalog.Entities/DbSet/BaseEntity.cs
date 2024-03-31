using System;

namespace Catalog.Entities.DbSet;

public abstract class BaseEntity
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public DateTime DateAdded { get; set;}//init; } = DateTime.Now;
    public DateTime DateUpdated { get; set; }

}