using Heracles.Domain.Abstractions.Responses;
using Microsoft.AspNetCore.Http;

namespace Heracles.Infrastructure.Extensions;

public static class ResultExtensions
{
    public static IResult ToProblemDetails(this ServiceResponse serviceResponse)
    {
        if (serviceResponse.IsSuccess)
        {
            throw new InvalidOperationException("Can't convert successful serviceResponse to problem.");
        }
        
        return Results.Problem(
            statusCode: serviceResponse.StatusCode,
            title: "One or more validation errors occurred!.",
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.1",
            extensions: new Dictionary<string, object?>
            {
                {"errors",new[] {serviceResponse.Error}}
            });
    }
}