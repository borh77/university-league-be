namespace Solution.Identity.Infrastructure;

// Konfiguracija tokena se cita iz env varijabli, isto kao i konekcija ka bazi
public static class JwtSettingsBuilder
{
    public static string Key => Environment.GetEnvironmentVariable("JWT_KEY")
        ?? "dev-only-key-min-32-chars-long-change-me";

    public static string Issuer => Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "unileague";

    public static string Audience => Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "unileague-web";

    public static int ExpiryHours =>
        int.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRY_HOURS"), out var h) ? h : 12;
}
