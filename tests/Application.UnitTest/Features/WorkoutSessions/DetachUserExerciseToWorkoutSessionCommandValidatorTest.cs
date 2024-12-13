using Application.Features.WorkoutSessions.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.WorkoutSessions;

[TestFixture(Category = "FluentValidation")]
public class DetachUserExerciseToWorkoutSessionCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new DetachUserExerciseToWorkoutSessionCommandValidator();
	}

	private DetachUserExerciseToWorkoutSessionCommandValidator _validator;

	[Test]
	public void AttacheUserExerciseToWorkoutSessionCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var request = new DetachUserExerciseToWorkoutSessionRequest(1, 1);
		var command = new DetachUserExerciseToWorkoutSessionCommand(request);
		var result = _validator.TestValidate(command);

		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSessionRequest.WorkoutSessionId);
		result.ShouldNotHaveValidationErrorFor(x => x.WorkoutSessionRequest.UserExerciseId);
	}

	[TestCase(0, 1, "WorkoutSessionId")]
	[TestCase(-1, 1, "WorkoutSessionId")]
	[TestCase(1, 0, "UserExerciseId")]
	[TestCase(1, -1, "UserExerciseId")]
	public void AttacheUserExerciseToWorkoutSessionCommandValidator_ShouldHaveError_WhenInputIsInvalid(
		int workoutSessionId, int userExerciseId, string testForPropertyName)
	{
		var request = new DetachUserExerciseToWorkoutSessionRequest(workoutSessionId, userExerciseId);
		var command = new DetachUserExerciseToWorkoutSessionCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "WorkoutSessionId":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSessionRequest.WorkoutSessionId);
				break;
			case "UserExerciseId":
				result.ShouldHaveValidationErrorFor(x => x.WorkoutSessionRequest.UserExerciseId);
				break;

		}
	}
}
