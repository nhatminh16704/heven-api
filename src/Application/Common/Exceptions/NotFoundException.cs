namespace Heven.Api.Application.Common.Exceptions;

public class NotFoundException : Exception
{
    public string ErrorCode { get; }

    public NotFoundException() : base("The specified resource was not found.")
    {
        ErrorCode = "NOT_FOUND";
    }

    public NotFoundException(string message) : base(message)
    {
        ErrorCode = "NOT_FOUND";
    }

    public NotFoundException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}
