namespace Infrastructure.XmlStorage.Config;

/// <summary>
/// Represents authentication credentials loaded from config.xml.
/// </summary>
public sealed class AuthConfig
{
    /// <summary>
    /// Gets or sets the username for login.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
