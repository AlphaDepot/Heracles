using Application.Features.EquipmentGroups.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.EquipmentGroups;

[TestFixture(Category = "FluentValidation")]
public class DetachEquipmentCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new DetachEquipmentCommandValidator();
	}

	private DetachEquipmentCommandValidator _validator;

	[Test]
	public void DetachEquipmentCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var request = new DetachEquipmentRequest(1, 1);
		var command = new DetachEquipmentCommand(request);
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.EquipmentRequest.EquipmentGroupId);
	}

	[TestCase(0, 1, "Equipment Group Id")]
	[TestCase(1, 0, "Equipment Id")]
	public void DetachEquipmentCommandValidator_ShouldHaveError_WhenInputIsInvalid(int equipmentGroupId,
		int equipmentId, string testForPropertyName)
	{
		var request = new DetachEquipmentRequest(equipmentGroupId, equipmentId);
		var command = new DetachEquipmentCommand(request);
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
