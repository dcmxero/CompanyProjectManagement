namespace WebApi.Auth;

/// <summary>
/// Represents JWT configuration settings bound from appsettings.json.
/// </summary>
public sealed class JwtSettings
{
    /// <summary>
    /// Gets or sets the base64-encoded signing key.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the token issuer.
    /// </summary>
    public string Issuer { get; set; } = "CompanyProjectManagement";

    /// <summary>
    /// Gets or sets the token audience.
    /// </summary>
    public string Audience { get; set; } = "CompanyProjectManagement";

    /// <summary>
    /// Gets or sets the token lifetime in hours.
    /// </summary>
    public int ExpiresHours { get; set; } = 8;
}
