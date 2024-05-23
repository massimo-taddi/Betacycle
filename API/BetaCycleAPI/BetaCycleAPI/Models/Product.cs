using System.ComponentModel.DataAnnotations;

namespace BetaCycleAPI.Models;

/// <summary>
/// Products sold or used in the manfacturing of sold products.
/// </summary>
public partial class Product
{
    /// <summary>
    /// Primary key for Product records.
    /// </summary>
    /// 
    [Key]
    public int ProductId { get; set; }

    /// <summary>
    /// Name of the product.
    /// </summary>
    /// 
    [Required]
    [MaxLength(50, ErrorMessage = "Massimo 50 caratteri"), MinLength(4, ErrorMessage = "Minimo 4 caratteri")]
    public string Name { get; set; } = null;

    /// <summary>
    /// Unique product identification number.
    /// </summary>
    ///
    [Required]
    public string ProductNumber { get; set; } = null!;

    /// <summary>
    /// Product color.
    /// </summary>
    /// 
    [MaxLength(50, ErrorMessage = "Massimo 50 caratteri")]
    public string? Color { get; set; }

    /// <summary>
    /// Standard cost of the product.
    /// </summary>
    /// 
    [Required]
    public decimal StandardCost { get; set; }

    /// <summary>
    /// Selling price.
    /// </summary>
    /// 
    [Required]
    public decimal ListPrice { get; set; }

    /// <summary>
    /// Product size.
    /// </summary>
    public string? Size { get; set; }

    /// <summary>
    /// Product weight.
    /// </summary>
    public decimal? Weight { get; set; }

    /// <summary>
    /// Product is a member of this product category. Foreign key to ProductCategory.ProductCategoryID. 
    /// </summary>
    /// 

    public int? ProductCategoryId { get; set; }

    /// <summary>
    /// Product is a member of this product model. Foreign key to ProductModel.ProductModelID.
    /// </summary>
    /// 

    public int? ProductModelId { get; set; }

    /// <summary>
    /// Date the product was available for sale.
    /// </summary>
    public DateTime SellStartDate { get; set; }

    /// <summary>
    /// Date the product was no longer available for sale.
    /// </summary>
    public DateTime? SellEndDate { get; set; }

    /// <summary>
    /// Date the product was discontinued.
    /// </summary>
    public DateTime? DiscontinuedDate { get; set; }

    /// <summary>
    /// Small image of the product.
    /// </summary>
    /// 
    public byte[]? ThumbNailPhoto { get; set; }

    /// <summary>
    /// Small image file name.
    /// </summary>
    /// 
    public string? ThumbnailPhotoFileName { get; set; }

    /// <summary>
    /// ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.
    /// </summary>
    public Guid Rowguid { get; set; }

    /// <summary>
    /// Date and time the record was last updated.
    /// </summary>
    /// 
    [Required]
    public DateTime ModifiedDate { get; set; }

    /// <summary>
    /// Large image of the product.
    /// </summary>
    /// 
    public byte[]? LargePhoto { get; set; }

    /// <summary>
    /// Large image file name.
    /// </summary>
    /// 
    public string? LargePhotoFileName { get; set; }

    public bool OnSale { get; set; }

    public virtual ProductCategory? ProductCategory { get; set; }

    public virtual ProductModel? ProductModel { get; set; }

    public virtual ICollection<SalesOrderDetail> SalesOrderDetails { get; set; } = new List<SalesOrderDetail>();

    public virtual ICollection<ShoppingCartItem> ShoppingCartItems { get; set; } = new List<ShoppingCartItem>();
}
