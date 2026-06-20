using FluentValidation.TestHelper;
using Heracles.Application.Features.EquipmentGroups.Commands;
using Heracles.Shared.Requests.EquipmentGroups;

namespace Heracles.Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "FluentValidation")]
public class AttachEquipmentCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new AttachEquipmentCommandValidator();
	}

	private AttachEquipmentCommandValidator _validator;

	[Test]
	public void AttachEquipmentCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var request = new AttachEquipmentGroupRequest(1, 1);
		var command = new AttachEquipmentCommand(request);
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.EquipmentGroupRequest.EquipmentGroupId);
	}

	[TestCase(0, 1, "Equipment Group Id")]
	[TestCase(1, 0, "Equipment Id")]
	public void AttachEquipmentCommandValidator_ShouldHaveError_WhenInputIsInvalid(int equipmentGroupId,
		int equipmentId, string testForPropertyName)
	{
		var request = new AttachEquipmentGroupRequest(equipmentGroupId, equipmentId);
		var command = new AttachEquipmentCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Equipment Group Id":
				result.ShouldHaveValidationErrorFor(x => x.EquipmentGroupRequest.EquipmentGroupId);
				break;
			case "Equipment Id":
				result.ShouldHaveValidationErrorFor(x => x.EquipmentGroupRequest.EquipmentId);
				break;
		}
	}
}
