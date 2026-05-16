namespace Heven.Api.Application.Common.Exceptions;

public class ForbiddenAccessException : Exception
{
    public string ErrorCode { get; }

    public ForbiddenAccessException() : base("You do not have permission to access this resource.")
    {
        ErrorCode = "FORBIDDEN";
    }

    public ForbiddenAccessException(string message) : base(message)
    {
        ErrorCode = "FORBIDDEN";
    }

    public ForbiddenAccessException(string message, string errorCode) : base(message)
    {
        ErrorCode = errorCode;
    }
}
