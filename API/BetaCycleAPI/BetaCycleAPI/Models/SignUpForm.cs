namespace BetaCycleAPI.Models
{
    public class SignUpForm
    {
        public string? Title { get; set; }
        public string FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string LastName { get; set; }
        public string? Suffix { get; set; }
        public string? CompanyName { get; set; }
        public string? SalesPerson { get; set; }
        public string EmailAddress { get; set; }
        public string? Phone { get; set; }
        public string Password { get; set; }
        public bool IsMigrated { get; set; }
        public List<CustomerAddress>? CustomerAddresses { get; set; } = new List<CustomerAddress>();
    }
}
