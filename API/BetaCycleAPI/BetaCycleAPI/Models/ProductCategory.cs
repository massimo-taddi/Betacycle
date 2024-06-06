using System.ComponentModel.DataAnnotations;

namespace BetaCycleAPI.Models;

/// <summary>
/// High-level product categorization.
/// </summary>
public partial class ProductCategory
{
    /// <summary>
    /// Primary key for ProductCategory records.
    /// </summary>

    [Key]
    public int ProductCategoryId { get; set; }

    /// <summary>
    /// Product category identification number of immediate ancestor category. Foreign key to ProductCategory.ProductCategoryID.
    /// </summary>
    public int? ParentProductCategoryId { get; set; }

    /// <summary>
    /// Category description.
    /// </summary>
   
    [MaxLength(50, ErrorMessage = "Massimo 50 caratteri"), MinLength(4, ErrorMessage = "Minimo 4 caratteri")]
    public string Name { get; set; } = null!;

    /// <summary>
    /// ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.
    /// </summary>
    public Guid Rowguid { get; set; }

    /// <summary>
    /// Date and time the record was last updated.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    public bool Discontinued { get; set; } = false;

    public virtual ICollection<ProductCategory> InverseParentProductCategory { get; set; } = new List<ProductCategory>();

    public virtual ProductCategory? ParentProductCategory { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
