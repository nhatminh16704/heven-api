using Heven.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heven.Api.Infrastructure.Data.Configurations;

public class ListingConfiguration : IEntityTypeConfiguration<Listing>
{
    public void Configure(EntityTypeBuilder<Listing> builder)
    {
        builder.Property(x => x.HostId)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(x => x.Description)
            .HasMaxLength(2000);

        builder.Property(x => x.PropertyType)
            .HasMaxLength(100);

        builder.Property(x => x.PricePerNight)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.CleaningFee)
            .HasColumnType("decimal(18,2)");

        builder.Property(x => x.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder.HasOne(x => x.Category)
            .WithMany(x => x.Listings)
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.Location)
            .WithMany(x => x.Listings)
            .HasForeignKey(x => x.LocationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(x => x.HostId);
        builder.HasIndex(x => x.CategoryId);
        builder.HasIndex(x => x.LocationId);
        builder.HasIndex(x => x.Status);
    }
}
