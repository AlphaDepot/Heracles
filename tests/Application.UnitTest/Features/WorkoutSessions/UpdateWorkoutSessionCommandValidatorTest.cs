using Application.Features.WorkoutSessions.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "FluentValidation")]
public class UpdateWorkoutSessionCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new UpdateWorkoutSessionCommandValidator();
	}

	private UpdateWorkoutSessionCommandValidator _validator;

	[Test]
	public void UpdateWorkoutSessionCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var request = new UpdateWorkoutSessionRequest
		{
			Name = "UniqueName",
			DayOfWeek = "Monday",
			UserId = "123456789012345678901234567890123456",
			Id = 1,
			Concurrency = StringStaticGuid
		};
		var command = new UpdateWorkoutSessionCommand(request);
		var result = _validator.TestValidate(command);

		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSession.Id);
		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSession.Name);
		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSession.DayOfWeek);
		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSession.UserId);
		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSession.Concurrency);
	}

	[TestCase(0, "Name", "Monday", 1, StringStaticGuid, StringStaticGuid, "Id")]
	[TestCase(1, null, "Monday", 1, StringStaticGuid, StringStaticGuid, "Name")]
	[TestCase(1, "", "Monday", 1, StringStaticGuid, StringStaticGuid, "Name")]
	[TestCase(1, StringWith256Characters, "Monday", 1, StringStaticGuid, StringStaticGuid, "Name")]
	[TestCase(1, "Name", null, 1, StringStaticGuid, StringStaticGuid, "DayOfWeek")]
	[TestCase(1, "Name", "", 1, StringStaticGuid, StringStaticGuid, "DayOfWeek")]
	[TestCase(1, "Name", "ThisIsAnInvalidDay   ", 1, StringStaticGuid, StringStaticGuid, "DayOfWeek")]
	[TestCase(1, "Name", "Monday", 1, null, StringStaticGuid, "UserId")]
	[TestCase(1, "Name", "Monday", 1, "", StringStaticGuid, "UserId")]
	[TestCase(1, "Name", "Monday", 1, StringWith51Characters, StringStaticGuid, "UserId")]
	[TestCase(1, "Name", "Monday", 1, StringStaticGuid, null, "Concurrency")]
	[TestCase(1, "Name", "Monday", 1, StringStaticGuid, "", "Concurrency")]
	[TestCase(1, "Name", "Monday", 1, StringStaticGuid, StringWith256Characters, "Concurrency")]
	public void UpdateWorkoutSessionCommandValidator_ShouldHaveError_WhenInputIsInvalid(int id, string? name,
		string? dayOfWeek, int sortOrder, string? userId, string? concurrency, string testForPropertyName)
	{
		var request = new UpdateWorkoutSessionRequest
		{
			Name = name!,
			DayOfWeek = dayOfWeek,
			UserId = userId!,
			Id = id,
			Concurrency = concurrency
		};
		var command = new UpdateWorkoutSessionCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Id":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSession.Id);
				break;
			case "Name":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSession.Name);
				break;
			case "DayOfWeek":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSession.DayOfWeek);
				break;
			case "UserId":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSession.UserId);
				break;
			case "Concurrency":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSession.Concurrency);
				break;
		}
	}
}
