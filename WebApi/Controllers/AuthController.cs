using Application.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using WebApi.Auth;
using WebApi.Models;

namespace WebApi.Controllers;

/// <summary>
/// Provides authentication endpoints for obtaining JWT tokens.
/// </summary>
/// <remarks>
/// This controller handles user authentication using credentials defined in the XML configuration.
/// It validates the username and password, and upon success returns a signed JWT bearer token
/// which can be used to authorize access to other API endpoints.
/// </remarks>
[ApiController]
[Route("api/[controller]")]
[Tags("Auth")]
public sealed class AuthController(
    IAuthService auth,
    IOptions<JwtSettings> jwtOptions) : ControllerBase
{
    /// <summary>
    /// Authenticates the user and returns a JWT token if the credentials are valid.
    /// </summary>
    /// <remarks>
    /// This endpoint accepts a username and password in the request body, validates them against
    /// the credentials stored in <c>Config/config.xml</c>, and issues a signed JSON Web Token (JWT).
    /// The token includes user identification claims and can be used for authenticated API requests.
    /// </remarks>
    /// <param name="request">Login credentials containing <c>Username</c> and <c>Password</c>.</param>
    /// <returns>
    /// Returns a JSON object containing the generated JWT token and its expiration timestamp (UTC).  
    /// - <b>200 OK</b>: Login successful, token returned.  
    /// - <b>400 Bad Request</b>: Missing or empty username/password.  
    /// - <b>401 Unauthorized</b>: Invalid credentials.
    /// </returns>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (request == null || string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { error = "Username and password are required." });
        }

        if (!auth.ValidateCredentials(request.Username, request.Password))
        {
            return Unauthorized(new { error = "Invalid credentials." });
        }

        var jwt = jwtOptions.Value;
        var now = DateTime.UtcNow;
        var expires = now.AddHours(jwt.ExpiresHours);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username.Trim()),
            new Claim(ClaimTypes.Name, request.Username.Trim()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new Claim(JwtRegisteredClaimNames.Iat,
                ((long)(now - DateTime.UnixEpoch).TotalSeconds).ToString(),
                ClaimValueTypes.Integer64)
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Convert.FromBase64String(jwt.Key)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            tokenType = "Bearer",
            expiresAtUtc = expires
        });
    }
}
