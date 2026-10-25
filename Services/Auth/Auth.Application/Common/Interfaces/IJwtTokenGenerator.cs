using Auth.Domain.Entities;

namespace Auth.Application.Common.Interfaces;

// Contract interface for generating JWT Access Tokens and Refresh Tokens
public interface IJwtTokenGenerator
{
    // Generates a short-lived JWT Access Token containing User Claims
    string GenerateAccessToken(ApplicationUser user, IEnumerable<string> roles);

    // Generates a cryptographically secure random Refresh Token
    RefreshToken GenerateRefreshToken(Guid userId, string jwtId);
    System.Security.Claims.ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
}