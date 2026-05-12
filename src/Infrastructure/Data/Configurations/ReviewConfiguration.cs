using Heven.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heven.Api.Infrastructure.Data.Configurations;

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.Property(x => x.AuthorId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasMaxLength(2000);

        builder.HasOne(x => x.Listing)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Booking)
            .WithMany()
            .HasForeignKey(x => x.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.AuthorProfile)
            .WithMany()
            .HasPrincipalKey(p => p.UserId)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Map quan hệ 1-1 với ReviewReply
        builder.HasOne(x => x.Reply)
            .WithOne(r => r.Review)
            .HasForeignKey<ReviewReply>(r => r.ReviewId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.ListingId);
        builder.HasIndex(x => x.BookingId);
        builder.HasIndex(x => x.AuthorId);
    }
}
