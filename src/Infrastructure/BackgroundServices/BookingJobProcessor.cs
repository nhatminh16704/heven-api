using Heven.Api.Application.Bookings.Commands.CancelBookingTimeout;
using MediatR;

namespace Heven.Api.Infrastructure.BackgroundServices;

public class BookingJobProcessor
{
    private readonly ISender _sender;

    public BookingJobProcessor(ISender sender)
    {
        _sender = sender;
    }

    public async Task ProcessTimeout(int bookingId)
    {
        await _sender.Send(new CancelBookingTimeoutCommand(bookingId));
    }
}
