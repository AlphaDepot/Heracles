using Application.Features.WorkoutSessions.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "FluentValidation")]
public class CreateWorkoutSessionCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new CreateWorkoutSessionCommandValidator();
	}

	private CreateWorkoutSessionCommandValidator _validator;

	[Test]
	public void CreateWorkoutSessionCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var request = new CreateWorkoutSessionRequest(
			"TestName",
			"Monday",
			1,
			"123456789012345678901234567890123456");

		var command = new CreateWorkoutSessionCommand(request);
		var result = _validator.TestValidate(command);

		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSession.UserId);
		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSession.Name);
		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSession.DayOfWeek);
	}

	[TestCase(null, "Monday", 1, "123456789012345678901234567890123456", "Name")]
	[TestCase("", "Monday", 1, "123456789012345678901234567890123456", "Name")]
	[TestCase(StringWith256Characters, "Monday", 1, "123456789012345678901234567890123456", "Name")]
	[TestCase("TestName", null, 1, "123456789012345678901234567890123456", "DayOfWeek")]
	[TestCase("TestName", "", 1, "123456789012345678901234567890123456", "DayOfWeek")]
	[TestCase("TestName", "ThisIsAnInvalidDay   ", 1, "123456789012345678901234567890123456", "DayOfWeek")]
	[TestCase("TestName", "Monday", 1, null, "UserId")]
	[TestCase("TestName", "Monday", 1, "", "UserId")]
	[TestCase("TestName", "Monday", 1, StringWith51Characters, "UserId")]
	public void CreateWorkoutSessionCommandValidator_ShouldHaveError_WhenInputIsInvalid(string? name, string? dayOfWeek,
		int sortOrder, string? userId, string testForPropertyName)
	{
		var request = new CreateWorkoutSessionRequest(name!, dayOfWeek!, sortOrder, userId!);

		var command = new CreateWorkoutSessionCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Name":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSession.Name);
				break;
			case "DayOfWeek":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSession.DayOfWeek);
				break;
			case "UserId":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSession.UserId);
				break;
		}
	}
}
