using Application.Features.ExerciseTypes.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "FluentValidation")]
public class AddExerciseMuscleGroupCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new AddExerciseMuscleGroupCommandValidator();
	}

	private AddExerciseMuscleGroupCommandValidator _validator;

	[Test]
	public void AddExerciseMuscleGroupCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var request = new AttachExerciseMuscleGroupRequest(1, 1);
		var command = new AttachExerciseMuscleGroupCommand(request);
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseMuscleGroup.ExerciseTypeId);
	}

	[TestCase(0, 1, "Exercise Type Id")]
	[TestCase(1, 0, "Muscle Group Id")]
	public void AddExerciseMuscleGroupCommandValidator_ShouldHaveError_WhenInputIsInvalid(int exerciseTypeId,
		int muscleGroupId, string testForPropertyName)
	{
		var request = new AttachExerciseMuscleGroupRequest(exerciseTypeId, muscleGroupId);
		var command = new AttachExerciseMuscleGroupCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Exercise Type Id":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.ExerciseTypeId);
				break;
			case "Muscle Group Id":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.MuscleGroupId);
				break;
		}
	}
}
