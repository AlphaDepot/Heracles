using Application.Features.UserExercises.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "FluentValidation")]
public class UpdateUserExerciseCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new UpdateUserExerciseCommandValidator();
	}

	private UpdateUserExerciseCommandValidator _validator;


	[Test]
	public void UpdateUserExerciseCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var command = new UpdateUserExerciseCommand(new UpdateUserExerciseRequest
		{
			Id = 1,
			Concurrency = StringStaticGuid,
			StaticResistance = 1
		});
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.UserExercise.Id);
	}

	[TestCase(0, StringStaticGuid, "Id")]
	[TestCase(-1, StringStaticGuid, "Id")]
	[TestCase(1, "", "Concurrency")]
	[TestCase(1, null, "Concurrency")]
	[TestCase(1, "eee", "Concurrency")]
	[TestCase(1, StringWith51Characters, "Concurrency")]
	public void UpdateUserExerciseCommandValidator_ShouldHaveError_WhenInputIsInvalid(int id, string? concurrency,
		string testForPropertyName)
	{
		var command = new UpdateUserExerciseCommand(new UpdateUserExerciseRequest
		{
			Id = id,
			Concurrency = concurrency!,
			StaticResistance = 1
		});
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Id":
				result.ShouldHaveValidationErrorFor(x => x.UserExercise.Id);
				break;
			case "Concurrency":
				result.ShouldHaveValidationErrorFor(x => x.UserExercise.Concurrency);
				break;
		}
	}
}
