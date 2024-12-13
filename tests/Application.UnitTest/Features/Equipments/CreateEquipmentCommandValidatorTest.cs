using Application.Features.Equipments.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.Equipments;

[TestFixture(Category = "FluentValidation")]
public class CreateEquipmentCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new CreateEquipmentCommandValidator();
	}

	private CreateEquipmentCommandValidator _validator;

	[TestCase("Type", 1, 1)]
	[TestCase("Type", 0, 1)]
	[TestCase("Type", 1, 0)]
	public void CreateEquipmentCommandValidator_ShouldNotHaveError_WhenInputIsValid(string type, double weight,
		double resistance)
	{
		var command = new CreateEquipmentCommand(new CreateEquipmentRequest(type, weight, resistance));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.Equipment.Type);
	}

	[TestCase("", 1, 1, "Type")]
	[TestCase(null, 1, 1, "Type")]
	[TestCase(StringWith256Characters, 1, 1, "Type")]
	public void CreateEquipmentCommandValidator_ShouldHaveError_WhenInputIsInvalid(string? type, double weight,
		double resistance, string testForPropertyName)
	{
		var command = new CreateEquipmentCommand(new CreateEquipmentRequest(type!, weight, resistance));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Type":
				result.ShouldHaveValidationErrorFor(x => x.Equipment.Type);
				break;
		}
	}
}
