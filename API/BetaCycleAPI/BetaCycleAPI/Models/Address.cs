using System.ComponentModel.DataAnnotations;

namespace BetaCycleAPI.Models;

/// <summary>
/// Street address information for customers.
/// </summary>
public partial class Address
{
    /// <summary>
    /// Primary key for Address records.
    /// </summary>
    [Key]
    public int AddressId { get; set; }

    /// <summary>
    /// First street address line.
    /// </summary>
    /// 
    [Required]
    [MaxLength(70, ErrorMessage = "Massimo 70 caratteri"), MinLength(6, ErrorMessage = "Minimo 6 caratteri")]
    public string AddressLine1 { get; set; } = null!;

    /// <summary>
    /// Second street address line.
    /// </summary>
    /// 
    [MaxLength(70, ErrorMessage = "Massimo 70 caratteri")]

    public string? AddressLine2 { get; set; }

    /// <summary>
    /// Name of the city.
    /// </summary>
    /// 
    [Required]
    [MaxLength(30, ErrorMessage = "Massimo 30 caratteri"), MinLength(4, ErrorMessage = "Minimo 4 caratteri")]
    public string City { get; set; } = null!;

    /// <summary>
    /// Name of state or province.
    /// </summary>
    /// 
    [Required]
    [MaxLength(30, ErrorMessage = "Massimo 30 caratteri"), MinLength(4, ErrorMessage = "Minimo 4 caratteri")]

    public string StateProvince { get; set; } = null!;

    [Required]
    [MaxLength(30, ErrorMessage = "Massimo 30 caratteri"), MinLength(4, ErrorMessage = "Minimo 4 caratteri")]
    public string CountryRegion { get; set; } = null!;

    /// <summary>
    /// Postal code for the street address.
    /// </summary>
    [Required]
    [MaxLength(5, ErrorMessage = "Massimo 5 caratteri"), MinLength(5, ErrorMessage = "Minimo 5 caratteri")]
    public string PostalCode { get; set; } = null!;

    /// <summary>
    /// ROWGUIDCOL number uniquely identifying the record. Used to support a merge replication sample.
    /// </summary>
    public Guid Rowguid { get; set; }

    /// <summary>
    /// Date and time the record was last updated.
    /// </summary>
    public DateTime ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<SalesOrderHeader> SalesOrderHeaderBillToAddresses { get; set; } = new List<SalesOrderHeader>();

    public virtual ICollection<SalesOrderHeader> SalesOrderHeaderShipToAddresses { get; set; } = new List<SalesOrderHeader>();
}
