using Heven.Api.Domain.Common;
using Heven.Api.Domain.Entities;

namespace Heven.Api.Domain.Events;

public class BookingPendingCreatedEvent : BaseEvent
{
    public BookingPendingCreatedEvent(Booking booking)
    {
        Booking = booking;
    }
    public Booking Booking { get; }
}
