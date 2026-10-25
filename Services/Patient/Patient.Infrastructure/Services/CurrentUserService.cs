using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Patient.Application.Common.Interfaces;

namespace Patient.Infrastructure.Services;

/// <summary>
/// Service implementation for retrieving the current authenticated user identifier from HttpContext claims.
/// </summary>
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Reads and parses the user identifier claim from active JWT token.
    /// </summary>
    public Guid UserId
    {
        get
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? _httpContextAccessor.HttpContext?.User?.FindFirstValue("sub");

            return Guid.TryParse(userIdStr, out var userId) ? userId : Guid.Empty;
        }
    }
}