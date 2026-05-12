using Heven.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heven.Api.Infrastructure.Data.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.UserId)
            .IsRequired()
            .HasMaxLength(450);

        builder.HasIndex(t => t.UserId)
            .IsUnique();

        builder.Property(t => t.FirstName)
            .HasMaxLength(100);

        builder.Property(t => t.LastName)
            .HasMaxLength(100);

        builder.Property(t => t.AvatarUrl)
            .HasMaxLength(500);

        builder.Property(t => t.Bio)
            .HasMaxLength(1000);

        builder.HasOne(t => t.City)
            .WithMany()
            .HasForeignKey(t => t.CityId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
