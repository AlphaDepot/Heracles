using Application.Features.ExerciseTypes.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "FluentValidation")]
public class UpdateExerciseTypeCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new UpdateExerciseTypeCommandValidator();
	}

	private UpdateExerciseTypeCommandValidator _validator;

	[TestCase(1, "Name", StringStaticGuid, "Description", "ImageUrl")]
	[TestCase(1, "Name", StringStaticGuid, null, "ImageUrl")]
	[TestCase(1, "Name", StringStaticGuid, "", "ImageUrl")]
	[TestCase(1, "Name", StringStaticGuid, "Description", null)]
	[TestCase(1, "Name", StringStaticGuid, "Description", "")]
	public void UpdateExerciseTypeCommandValidator_ShouldNotHaveError_WhenInputIsValid(int id, string name,
		string concurrency, string? description, string? imageUrl)
	{
		var command =
			new UpdateExerciseTypeCommand(new UpdateExerciseTypeRequest(id, name, concurrency, description, imageUrl));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseType.Id);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseType.Name);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseType.Description);
		result.ShouldNotHaveValidationErrorFor(x => x.ExerciseType.ImageUrl);
	}

	[TestCase(1, "", StringStaticGuid, "Description", "ImageUrl", "Name")]
	[TestCase(1, null, StringStaticGuid, "Description", "ImageUrl", "Name")]
	[TestCase(1, StringWith256Characters, StringStaticGuid, "Description", "ImageUrl", "Name")]
	[TestCase(1, "Name", StringStaticGuid, StringWith1001Characters, "ImageUrl", "Description")]
	[TestCase(1, "Name", StringStaticGuid, "Description", StringWith256Characters, "ImageUrl")]
	[TestCase(1, "Name", null, "Description", "ImageUrl", "Concurrency")]
	[TestCase(1, "Name", "eee", "Description", "ImageUrl", "Concurrency")]
	[TestCase(1, "Name", StringWith51Characters, "Description", "ImageUrl", "Concurrency")]
	public void UpdateExerciseTypeCommandValidator_ShouldHaveError_WhenInputIsInvalid(int id, string? name,
		string? concurrency, string description, string imageUrl, string testForPropertyName)
	{
		var command =
			new UpdateExerciseTypeCommand(new UpdateExerciseTypeRequest(id, name!, concurrency, description, imageUrl));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Id":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseType.Id);
				break;
			case "Name":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseType.Name);
				break;
			case "Description":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseType.Description);
				break;
			case "ImageUrl":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseType.ImageUrl);
				break;
			case "Concurrency":
				result.ShouldHaveValidationErrorFor(x => x.ExerciseType.Concurrency);
				break;
		}
	}
}
