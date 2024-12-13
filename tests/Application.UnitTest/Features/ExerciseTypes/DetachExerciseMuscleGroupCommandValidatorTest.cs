using Application.Features.ExerciseTypes.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "FluentValidation")]
public class DetachExerciseMuscleGroupCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new DetachExerciseMuscleGroupCommandValidator();
	}

	private DetachExerciseMuscleGroupCommandValidator _validator;

	[Test]
	public void DetachExerciseMuscleGroupCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var request = new DetachExerciseMuscleGroupRequest(1, 1);
		var command = new DetachExerciseMuscleGroupCommand(request);
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseMuscleGroup.ExerciseTypeId);
	}

	[TestCase(0, 1, "Exercise Type Id")]
	[TestCase(1, 0, "Exercise Muscle Group Id")]
	public void DetachExerciseMuscleGroupCommandValidator_ShouldHaveError_WhenInputIsInvalid(int exerciseTypeId,
		int muscleGroupId, string testForPropertyName)
	{
		var request = new DetachExerciseMuscleGroupRequest(exerciseTypeId, muscleGroupId);
		var command = new DetachExerciseMuscleGroupCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Exercise Type Id":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.ExerciseTypeId);
				break;
			case "Exercise Muscle Group Id":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.MuscleGroupId);
				break;
		}
	}
}
