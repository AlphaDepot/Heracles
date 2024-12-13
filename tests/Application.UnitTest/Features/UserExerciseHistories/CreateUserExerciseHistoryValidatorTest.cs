using Application.Features.UserExerciseHistories;
using Application.Features.UserExerciseHistories.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.UserExerciseHistories;
[TestFixture(Category = "FluentValidation")]
public class CreateUserExerciseHistoryTest : FluentValidationBaseUnitTest
{
	private CreateUserExerciseHistoryValidator _validator;
	[SetUp]
	public void Setup()
	{
		_validator = new CreateUserExerciseHistoryValidator();
	}

	[Test]
	public void CreateUserExerciseHistoryValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var userExerciseHistory = new CreateUserExerciseHistoryRequest(1, 10, 10, StringStaticGuid);
		var command = new CreateUserExerciseHistoryCommand(userExerciseHistory);
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.UserExerciseHistory.UserExerciseId);
		result.ShouldNotHaveValidationErrorFor(x => x.UserExerciseHistory.Weight);
		result.ShouldNotHaveValidationErrorFor(x => x.UserExerciseHistory.Repetition);
	}

	[TestCase  (0, 10, 10,  StringStaticGuid, "UserExerciseId")]
	[TestCase  (1, -1, 10, StringStaticGuid, "Weight")]
	[TestCase  (1, 10, -1, StringStaticGuid, "Repetition")]
	[TestCase  (1, 10, 10, "", "UserId")]
	[TestCase  (1, 10, 10, null, "UserId")]
	[TestCase  (1, 10, 10, StringWith51Characters, "UserId")]
	public void CreateUserExerciseHistoryValidator_ShouldHaveError_WhenInputIsInvalid(int userExerciseId, double weight, int repetition, string? userId, string testForPropertyName)
	{
		var userExerciseHistory = new CreateUserExerciseHistoryRequest(userExerciseId, weight, repetition, userId!);
		var command = new CreateUserExerciseHistoryCommand(userExerciseHistory);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "UserExerciseId":
				result.ShouldHaveValidationErrorFor(x => x.UserExerciseHistory.UserExerciseId);
				break;
			case "Weight":
				result.ShouldHaveValidationErrorFor(x => x.UserExerciseHistory.Weight);
				break;
			case "Repetition":
				result.ShouldHaveValidationErrorFor(x => x.UserExerciseHistory.Repetition);
				break;
			case "UserId":
				result.ShouldHaveValidationErrorFor(x => x.UserExerciseHistory.UserId);
				break;
		}
	}

}
