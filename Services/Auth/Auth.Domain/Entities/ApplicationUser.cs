namespace Auth.Domain.Entities;

using Microsoft.AspNetCore.Identity;

// Custom User entity extending IdentityUser with Guid Primary Key
public class ApplicationUser : IdentityUser<Guid>
{
    // First name of the system user
    public string FirstName { get; set; } = string.Empty;

    // Last name of the system user
    public string LastName { get; set; } = string.Empty;

    // Defines whether the user is Patient, Doctor, Admin, or Staff
    public UserType UserType { get; set; }

    // System audit flags
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAtUtc { get; set; }

    // Navigation property for multi-device refresh tokens
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

public enum UserType
{
    Patient = 1,
    Doctor = 2,
    Admin = 3,
    Staff = 4
}