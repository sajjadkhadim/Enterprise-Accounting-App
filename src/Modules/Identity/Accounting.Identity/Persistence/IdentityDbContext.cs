
using Accounting.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Accounting.Identity.Persistence;

public sealed class IdentityDbContext
    : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public IdentityDbContext(
        DbContextOptions<IdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("users");

            entity.Property(user => user.FirstName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(user => user.LastName)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(user => user.IsActive)
                .IsRequired();

            entity.Property(user => user.CreatedAtUtc)
                .IsRequired();
        });

        builder.Entity<IdentityRole<Guid>>()
            .ToTable("roles");

        builder.Entity<IdentityUserRole<Guid>>()
            .ToTable("user_roles");

        builder.Entity<IdentityUserClaim<Guid>>()
            .ToTable("user_claims");

        builder.Entity<IdentityUserLogin<Guid>>()
            .ToTable("user_logins");

        builder.Entity<IdentityRoleClaim<Guid>>()
            .ToTable("role_claims");

        builder.Entity<IdentityUserToken<Guid>>()
            .ToTable("user_tokens");

        builder.Entity<TenantMembership>(entity =>
        {
            entity.ToTable("tenant_memberships");

            entity.HasKey(membership => membership.Id);

            entity.Property(membership => membership.Role)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(membership => membership.IsActive)
                .IsRequired();

            entity.Property(membership => membership.CreatedAtUtc)
                .IsRequired();

            entity.HasIndex(membership => new
            {
                membership.TenantId,
                membership.UserId
            }).IsUnique();

            entity.HasOne(membership => membership.User)
                .WithMany()
                .HasForeignKey(membership => membership.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

    }
}
