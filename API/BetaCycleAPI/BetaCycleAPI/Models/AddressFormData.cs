namespace BetaCycleAPI.Models
{
    public class AddressFormData
    {
        /// <summary>
        /// First street address line.
        /// </summary>
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Second street address line.
        /// </summary>
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// Name of the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Name of state or province.
        /// </summary>
        public string StateProvince { get; set; }

        public string CountryRegion { get; set; }

        /// <summary>
        /// Postal code for the street address.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// The kind of Address. One of: Archive, Billing, Home, Main Office, Primary, Shipping
        /// </summary>
        public string AddressType { get; set; }

        /// <summary>
        /// Date and time the record was last updated.
        /// </summary>
        public DateTime ModifiedDate { get; set; }
    }
}
