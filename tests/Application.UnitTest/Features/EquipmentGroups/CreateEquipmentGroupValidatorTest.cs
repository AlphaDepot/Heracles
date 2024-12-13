using Application.Features.EquipmentGroups.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "FluentValidation")]
public class CreateEquipmentGroupValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new CreateEquipmentGroupValidator();
	}

	private CreateEquipmentGroupValidator _validator;

	[TestCase("Group Name")]
	public void CreateEquipmentGroupValidator_ShouldNotHaveError_WhenInputIsValid(string name)
	{
		var command = new CreateEquipmentGroupCommand(new CreateEquipmentGroupRequest(name));
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.EquipmentGroup.Name);
	}

	[TestCase("", "Name")]
	[TestCase(null, "Name")]
	[TestCase(StringWith256Characters, "Name")]
	public void CreateEquipmentGroupValidator_ShouldHaveError_WhenInputIsInvalid(string? name,
		string testForPropertyName)
	{
		var command = new CreateEquipmentGroupCommand(new CreateEquipmentGroupRequest(name!));
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Name":
				result.ShouldHaveValidationErrorFor(x => x.EquipmentGroup.Name);
				break;
		}
	}
}
