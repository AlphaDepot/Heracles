using Application.Common.Errors;
using Microsoft.AspNetCore.Http;

namespace Application.UnitTest.Common.Errors;

[Category("Errors")]
public class ErrorCodesTest
{
	[Test]
	public void NullValue_ShouldHaveCorrectProperties()
	{
		var error = ErrorTypes.NullValue;
		Assert.That(error, Is.Not.Null);
		Assert.That(error.Type, Is.EqualTo(ErrorCodes.NullValue));
		Assert.That(error.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
		Assert.That(error.Description, Is.EqualTo(ErrorMessages.NullValue));
	}

	[Test]
	public void InvalidRequest_ShouldHaveCorrectProperties()
	{
		var error = ErrorTypes.BadRequest;
		Assert.That(error, Is.Not.Null);
		Assert.That(error.Type, Is.EqualTo(ErrorCodes.BadRequest));
		Assert.That(error.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
		Assert.That(error.Description, Is.EqualTo(ErrorMessages.InvalidRequest));
	}

	[Test]
	public void DuplicateEntry_ShouldHaveCorrectProperties()
	{
		var error = ErrorTypes.DuplicateEntry;

		Assert.That(error, Is.Not.Null);
		Assert.That(error.Type, Is.EqualTo(ErrorCodes.DuplicateEntry));
		Assert.That(error.StatusCode, Is.EqualTo(StatusCodes.Status409Conflict));
		Assert.That(error.Description, Is.EqualTo(ErrorMessages.DuplicateEntry));
	}

	[Test]
	public void IncompleteUserClaims_ShouldHaveCorrectProperties()
	{
		var error = ErrorTypes.IncompleteUserClaims;

		Assert.That(error, Is.Not.Null);
		Assert.That(error.Type, Is.EqualTo(ErrorCodes.IncompleteUserClaims));
		Assert.That(error.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
		Assert.That(error.Description, Is.EqualTo(ErrorMessages.IncompleteUserClaims));
	}

	[Test]
	public void Validation_ShouldHaveCorrectProperties()
	{
		var error = ErrorTypes.Validation;

		Assert.That(error, Is.Not.Null);
		Assert.That(error.Type, Is.EqualTo(ErrorCodes.Validation));
		Assert.That(error.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
		Assert.That(error.Description, Is.EqualTo(ErrorMessages.Validation));
	}

	[Test]
	public void InvalidErrorType_ShouldHaveCorrectProperties()
	{
		var error = ErrorTypes.InvalidErrorType;

		Assert.That(error, Is.Not.Null);
		Assert.That(error.Type, Is.EqualTo(ErrorCodes.InvalidErrorType));
		Assert.That(error.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
		Assert.That(error.Description, Is.EqualTo(ErrorMessages.InvalidErrorType));
	}

	[Test]
	public void BadRequestErrorType_ShouldHaveCorrectProperties()
	{
		var error = ErrorTypes.BadRequest;

		Assert.That(error, Is.Not.Null);
		Assert.That(error.Type, Is.EqualTo(ErrorCodes.BadRequest));
		Assert.That(error.StatusCode, Is.EqualTo(StatusCodes.Status400BadRequest));
		Assert.That(error.Description, Is.EqualTo(ErrorMessages.InvalidRequest));
	}

	[Test]
	public void DatabaseErrorType_ShouldHaveCorrectProperties()
	{
		var error = ErrorTypes.DatabaseError;

		Assert.That(error, Is.Not.Null);
		Assert.That(error.Type, Is.EqualTo(ErrorCodes.DatabaseError));
		Assert.That(error.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
		Assert.That(error.Description, Is.EqualTo(ErrorMessages.DatabaseError));
	}

	[Test]
	public void DatabaseWithErrorMessageErrorType_ShouldHaveCorrectProperties()
	{
		var errorMessage = "Test error message";
		var error = ErrorTypes.DatabaseErrorWithMessage(errorMessage);

		Assert.That(error, Is.Not.Null);
		Assert.That(error.Type, Is.EqualTo(ErrorCodes.DatabaseError));
		Assert.That(error.StatusCode, Is.EqualTo(StatusCodes.Status500InternalServerError));
		Assert.That(error.Description, Is.EqualTo(errorMessage));
	}
}
