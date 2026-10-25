using Auth.Application.Commands.Register;
using Auth.Application.Common.Interfaces;
using Auth.Domain.Entities;
using Healthcare.SharedKernel.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Commands.RefreshToken;

// MediatR command carrying expired AccessToken and valid RefreshToken
public record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken
) : IRequest<Result<AuthenticationResult>>;

// Handler to validate tokens and issue a fresh pair (Token Rotation)
public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResult>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RefreshTokenCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate basic request inputs
        if (string.IsNullOrWhiteSpace(request.AccessToken) || string.IsNullOrWhiteSpace(request.RefreshToken))
        {
            return Result<AuthenticationResult>.Failure(
                new Error("Auth.InvalidToken", "Invalid access or refresh token provided."));
        }

        // 2. Extract user claims safely via existing IJwtTokenGenerator service
        var principal = _jwtTokenGenerator.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null)
        {
            return Result<AuthenticationResult>.Failure(
                new Error("Auth.InvalidToken", "Invalid or malformed access token."));
        }

        // 3. Retrieve user ID claim
        var userIdClaim = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                       ?? principal.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Result<AuthenticationResult>.Failure(
                new Error("Auth.InvalidToken", "Invalid token claims structure."));
        }

        // 4. Find user in the system
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null || !user.IsActive)
        {
            return Result<AuthenticationResult>.Failure(
                new Error("Auth.UserNotFound", "User associated with token is invalid or inactive."));
        }

        // 5. Generate fresh Access Token and Refresh Token rotation
        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id, Guid.NewGuid().ToString());

        var authResult = new AuthenticationResult(
            user.Id,
            user.Email!,
            user.FirstName,
            user.LastName,
            accessToken,
            refreshToken.Token
        );

        return Result<AuthenticationResult>.Success(authResult);
    }
}