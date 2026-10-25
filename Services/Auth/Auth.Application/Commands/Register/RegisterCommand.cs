using Auth.Application.Common.Interfaces;
using Auth.Domain.Entities;
using Healthcare.SharedKernel.Common;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Auth.Application.Commands.Register;

// Response DTO containing user info and generated authentication tokens
public record AuthenticationResult(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken
);

// MediatR command for registering a new system user
public record RegisterCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName,
    UserType UserType
) : IRequest<Result<AuthenticationResult>>;

// Handler for executing the user registration flow
public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthenticationResult>>
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public RegisterCommandHandler(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<AuthenticationResult>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Check if user email already exists
        var existingUser = await _userManager.FindByEmailAsync(request.Email);
        if (existingUser != null)
        {
            return Result<AuthenticationResult>.Failure(
                new Error("Auth.DuplicateEmail", "The email address is already registered."));
        }

        // Create new ApplicationUser domain entity
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            UserName = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName,
            UserType = request.UserType,
            CreatedAtUtc = DateTime.UtcNow,
            IsActive = true
        };

        // Create user with hashed password via ASP.NET Core Identity
        var identityResult = await _userManager.CreateAsync(user, request.Password);
        if (!identityResult.Succeeded)
        {
            var firstError = identityResult.Errors.FirstOrDefault()?.Description ?? "Registration failed.";
            return Result<AuthenticationResult>.Failure(new Error("Auth.UserCreationError", firstError));
        }

        // Ensure role exists in the database before assigning it
        var roleName = request.UserType.ToString();
        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            var createRoleResult = await _roleManager.CreateAsync(new IdentityRole<Guid>
            {
                Name = roleName,
                NormalizedName = roleName.ToUpperInvariant()
            });

            if (!createRoleResult.Succeeded)
            {
                var roleError = createRoleResult.Errors.FirstOrDefault()?.Description ?? "Failed to create security role.";
                return Result<AuthenticationResult>.Failure(new Error("Auth.RoleCreationError", roleError));
            }
        }

        // Assign default role based on UserType
        var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
        if (!addToRoleResult.Succeeded)
        {
            var assignError = addToRoleResult.Errors.FirstOrDefault()?.Description ?? "Failed to assign role to user.";
            return Result<AuthenticationResult>.Failure(new Error("Auth.RoleAssignmentError", assignError));
        }

        // Generate Access Token and Refresh Token
        var accessToken = _jwtTokenGenerator.GenerateAccessToken(user, new[] { roleName });
        var refreshToken = _jwtTokenGenerator.GenerateRefreshToken(user.Id, Guid.NewGuid().ToString());

        var authResult = new AuthenticationResult(
            user.Id,
            user.Email,
            user.FirstName,
            user.LastName,
            accessToken,
            refreshToken.Token
        );

        return Result<AuthenticationResult>.Success(authResult);
    }
}