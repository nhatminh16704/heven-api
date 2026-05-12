using Heven.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heven.Api.Infrastructure.Data.Configurations;

public class ReviewReplyConfiguration : IEntityTypeConfiguration<ReviewReply>
{
    public void Configure(EntityTypeBuilder<ReviewReply> builder)
    {
        builder.Property(x => x.AuthorId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasMaxLength(2000)
            .IsRequired();

        builder.HasOne(x => x.AuthorProfile)
            .WithMany()
            .HasPrincipalKey(p => p.UserId)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ReviewId).IsUnique(); 
        builder.HasIndex(x => x.AuthorId);
    }
}
