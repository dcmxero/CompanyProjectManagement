namespace WebApi.Models;

/// <summary>
/// Login request payload.
/// </summary>
public sealed class LoginRequest
{
    /// <summary>
    /// Gets or sets username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets password.
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
