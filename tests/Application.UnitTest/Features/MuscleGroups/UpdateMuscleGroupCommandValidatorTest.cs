using Application.Features.MuscleGroups.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "FluentValidation")]
public class UpdateMuscleGroupCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new UpdateMuscleGroupCommandValidator();
	}

	private UpdateMuscleGroupCommandValidator _validator;


	[TestCase(1, "Name", StringStaticGuid)]
	public void UpdateMuscleGroupCommandValidator_ShouldNotHaveError_WhenInputIsValid(int id, string name,
		string concurrency)
	{
		var command = new UpdateMuscleGroupCommand(new UpdateMuscleGroupRequest(id, name, concurrency));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.MuscleGroup.Name);
	}

	[TestCase(1, "", StringStaticGuid, "Name")]
	[TestCase(1, null, StringStaticGuid, "Name")]
	[TestCase(0, "Name", StringStaticGuid, "Id")]
	[TestCase(1, "Name", null, "Concurrency")]
	[TestCase(1, "Name", "eee", "Concurrency")]
	[TestCase(1, "Name", StringWith51Characters, "Concurrency")]
	public void UpdateMuscleGroupCommandValidator_ShouldHaveError_WhenInputIsInvalid(int id, string? name,
		string? concurrency, string testForPropertyName)
	{
		var command = new UpdateMuscleGroupCommand(new UpdateMuscleGroupRequest(id, name!, concurrency));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Id":
				result.ShouldHaveValidationErrorFor(x => x.MuscleGroup.Id);
				break;
			case "Name":
				result.ShouldHaveValidationErrorFor(x => x.MuscleGroup.Name);
				break;
			case "Concurrency":
				result.ShouldHaveValidationErrorFor(x => x.MuscleGroup.Concurrency);
				break;
		}
	}
}
