namespace Heven.Api.Domain.Enums;

public enum ListingStatus { Pending, Active, Suspended, Inactived }
public enum BookingStatus { Pending, Confirmed, Cancelled, Completed }
public enum PaymentStatus { Pending, Completed, Failed, Refunded }
public enum ReviewType { GuestToHost, HostToGuest }
public enum NotificationType { System, Booking, Message, Promotion }
public enum ListingCalendarStatus { Available, Blocked, Booked }
