using System;
using System.Net.Mail;

namespace Catalog.Entities.DbSet;

public class CatalogType : BaseEntity
{
    public string Name { get; set; } = string.Empty;

    public static implicit operator CatalogType(string name) => new CatalogType { Name = name};

}

