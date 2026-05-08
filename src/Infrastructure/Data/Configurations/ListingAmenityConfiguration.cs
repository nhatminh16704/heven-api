using Heven.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heven.Api.Infrastructure.Data.Configurations;

public class ListingAmenityConfiguration : IEntityTypeConfiguration<ListingAmenity>
{
    public void Configure(EntityTypeBuilder<ListingAmenity> builder)
    {
        // Composite unique index: 1 listing không thể có 2 amenity giống nhau
        builder.HasIndex(x => new { x.ListingId, x.AmenityId }).IsUnique();

        builder.HasOne(x => x.Listing)
            .WithMany(x => x.Amenities)
            .HasForeignKey(x => x.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Amenity)
            .WithMany(x => x.ListingAmenities)
            .HasForeignKey(x => x.AmenityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
