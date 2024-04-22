using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;

namespace BetaCycleAPI.Models;

/// <summary>
/// Customer information.
/// </summary>
public partial class Customer
{
    /// <summary>
    /// Primary key for Customer records.
    /// </summary>
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
    public string FirstName { get; set; } = null!;

    /// <summary>
    /// Middle name or middle initial of the person.
    /// </summary>
    public string? MiddleName { get; set; }

    /// <summary>
    /// Last name of the person.
    /// </summary>
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
    public DateTime ModifiedDate { get; set; }

    public bool IsMigrated { get; set; }

    public virtual ICollection<CustomerAddress> CustomerAddresses { get; set; } = new List<CustomerAddress>();

    public virtual ICollection<SalesOrderHeader> SalesOrderHeaders { get; set; } = new List<SalesOrderHeader>();


    public bool Validate()
    {
        return (this.Title.IsNullOrEmpty() || this.Title.Length <= 8) &&
                (this.FirstName.Length > 0 && this.FirstName.Length < 50) &&
                (this.MiddleName.IsNullOrEmpty() || this.MiddleName.Length <= 50) &&
                (this.LastName.Length > 0 && this.LastName.Length < 50) &&
                (this.Suffix.IsNullOrEmpty() || this.Suffix.Length < 10) &&
                (this.CompanyName.IsNullOrEmpty() || this.CompanyName.Length < 128) &&
                (this.SalesPerson.IsNullOrEmpty() || this.SalesPerson.Length < 256) &&
                (this.EmailAddress.IsNullOrEmpty() || this.EmailAddress.Length < 50) &&
                (this.Phone.IsNullOrEmpty() || this.Phone.Length < 25) &&
                (this.PasswordHash.Length > 0 && this.PasswordHash.Length < 128) &&
                (this.PasswordSalt.Length > 0 && this.PasswordSalt.Length < 10) &&
                (this.Rowguid.ToString().Length > 0) &&
                (this.ModifiedDate.CompareTo(new DateTime(1970, 1, 1)) > 0 && this.ModifiedDate.CompareTo(DateTime.Now) <= 0);
    }

}
