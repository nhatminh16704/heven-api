using Hangfire;
using Heven.Api.Application.Common.Interfaces;

namespace Heven.Api.Infrastructure.BackgroundServices;

public class BookingJobService : IBookingJobService
{
    private readonly IBackgroundJobClient _backgroundJobClient;

    public BookingJobService(IBackgroundJobClient backgroundJobClient)
    {
        _backgroundJobClient = backgroundJobClient;
    }   

    public string SchedulePaymentTimeout(int bookingId, TimeSpan delay)
    {
            return _backgroundJobClient.Schedule<BookingJobProcessor>(
                processor => processor.ProcessTimeout(bookingId), 
                delay);
    }

    public bool CancelPaymentTimeout(string jobId)
    {
        return _backgroundJobClient.Delete(jobId);
    }
}
