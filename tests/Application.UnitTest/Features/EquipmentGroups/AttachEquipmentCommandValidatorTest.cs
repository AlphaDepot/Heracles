using Application.Features.EquipmentGroups.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.EquipmentGroups;

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
		var request = new AttachEquipmentRequest(1, 1);
		var command = new AttachEquipmentCommand(request);
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.EquipmentRequest.EquipmentGroupId);
	}

	[TestCase(0, 1, "Equipment Group Id")]
	[TestCase(1, 0, "Equipment Id")]
	public void AttachEquipmentCommandValidator_ShouldHaveError_WhenInputIsInvalid(int equipmentGroupId,
		int equipmentId, string testForPropertyName)
	{
		var request = new AttachEquipmentRequest(equipmentGroupId, equipmentId);
		var command = new AttachEquipmentCommand(request);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Equipment Group Id":
				result.ShouldHaveValidationErrorFor(x => x.EquipmentRequest.EquipmentGroupId);
				break;
			case "Equipment Id":
				result.ShouldHaveValidationErrorFor(x => x.EquipmentRequest.EquipmentId);
				break;
		}
	}
}
