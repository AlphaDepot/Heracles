using Application.Features.Equipments.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.Equipments;

[TestFixture(Category = "FluentValidation")]
public class UpdateEquipmentCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new UpdateEquipmentCommandValidator();
	}

	private UpdateEquipmentCommandValidator _validator;

	[TestCase(1, "Type", StringStaticGuid, 1, 1)]
	[TestCase(1, "Type", StringStaticGuid, 0, 1)]
	[TestCase(1, "Type", StringStaticGuid, 1, 0)]
	[TestCase(1, "Type", StringStaticGuid, 0, 0)]
	[TestCase(1, "Type", StringStaticGuid, 1.1, 1.1)]
	public void UpdateEquipmentCommandValidator_ShouldNotHaveError_WhenInputIsValid(int id, string type,
		string concurrency, double weight, double resistance)
	{
		var command = new UpdateEquipmentCommand(new UpdateEquipmentRequest(id, type, concurrency, weight, resistance));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.Equipment.Id);
		result.ShouldNotHaveValidationErrorFor(x => x.Equipment.Type);
		result.ShouldNotHaveValidationErrorFor(x => x.Equipment.Concurrency);
		result.ShouldNotHaveValidationErrorFor(x => x.Equipment.Weight);
		result.ShouldNotHaveValidationErrorFor(x => x.Equipment.Resistance);
	}

	[TestCase(0, "Type", StringStaticGuid, 1, 1, "Id")]
	[TestCase(1, "", StringStaticGuid, 1, 1, "Type")]
	[TestCase(1, null, StringStaticGuid, 1, 1, "Type")]
	[TestCase(1, "Type", null, 1, 1, "Concurrency")]
	[TestCase(1, "Type", "eee", 1, 1, "Concurrency")]
	[TestCase(1, "Type", StringWith51Characters, 1, 1, "Concurrency")]
	public void UpdateEquipmentCommandValidator_ShouldHaveError_WhenInputIsInvalid(int id, string? type,
		string? concurrency, double weight, double resistance, string testForPropertyName)
	{
		var command =
			new UpdateEquipmentCommand(new UpdateEquipmentRequest(id, type!, concurrency, weight, resistance));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Id":
				result.ShouldHaveValidationErrorFor(x => x.Equipment.Id);
				break;
			case "Type":
				result.ShouldHaveValidationErrorFor(x => x.Equipment.Type);
				break;
			case "Concurrency":
				result.ShouldHaveValidationErrorFor(x => x.Equipment.Concurrency);
				break;
		}
	}
}
