using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Constants;
using Heven.Api.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heven.Api.Application.Bookings.EventHandlers;

public class BookingCreatedEventHandler : INotificationHandler<BookingCreatedEvent>
{
    private readonly IApplicationDbContext _context;

    public BookingCreatedEventHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(BookingCreatedEvent notification, CancellationToken cancellationToken)
    {
        var stayDates = new List<DateOnly>();
        for (var d = notification.Booking.CheckIn; d < notification.Booking.CheckOut; d = d.AddDays(1))
        {
            stayDates.Add(d);
        }

        var calendarRows = await _context.ListingCalendars
            .Where(lc => lc.ListingId == notification.Booking.ListingId && stayDates.Contains(lc.Date))
            .ToListAsync(cancellationToken);

        foreach (var row in calendarRows)
        {
            row.Status = ListingCalendarStatuses.Booked;
            row.Booking = notification.Booking;
        }
    }
}
