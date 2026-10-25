using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Auth.Application.Common.Interfaces;
using Auth.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Infrastructure.Authentication;

// Concrete implementation of JWT and Refresh Token generation
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles)
    {
        // Retrieve JWT configurations from appsettings.json
        var secretKey = _configuration["JwtSettings:Secret"]!;
        var issuer = _configuration["JwtSettings:Issuer"];
        var audience = _configuration["JwtSettings:Audience"];
        var expirationMinutes = double.Parse(_configuration["JwtSettings:AccessTokenExpirationMinutes"]!);

        // Create security key and signing credentials using HMAC SHA256
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Build standard JWT Claims
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new("firstName", user.FirstName),
            new("lastName", user.LastName),
            new("userType", user.UserType.ToString())
        };

        // Add user roles to claims list
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Create security token descriptor
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(expirationMinutes),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = credentials
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var securityToken = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(securityToken);
    }

    public RefreshToken GenerateRefreshToken(Guid userId, string jwtId)
    {
        // Generate a cryptographically secure 64-byte random string
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var expirationDays = double.Parse(_configuration["JwtSettings:RefreshTokenExpirationDays"]!);

        // Construct and return the RefreshToken entity
        return new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            JwtId = jwtId,
            Token = Convert.ToBase64String(randomNumber),
            CreatedAtUtc = DateTime.UtcNow,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(expirationDays),
            IsUsed = false,
            IsRevoked = false
        };
    }

    public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var secretKey = _configuration["JwtSettings:Secret"]!;
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
            ValidateLifetime = false // Ignore lifetime expiration to read claims from expired access tokens
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                return null;
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }
}