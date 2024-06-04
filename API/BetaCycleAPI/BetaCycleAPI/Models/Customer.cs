using System.ComponentModel.DataAnnotations;

namespace BetaCycleAPI.Models;

/// <summary>
/// Customer information.
/// </summary>
public partial class Customer
{
    /// <summary>
    /// Primary key for Customer records.
    /// </summary>
    [Key]
    public int CustomerId { get; set; }

    /// <summary>
    /// 0 = The data in FirstName and LastName are stored in western style (first name, last name) order.  1 = Eastern style (last name, first name) order.
    /// </summary>
    public bool NameStyle { get; set; }

    /// <summary>
    /// A courtesy title. For example, Mr. or Ms.
    /// </summary>

    public string? Title { get; set; }

    /// <summary>
    /// First name of the person.
    /// </summary>
    [Required]
    [MaxLength(50, ErrorMessage = "Massimo 50 caratteri"), MinLength(1, ErrorMessage = "Minimo 1 carattere")]
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Middle name or middle initial of the person.
    /// </summary>
    /// 
    [MaxLength(50, ErrorMessage = "Massimo 50 caratteri")]

    public string? MiddleName { get; set; }

    /// <summary>
    /// Last name of the person.
    /// </summary>
    [Required]
    [MaxLength(50, ErrorMessage = "Massimo 50 caratteri"), MinLength(4, ErrorMessage = "Minimo 1 carattere")]
    public string LastName { get; set; } = null!;

    /// <summary>
    /// Surname suffix. For example, Sr. or Jr.
    /// </summary>
    public string? Suffix { get; set; }

    /// <summary>
    /// The customer&apos;s organization.
    /// </summary>
    public string? CompanyName { get; set; }

    /// <summary>
    /// The customer&apos;s sales person, an employee of AdventureWorks Cycles.
    /// </summary>
    public string? SalesPerson { get; set; }

    /// <summary>
    /// E-mail address for the person.
    /// </summary>
    [Required]
    [DataType(DataType.EmailAddress)]
    [EmailAddress]
    [MaxLength(128, ErrorMessage = "Massimo 128 caratteri"), MinLength(4, ErrorMessage = "Minimo 4 caratteri")]
    public string? EmailAddress { get; set; }

    /// <summary>
    /// Phone number associated with the person.
    /// </summary>
    public string? Phone { get; set; }

    /// <summary>
    /// Password for the e-mail account.
    /// </summary>
    public string PasswordHash { get; set; } = null!;

    /// <summary>
    /// Random value concatenated with the password string before the password is hashed.
    /// </summary>
    public string PasswordSalt { get; set; } = null!;

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

    public bool IsMigrated { get; set; }

    public int? ShoppingCartId { get; set; }

    public int? CustomerReviewId { get; set; }

    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; } = new List<SalesOrderHeader>();

    public virtual ShoppingCart? ShoppingCart { get; set; }

    public virtual CustomerReview? CustomerReview { get; set; }

}
