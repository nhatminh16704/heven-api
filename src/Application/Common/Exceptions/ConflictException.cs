namespace Heven.Api.Application.Common.Exceptions;

public class ConflictException : Exception
{
    public string ErrorCode { get; }

    public ConflictException() : base("A conflict occurred with the current state of the resource.")
    {
        ErrorCode = "CONFLICT";
    }

    public ConflictException(string message) : base(message)
    {
        ErrorCode = "CONFLICT";
    }

    public ConflictException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}