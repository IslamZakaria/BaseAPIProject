namespace Core.Settings
{
    public class RateLimitingSettings
    {
        public const string RateLimitSection = "RateLimitingSettings";
        public string RateLimitMessage { get; set; }
        public string ThirdPartiesPolicy { get; set; }
        public int ThirdPartyWindowLimit { get; set; }
        public int ThirdPartyWindowPeriod { get; set; }
        public string DefaultGlobalTokenBucketKey { get; set; }
        public string GlobalTokenBucketHeaderName { get; set; }
        public int GlobalTokenReplenishmentPeriod { get; set; }
        public int GlobalTokensLimit { get; set; }
        public int GlobalTokensPerPeriod { get; set; }
    }
}
