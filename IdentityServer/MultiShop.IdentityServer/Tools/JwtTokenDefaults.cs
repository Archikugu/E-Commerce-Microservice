namespace MultiShop.IdentityServer.Tools;

public class JwtTokenDefaults
{
    public const string ValidAudience = "https://localhost:";
    public const string ValidIssuer = "https://localhost:";
    public const string Key = "This is my Multi Shop Secret key for authentication";
    public const int Expire = 60;
}
