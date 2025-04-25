namespace Shared;

public static class GlobalConstants
{
    public const string Secret = "my_secret_key_that_is_at_least_32_bytes_long";

    public const string ValidIssuer = "https://localhost:5002";
    public const string ValidAudience = "https://localhost:5002";

    public const string UserRoleName = "User";
    public const string DefaultUserLogin = "user";
    public const string DefaultUserPassword = "qweqwe";

    public const string AdminRoleName = "Administrator";
    public const string DefaultAdminLogin = "admin";
    public const string DefaultAdminPassword = "root";

    public const string BadRequestErrorMessage = "Invalid client request";
    public const string UnauthorizedErrorMessage = "Incorrect username or password";
    public const string SecurityTokenErrorMessage = "Invalid token";

    public const string CorsPolicy = "EnableCORS";
    public const string ManagerRole = "Manager";

    public const string AuthServiceConnectionStringName = "AuthServiceConnection";
    public const string PlatformServiceConnectionStringName = "PlatformServiceConnection";
    public const string InMemoryDatabaseName = "InMem";
}