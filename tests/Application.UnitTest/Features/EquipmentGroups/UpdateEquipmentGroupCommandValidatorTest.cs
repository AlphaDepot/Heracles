using Application.Features.EquipmentGroups.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "FluentValidation")]
public class UpdateEquipmentGroupCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new UpdateEquipmentGroupCommandValidator();
	}

	private UpdateEquipmentGroupCommandValidator _validator;

	[TestCase(1, "Name", StringStaticGuid)]
	public void UpdateEquipmentGroupCommandValidator_ShouldNotHaveError_WhenInputIsValid(int id, string name,
		string concurrency)
	{
		var command = new UpdateEquipmentGroupCommand(new UpdateEquipmentGroupRequest(id, name, concurrency));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.EquipmentGroup.Id);
		result.ShouldNotHaveValidationErrorFor(x => x.EquipmentGroup.Name);
		result.ShouldNotHaveValidationErrorFor(x => x.EquipmentGroup.Concurrency);
	}

	[TestCase(0, "Name", StringStaticGuid, "Id")]
	[TestCase(1, "", StringStaticGuid, "Name")]
	[TestCase(1, null, StringStaticGuid, "Name")]
	[TestCase(1, "Name", null, "Concurrency")]
	[TestCase(1, "Name", "eee", "Concurrency")]
	[TestCase(1, "Name", StringWith51Characters, "Concurrency")]
	public void UpdateEquipmentGroupCommandValidator_ShouldHaveError_WhenInputIsInvalid(int id, string? name,
		string? concurrency, string testForPropertyName)
	{
		var command = new UpdateEquipmentGroupCommand(new UpdateEquipmentGroupRequest(id, name!, concurrency));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Id":
				result.ShouldHaveValidationErrorFor(x => x.EquipmentGroup.Id);
				break;
			case "Name":
				result.ShouldHaveValidationErrorFor(x => x.EquipmentGroup.Name);
				break;
			case "Concurrency":
				result.ShouldHaveValidationErrorFor(x => x.EquipmentGroup.Concurrency);
				break;
		}
	}
}
