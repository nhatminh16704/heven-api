using Heven.Api.Domain.Common;
using Heven.Api.Domain.Entities;

namespace Heven.Api.Domain.Events;

public class BookingCancelledEvent : BaseEvent
{
    public BookingCancelledEvent(Booking booking)
    {
        Booking = booking;
    }

    public Booking Booking { get; }
}
