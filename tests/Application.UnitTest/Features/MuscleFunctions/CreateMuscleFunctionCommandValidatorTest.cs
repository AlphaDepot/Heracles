using Application.Features.MuscleFunctions.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "FluentValidation")]
public class CreateMuscleFunctionCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new CreateMuscleFunctionCommandValidator();
	}

	private CreateMuscleFunctionCommandValidator _validator;

	[TestCase("Name")]
	public void CreateMuscleFunctionCommandValidator_ShouldNotHaveError_WhenInputIsValid(string name)
	{
		var command = new CreateMuscleFunctionCommand(new CreateMuscleFunctionRequest(name));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.MuscleFunction.Name);
	}

	[TestCase("", "Name")]
	[TestCase(null, "Name")]
	[TestCase(StringWith256Characters, "Name")]
	public void CreateMuscleFunctionCommandValidator_ShouldHaveError_WhenInputIsInvalid(string? name,
		string testForPropertyName)
	{
		var command = new CreateMuscleFunctionCommand(new CreateMuscleFunctionRequest(name!));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Name":
				result.ShouldHaveValidationErrorFor(x => x.MuscleFunction.Name);
				break;
		}
	}
}
