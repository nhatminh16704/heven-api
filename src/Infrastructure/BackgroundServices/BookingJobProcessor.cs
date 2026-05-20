using Heven.Api.Application.Common.Interfaces;
using Heven.Api.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Heven.Api.Infrastructure.BackgroundServices;

public class BookingJobProcessor
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<BookingJobProcessor> _logger;

    public BookingJobProcessor(IApplicationDbContext context, ILogger<BookingJobProcessor> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task ProcessTimeout(int bookingId)
    {
        var booking = await _context.Bookings
            .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null)
        {
            _logger.LogWarning("Hangfire Timeout Job: Booking {BookingId} not found.", bookingId);
            return;
        }

        if (booking.Status == BookingStatus.Pending)
        {
            booking.Status = BookingStatus.Cancelled;
            
            var calendars = await _context.ListingCalendars
                .Where(c => c.BookingId == bookingId)
                .ToListAsync();

            foreach (var calendar in calendars)
            {
                calendar.Status = ListingCalendarStatus.Available;
                calendar.BookingId = null;
            }

            await _context.SaveChangesAsync(CancellationToken.None);
            _logger.LogInformation("Booking {BookingId} has been cancelled by Hangfire due to payment timeout.", bookingId);
        }
        else
        {
            _logger.LogInformation("Booking {BookingId} is not in Pending state ({Status}). No action taken.", bookingId, booking.Status);
        }
    }
}
