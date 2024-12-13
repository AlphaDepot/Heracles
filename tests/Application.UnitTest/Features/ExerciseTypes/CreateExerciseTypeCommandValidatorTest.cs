using Application.Features.ExerciseTypes.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "FluentValidation")]
public class CreateExerciseTypeCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new CreateExerciseTypeCommandValidator();
	}

	private CreateExerciseTypeCommandValidator _validator;

	[TestCase("Name", "Description", "ImageUrl")]
	[TestCase("Name", null, "ImageUrl")]
	[TestCase("Name", "", "ImageUrl")]
	[TestCase("Name", "Description", null)]
	[TestCase("Name", "Description", "")]
	public void CreateExerciseTypeCommandValidator_ShouldNotHaveError_WhenInputIsValid(string name, string? description,
		string? imageUrl)
	{
		var command = new CreateExerciseTypeCommand(new CreateExerciseTypeRequest(name, description, imageUrl));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseType.Name);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseType.Description);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseType.ImageUrl);
	}

	[TestCase("", "Description", "ImageUrl", "Name")]
	[TestCase(null, "Description", "ImageUrl", "Name")]
	[TestCase(StringWith51Characters, "Description", "ImageUrl", "Name")]
	[TestCase("Name", StringWith1001Characters, "ImageUrl", "Description")]
	[TestCase("Name", "Description", StringWith256Characters, "ImageUrl")]
	public void CreateExerciseTypeCommandValidator_ShouldHaveError_WhenInputIsInvalid(string? name, string description,
		string imageUrl, string testForPropertyName)
	{
		var command = new CreateExerciseTypeCommand(new CreateExerciseTypeRequest(name!, description, imageUrl));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Name":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseType.Name);
				break;
			case "Description":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseType.Description);
				break;
			case "ImageUrl":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseType.ImageUrl);
				break;
		}
	}
}
