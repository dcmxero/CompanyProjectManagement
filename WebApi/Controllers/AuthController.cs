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

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(IAuthService auth, IOptions<JwtSettings> jwtOptions) : ControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
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
            new Claim(JwtRegisteredClaimNames.Iat, ((long)(now - DateTime.UnixEpoch).TotalSeconds).ToString(), ClaimValueTypes.Integer64)
        };

        var creds = new SigningCredentials(new SymmetricSecurityKey(Convert.FromBase64String(jwt.Key)), SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt.Issuer,
            audience: jwt.Audience,
            claims: claims,
            notBefore: now,
            expires: expires,
            signingCredentials: creds);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { token = tokenString, tokenType = "Bearer", expiresAtUtc = expires });
    }
}
