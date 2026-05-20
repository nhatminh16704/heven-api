using Heven.Api.Domain.Entities;
using Heven.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heven.Api.Infrastructure.Data.Configurations;

public class ListingCalendarConfiguration : IEntityTypeConfiguration<ListingCalendar>
{
    public void Configure(EntityTypeBuilder<ListingCalendar> builder)
    {
        builder.HasKey(lc => new { lc.ListingId, lc.Date });

        builder.Property(lc => lc.Status)
            .IsRequired()
            .HasMaxLength(20)
            .HasConversion(
                v => v.ToString().ToLower(),
                v => (ListingCalendarStatus)Enum.Parse(typeof(ListingCalendarStatus), v, true)
            );

        builder.ToTable("ListingCalendars", t => t.HasCheckConstraint("CK_ListingCalendar_Status", "Status IN ('available', 'blocked', 'booked')"));

        builder.HasIndex(lc => new { lc.ListingId, lc.Status, lc.Date })
            .HasDatabaseName("idx_listing_calendar_status");

        builder.HasIndex(lc => lc.BookingId)
            .HasDatabaseName("idx_listing_calendar_reservation")
            .HasFilter("BookingId IS NOT NULL");

        builder.HasOne(lc => lc.Listing)
            .WithMany()
            .HasForeignKey(lc => lc.ListingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lc => lc.Booking)
            .WithMany()
            .HasForeignKey(lc => lc.BookingId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
