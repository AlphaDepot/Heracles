using Application.Features.MuscleFunctions.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.MuscleFunctions;

[TestFixture(Category = "FluentValidation")]
public class UpdateMuscleFunctionCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new UpdateMuscleFunctionCommandValidator();
	}

	private UpdateMuscleFunctionCommandValidator _validator;

	[TestCase(1, "Name", StringStaticGuid)]
	public void UpdateMuscleFunctionCommandValidator_ShouldNotHaveError_WhenInputIsValid(int id, string name,
		string concurrency)
	{
		var command = new UpdateMuscleFunctionCommand(new UpdateMuscleFunctionRequest(id, name, concurrency));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.MuscleFunction.Id);
		result.ShouldNotHaveValidationErrorFor(x => x.MuscleFunction.Name);
		result.ShouldNotHaveValidationErrorFor(x => x.MuscleFunction.Concurrency);
	}

	[TestCase(1, "", StringStaticGuid, "Name")]
	[TestCase(1, null, StringStaticGuid, "Name")]
	[TestCase(0, "Name", StringStaticGuid, "Id")]
	[TestCase(1, "Name", null, "Concurrency")]
	[TestCase(1, "Name", "eee", "Concurrency")]
	[TestCase(1, "Name", StringWith51Characters, "Concurrency")]
	public void UpdateMuscleFunctionCommandValidator_ShouldHaveError_WhenInputIsInvalid(int id, string? name,
		string? concurrency, string testForPropertyName)
	{
		var command = new UpdateMuscleFunctionCommand(new UpdateMuscleFunctionRequest(id, name!, concurrency));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Id":
				result.ShouldHaveValidationErrorFor(x => x.MuscleFunction.Id);
				break;
			case "Name":
				result.ShouldHaveValidationErrorFor(x => x.MuscleFunction.Name);
				break;
			case "Concurrency":
				result.ShouldHaveValidationErrorFor(x => x.MuscleFunction.Concurrency);
				break;
		}
	}
}
