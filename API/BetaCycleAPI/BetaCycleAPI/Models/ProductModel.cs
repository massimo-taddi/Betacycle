using System;
using System.Collections.Generic;

namespace BetaCycleAPI.Models;

public partial class ProductModel
{
    public int ProductModelId { get; set; }

    public string Name { get; set; } = null!;

    public string? CatalogDescription { get; set; }

    public Guid Rowguid { get; set; }

    public DateTime ModifiedDate { get; set; }

    public virtual ICollection<ProductModelProductDescription> ProductModelProductDescriptions { get; set; } = new List<ProductModelProductDescription>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();

    public bool Validate()
    {
        return (this.Name.Length < 50) &&
        
        //(this.CatalogDescription.MetodoCheParsaXMLinstringa())
        //this.rowguid
        (this.ModifiedDate > (new DateTime(1970, 1, 1)) && this.ModifiedDate < DateTime.Now);
    }

}
