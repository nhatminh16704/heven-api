using Heven.Api.Domain.Common;
using Heven.Api.Domain.Entities;

namespace Heven.Api.Domain.Events;

public class BookingCreatedEvent : BaseEvent
{
    public BookingCreatedEvent(Booking booking)
    {
        Booking = booking;
    }

    public Booking Booking { get; }
}
