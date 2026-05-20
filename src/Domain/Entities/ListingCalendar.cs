using Heven.Api.Domain.Common;

using Heven.Api.Domain.Enums;

namespace Heven.Api.Domain.Entities;

public class ListingCalendar
{
    public int ListingId { get; set; }
    public DateOnly Date { get; set; }
    public decimal? Price { get; set; }
    public ListingCalendarStatus Status { get; set; } = ListingCalendarStatus.Available;
    public int? BookingId { get; set; }

    public Listing Listing { get; set; } = null!;
    public Booking? Booking { get; set; }
}
