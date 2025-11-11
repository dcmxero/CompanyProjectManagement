namespace Application.Auth;

/// <summary>
/// Authentication contract.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Validates username and password against configured credentials.
    /// </summary>
    /// <param name="username">User name.</param>
    /// <param name="password">Password.</param>
    /// <returns>True if valid.</returns>
    bool ValidateCredentials(string username, string password);
}
