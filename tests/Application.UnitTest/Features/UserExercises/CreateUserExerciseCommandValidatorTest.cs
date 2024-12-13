using Application.Features.UserExercises.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "FluentValidation")]
public class CreateUserExerciseCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new CreateUserExerciseCommandValidator();
	}

	private CreateUserExerciseCommandValidator _validator;

	[Test]
	public void CreateUserExerciseCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var request = new CreateUserExerciseRequest
		{
			UserId = "123456789012345678901234567890123456",
			ExerciseTypeId = 1
		};
		var command = new CreateUserExerciseCommand(request);
		var result = _validator.TestValidate(command);

		result.ShouldNotHaveValidationErrorFor(x => x.UserExercise.UserId);
	}

	[TestCase("", 1, "UserId")]
	[TestCase(null, 1, "UserId")]
	[TestCase(StringWith256Characters, 1, "UserId")]
	[TestCase(StringStaticGuid, 0, "ExerciseTypeId")]
	[TestCase(StringStaticGuid, -1, "ExerciseTypeId")]
	public void CreateUserExerciseCommandValidator_ShouldHaveError_WhenInputIsInvalid(string? userId,
		int exerciseTypeId, string testForPropertyName)
	{
		var request = new CreateUserExerciseRequest
		{
			UserId = userId!,
			ExerciseTypeId = exerciseTypeId
		};
		var command = new CreateUserExerciseCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "UserId":
				result.ShouldHaveValidationErrorFor(x => x.UserExercise.UserId);
				break;
			case "ExerciseTypeId":
				result.ShouldHaveValidationErrorFor(x => x.UserExercise.ExerciseTypeId);
				break;
		}
	}
}
