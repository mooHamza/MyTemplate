using FullTemplate.Domain.Entities;
using FullTemplate.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FullTemplate.Infrastructure.Data.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId).IsRequired();
        builder.Property(t => t.TokenHash).HasMaxLength(128).IsRequired();
        builder.Property(t => t.ReplacedByTokenHash).HasMaxLength(128);
        builder.Property(t => t.CreatedByIp).HasMaxLength(64);

        builder.HasIndex(t => t.TokenHash).IsUnique();   // lookup on every refresh
        builder.HasIndex(t => t.UserId);                 // revoke-all-for-user

        // Foreign key to AspNetUsers; deleting a user deletes their refresh tokens.
        builder.HasOne<ApplicationUser>()
               .WithMany()
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
