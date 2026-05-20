namespace Heven.Api.Application.Common.Interfaces;

public interface IBookingJobService
{
    string SchedulePaymentTimeout(int bookingId, TimeSpan delay);
    bool CancelPaymentTimeout(string jobId);
}
