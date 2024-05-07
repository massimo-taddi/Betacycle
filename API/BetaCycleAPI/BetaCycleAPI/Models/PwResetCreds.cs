namespace BetaCycleAPI.Models
{
    public class PwResetCreds
    {
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
        public string? email {  get; set; }
    }
}
