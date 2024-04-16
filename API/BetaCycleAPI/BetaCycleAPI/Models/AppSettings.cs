namespace BetaCycleAPI.Models
{
    public class AppSettings
    {
        public Logging Logging { get; set; }
        public string AllowedHosts { get; set; }
        public Connectionstrings ConnectionStrings { get; set; }
    }

    public class Logging
    {
        public Loglevel LogLevel { get; set; }
    }

    public class Loglevel
    {
        public string Default { get; set; }
        public string MicrosoftAspNetCore { get; set; }
    }

    public class Connectionstrings
    {
        public string AdventureWorks { get; set; }
        public string Credentials { get; set; }
    }
}
