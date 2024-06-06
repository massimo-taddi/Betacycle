using System.ComponentModel.DataAnnotations;

namespace BetaCycleAPI.Models;

public partial class ProductModel
{
    [Key]
    public int ProductModelId { get; set; }

    [MaxLength(50, ErrorMessage = "Massimo 50 caratteri"), MinLength(4, ErrorMessage = "Minimo 4 caratteri")]
    public string Name { get; set; } = null!;

    [MaxLength(50, ErrorMessage = "Massimo 50 caratteri"), MinLength(4, ErrorMessage = "Minimo 4 caratteri")]
    public string? CatalogDescription { get; set; }

    public Guid Rowguid { get; set; }

    public DateTime ModifiedDate { get; set; }

    public bool Discontinued { get; set; } = false;

    public virtual ICollection<ProductModelProductDescription> ProductModelProductDescriptions { get; set; } = new List<ProductModelProductDescription>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
