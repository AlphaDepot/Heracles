using Heracles.Domain.Abstractions.Responses;

namespace Heracles.Domain.Abstractions.Errors;

public sealed record Error(string Code, string? Description = null, IDictionary<string, string[]>? Errors = null)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");

    public static implicit operator ServiceResponse(Error error) => ServiceResponse.Failure(error);

    public ServiceResponse ToResult() => ServiceResponse.Failure(this);
}