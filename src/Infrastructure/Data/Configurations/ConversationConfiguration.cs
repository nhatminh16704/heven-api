using Heven.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heven.Api.Infrastructure.Data.Configurations;

public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
{
    public void Configure(EntityTypeBuilder<Conversation> builder)
    {
        builder.Property(x => x.GuestId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.HostId)
            .HasMaxLength(450)
            .IsRequired();

        builder.HasOne(x => x.Listing)
            .WithMany()
            .HasForeignKey(x => x.ListingId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.ListingId);
        builder.HasIndex(x => x.GuestId);
        builder.HasIndex(x => x.HostId);
    }
}
