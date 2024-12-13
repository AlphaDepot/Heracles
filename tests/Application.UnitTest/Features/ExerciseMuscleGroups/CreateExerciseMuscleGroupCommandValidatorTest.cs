using Application.Features.ExerciseMuscleGroups.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "FluentValidation")]
public class CreateExerciseMuscleGroupCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new CreateExerciseMuscleGroupCommandValidator();
	}

	private CreateExerciseMuscleGroupCommandValidator _validator;

	[TestCase(1, 1, 1, 1)]
	[TestCase(1, 1, 1, 1.6)]
	public void CreateExerciseMuscleGroupCommandValidator_ShouldNotHaveError_WhenInputIsValid(int exerciseTypeId,
		int muscleId, int functionId, double functionPercentage)
	{
		var command = new CreateExerciseMuscleGroupCommand(
			new CreateExerciseMuscleGroupRequest(exerciseTypeId, muscleId, functionId, functionPercentage));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseMuscleGroup.ExerciseTypeId);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseMuscleGroup.MuscleId);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseMuscleGroup.FunctionId);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseMuscleGroup.FunctionPercentage);
	}

	[TestCase(0, 1, 1, 1, "ExerciseTypeId")]
	[TestCase(1, 0, 1, 1, "MuscleId")]
	[TestCase(1, 1, 0, 1, "FunctionId")]
	[TestCase(1, 1, 1, 0, "FunctionPercentage")]
	[TestCase(1, 1, 1, 101, "FunctionPercentage")]
	public void CreateExerciseMuscleGroupCommandValidator_ShouldHaveError_WhenInputIsInvalid(int exerciseTypeId,
		int muscleId, int functionId, double functionPercentage, string testForPropertyName)
	{
		var command = new CreateExerciseMuscleGroupCommand(
			new CreateExerciseMuscleGroupRequest(exerciseTypeId, muscleId, functionId, functionPercentage));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "ExerciseTypeId":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.ExerciseTypeId);
				break;
			case "MuscleId":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.MuscleId);
				break;
			case "FunctionId":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.FunctionId);
				break;
			case "FunctionPercentage":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.FunctionPercentage);
				break;
		}
	}
}
