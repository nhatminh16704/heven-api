using Heven.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Heven.Api.Infrastructure.Data.Configurations;

public class BookingCancellationConfiguration : IEntityTypeConfiguration<BookingCancellation>
{
    public void Configure(EntityTypeBuilder<BookingCancellation> builder)
    {
        builder.Property(x => x.CancelledBy)
            .HasMaxLength(450)
            .IsRequired();

        builder.Property(x => x.Reason)
            .HasMaxLength(1000)
            .IsRequired();

        builder.Property(x => x.RefundStatus)
            .HasMaxLength(50);

        builder.Property(x => x.RefundAmount)
            .HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Booking)
            .WithOne(x => x.Cancellation)
            .HasForeignKey<BookingCancellation>(x => x.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
