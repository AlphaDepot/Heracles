using Application.Features.WorkoutSessions.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "FluentValidation")]
public class AttachUserExerciseToWorkoutSessionCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new AttachUserExerciseToWorkoutSessionCommandValidator();
	}

	private AttachUserExerciseToWorkoutSessionCommandValidator _validator;

	[Test]
	public void AttacheUserExerciseToWorkoutSessionCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var request = new AttachUserExerciseToWorkoutSessionRequest(1, 1);
		var command = new AttachUserExerciseToWorkoutSessionCommand(request);
		var result = _validator.TestValidate(command);

		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSessionRequest.WorkoutSessionId);
		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSessionRequest.UserExerciseId);
	}

	[TestCase(0, 1, "UserExerciseId")]
	[TestCase(-1, 1, "UserExerciseId")]
	[TestCase(1, 0, "WorkoutSessionId")]
	[TestCase(1, -1, "WorkoutSessionId")]
	public void AttacheUserExerciseToWorkoutSessionCommandValidator_ShouldHaveError_WhenInputIsInvalid(
		int userExerciseId, int workoutSessionId, string testForPropertyName)
	{
		var request = new AttachUserExerciseToWorkoutSessionRequest(userExerciseId, workoutSessionId);
		var command = new AttachUserExerciseToWorkoutSessionCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "UserExerciseId":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSessionRequest.UserExerciseId);
				break;
			case "WorkoutSessionId":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSessionRequest.WorkoutSessionId);
				break;
		}
	}
}
