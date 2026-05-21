using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Enums;
using Heven.Api.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Heven.Api.Application.Bookings.EventHandlers;

public class BookingCancelledEventHandler : INotificationHandler<BookingCancelledEvent>
{
    private readonly IApplicationDbContext _context;

    public BookingCancelledEventHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task Handle(BookingCancelledEvent notification, CancellationToken cancellationToken)
    {
        // Free up the calendars
        var calendars = await _context.ListingCalendars
            .Where(c => c.BookingId == notification.Booking.Id)
            .ToListAsync(cancellationToken);

        foreach (var calendar in calendars)
        {
            calendar.Status = ListingCalendarStatus.Available;
            calendar.BookingId = null;
        }

    }
}
