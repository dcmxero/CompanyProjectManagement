using Infrastructure.XmlStorage.Config;
using System.Security.Cryptography;
using System.Text;

namespace Application.Auth;

/// <summary>
/// Authentication logic using credentials from XmlConfigProvider.
/// </summary>
public sealed class AuthService(XmlConfigProvider config) : IAuthService
{
    public bool ValidateCredentials(string username, string password)
    {
        // Basic input hardening
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password)) return false;

        // Normalize input
        var inputName = username.Trim();

        // Expected
        var expectedName = config.Auth.Username?.Trim() ?? string.Empty;

        // Constant-time compare for username to reduce timing side-channel (even if low-impact here)
        if (!SafeEquals(inputName, expectedName))
        {
            return false;
        }

        return SafeEquals(password, config.Auth.Password!);
    }

    private static bool SafeEquals(string a, string b)
    {
        var ab = Encoding.UTF8.GetBytes(a);
        var bb = Encoding.UTF8.GetBytes(b);
        if (ab.Length != bb.Length) return false;
        return CryptographicOperations.FixedTimeEquals(ab, bb);
    }
}
