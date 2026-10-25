namespace Auth.Domain.Entities;

// Entity to store and manage refresh tokens for multi-device authentication
public class RefreshToken
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public string JwtId { get; set; } = string.Empty; // Unique token identifier matching the JTI claim
    public bool IsUsed { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAtUtc { get; set; }
    public string? ReplacedByToken { get; set; }

    // Navigation property back to ApplicationUser entity
    public ApplicationUser User { get; set; } = null!;
}