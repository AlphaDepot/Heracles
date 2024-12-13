using Application.Features.MuscleGroups.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "FluentValidation")]
public class CreateMuscleGroupCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new CreateMuscleGroupCommandValidator();
	}

	private CreateMuscleGroupCommandValidator _validator;

	[TestCase("Name")]
	public void CreateMuscleGroupCommandValidator_ShouldNotHaveError_WhenInputIsValid(string name)
	{
		var request = new CreateMuscleGroupRequest(name);
		var command = new CreateMuscleGroupCommand(request);
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.MuscleGroup.Name);
	}

	[TestCase("", "Name")]
	[TestCase(null, "Name")]
	[TestCase(StringWith256Characters, "Name")]
	public void CreateMuscleGroupCommandValidator_ShouldHaveError_WhenInputIsInvalid(string? name,
		string testForPropertyName)
	{
		var request = new CreateMuscleGroupRequest(name!);
		var command = new CreateMuscleGroupCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Name":
				result.ShouldHaveValidationErrorFor(x => x.MuscleGroup.Name);
				break;
		}
	}
}
