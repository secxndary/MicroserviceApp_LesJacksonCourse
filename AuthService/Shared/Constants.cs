namespace AuthService.Shared;

public static class Constants
{
    public const string Secret = "my_secret_key_that_is_at_least_32_bytes_long";

    public const string ValidIssuer = "https://localhost:5001";
    public const string ValidAudience = "https://localhost:5001";
    public const string DefaultUserName = "johndoe";
    public const string DefaultPassword = "def@123";

    public const string CorsPolicy = "EnableCORS";

    public const string ConnectionStringName = "AuthServiceConnection";
    public const string InMemoryDatabaseName = "InMem";

    public const string BadRequestErrorMessage = "Invalid client request";
    public const string UnauthorizedErrorMessage = "Incorrect username or password";
    public const string SecurityTokenErrorMessage = "Invalid token";

    public const string ManagerRole = "Manager";
}