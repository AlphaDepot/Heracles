using Application.Common.Errors;
using Microsoft.AspNetCore.Http;

namespace Application.Common.Responses;

public interface IValidationResult
{
	public static readonly Error ValidationError = new(
		ErrorCodes.Validation, StatusCodes.Status400BadRequest, "Validation error occurred. ");
}
