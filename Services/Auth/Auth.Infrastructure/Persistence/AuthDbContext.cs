using Auth.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistence;

// Custom DbContext for Authentication using ASP.NET Core Identity with Guid key
public class AuthDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    // DbSet for multi-device refresh tokens
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply Identity base configurations
        base.OnModelCreating(builder);

        // Map Identity entities to "auth" database schema
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("Users", "auth");
            entity.Property(u => u.FirstName).HasMaxLength(50).IsRequired();
            entity.Property(u => u.LastName).HasMaxLength(50).IsRequired();
        });

        builder.Entity<IdentityRole<Guid>>(entity => entity.ToTable("Roles", "auth"));
        builder.Entity<IdentityUserRole<Guid>>(entity => entity.ToTable("UserRoles", "auth"));
        builder.Entity<IdentityUserClaim<Guid>>(entity => entity.ToTable("UserClaims", "auth"));
        builder.Entity<IdentityUserLogin<Guid>>(entity => entity.ToTable("UserLogins", "auth"));
        builder.Entity<IdentityRoleClaim<Guid>>(entity => entity.ToTable("RoleClaims", "auth"));
        builder.Entity<IdentityUserToken<Guid>>(entity => entity.ToTable("UserTokens", "auth"));

        // Configure RefreshToken mapping
        builder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens", "auth");
            entity.HasKey(rt => rt.Id);
            entity.Property(rt => rt.Token).HasMaxLength(200).IsRequired();
            entity.Property(rt => rt.JwtId).HasMaxLength(100).IsRequired();

            entity.HasOne(rt => rt.User)
                  .WithMany(u => u.RefreshTokens)
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}