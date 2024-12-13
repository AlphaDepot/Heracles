using Application.Features.ExerciseMuscleGroups.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "FluentValidation")]
public class UpdateExerciseMuscleGroupCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new UpdateExerciseMuscleGroupCommandValidator();
	}

	private UpdateExerciseMuscleGroupCommandValidator _validator;

	[TestCase(1, StringStaticGuid, 1)]
	[TestCase(1, StringStaticGuid, 45.58)]
	[TestCase(1, StringStaticGuid, 100)]
	public void UpdateExerciseMuscleGroupCommandValidator_ShouldNotHaveError_WhenInputIsValid(int id,
		string concurrency, double functionPercentage)
	{
		var command =
			new UpdateExerciseMuscleGroupCommand(
				new UpdateExerciseMuscleGroupRequest(id, concurrency, functionPercentage));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseMuscleGroup.Id);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseMuscleGroup.FunctionPercentage);
	}

	[TestCase(0, StringStaticGuid, 1, "Id")]
	[TestCase(1, StringStaticGuid, 0, "FunctionPercentage")]
	[TestCase(1, StringStaticGuid, 101, "FunctionPercentage")]
	[TestCase(1, null, 1, "Concurrency")]
	[TestCase(1, "eee", 1, "Concurrency")]
	[TestCase(1, StringWith51Characters, 1, "Concurrency")]
	public void UpdateExerciseMuscleGroupCommandValidator_ShouldHaveError_WhenInputIsInvalid(int id,
		string? concurrency, double functionPercentage, string testForPropertyName)
	{
		var command =
			new UpdateExerciseMuscleGroupCommand(
				new UpdateExerciseMuscleGroupRequest(id, concurrency, functionPercentage));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Id":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.Id);
				break;
			case "FunctionPercentage":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.FunctionPercentage);
				break;
			case "Concurrency":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseMuscleGroup.Concurrency);
				break;
		}
	}
}
