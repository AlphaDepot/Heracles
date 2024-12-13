using Application.Features.Users.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.Users;

[TestFixture(Category = "FluentValidation")]
public class CreateOrUpdateCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new CreateOrUpdateCommandValidator();
	}

	private CreateOrUpdateCommandValidator _validator;

	[TestCase(StringStaticGuid, "Email@email.com")]
	public void CreateOrUpdateCommandValidator_ShouldNotHaveError_WhenInputIsValid(string userId, string email)
	{
		var request = new CreateOrUpdateRequest(userId, email, true);
		var command = new CreateOrUpdateCommand(request);
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.UserRequest.UserId);
		result.ShouldNotHaveValidationErrorFor(x => x.UserRequest.Email);
	}

	[TestCase("", "UserId")]
	[TestCase(null, "UserId")]
	[TestCase(StringWith256Characters, "UserId")]
	[TestCase(null, "Email")]
	[TestCase("", "Email")]
	[TestCase("email.com", "Email")]
	public void CreateOrUpdateCommandValidator_ShouldHaveError_WhenInputIsInvalid(string? value,
		string testForPropertyName)
	{
		var request = new CreateOrUpdateRequest(value!, value!, true);
		var command = new CreateOrUpdateCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "UserId":
				result.ShouldHaveValidationErrorFor(x => x.UserRequest.UserId);
				break;
			case "Email":
				result.ShouldHaveValidationErrorFor(x => x.UserRequest.Email);
				break;
		}
	}
}
