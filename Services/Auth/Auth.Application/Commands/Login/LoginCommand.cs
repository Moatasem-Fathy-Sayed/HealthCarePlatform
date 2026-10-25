using Auth.Application.Commands.Register;
using Auth.Application.Common.Interfaces;
using Auth.Domain.Entities;
using Healthcare.SharedKernel.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Commands.Login;

// MediatR command carrying user login credentials
public record LoginCommand(
    string Email,
    string Password
) : IRequest<Result<AuthenticationResult>>;

// Handler for executing the user authentication flow
public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResult>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public LoginCommandHandler(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        // Find user by email
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null || !user.IsActive)
        {
            return Result<AuthenticationResult>.Failure(
                new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        // Verify password hash
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return Result<AuthenticationResult>.Failure(
                new Error("Auth.InvalidCredentials", "Invalid email or password."));
        }

        // Fetch user roles
        var roles = await _userManager.GetRolesAsync(user);

        // Generate Access Token and Refresh Token
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, roles);
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id, Guid.NewGuid().ToString());

        // Update last login timestamp
        user.LastLoginAtUtc = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);

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