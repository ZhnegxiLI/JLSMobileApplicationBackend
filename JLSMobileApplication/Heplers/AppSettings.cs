namespace JLSMobileApplication.Heplers
{
    public class AppSettings
    {
        public string SendGridKey { get; set; }
        public string SendGridUser { get; set; }
        public string EmailAccount { get; set; }
        public string EmailPassword { get; set; }
        public string MyAllowSpecificOrigins { get; set; }

        public string JwtSecret { get; set; }
        public string JwtIssuer { get; set; }
        public string JwtAudience { get; set; }
    }
}
