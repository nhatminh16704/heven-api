using FluentValidation.Results;

namespace Heven.Api.Application.Common.Exceptions;

public class ValidationException : Exception
{
    public ValidationException()
        : base("One or more validation failures have occurred.")
    {
        Errors = new Dictionary<string, ValidationError[]>();
    }

    public ValidationException(IEnumerable<ValidationFailure> failures)
        : this()
    {
        Errors = failures
            .GroupBy(e => e.PropertyName, e => new ValidationError(e.ErrorMessage, e.ErrorCode))
            .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
    }

    public IDictionary<string, ValidationError[]> Errors { get; }
}

public record ValidationError(string ErrorMessage, string? ErrorCode);
