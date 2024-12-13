using Application.Features.UserExerciseHistories.Commands;
using FluentValidation.TestHelper;

namespace Application.UnitTest.Features.UserExerciseHistories;

[TestFixture(Category = "FluentValidation")]
public class UpdateUserExerciseHistoryCommandValidatorTest : FluentValidationBaseUnitTest
{
	[SetUp]
	public void Setup()
	{
		_validator = new UpdateUserExerciseHistoryCommandValidator();
	}

	private UpdateUserExerciseHistoryCommandValidator _validator;

	[Test]
	public void UpdateUserExerciseHistoryCommandValidator_ShouldNotHaveError_WhenInputIsValid()
	{
		var userExerciseHistory = new UpdateUserExerciseHistoryRequest
		{
			Id = 1,
			Weight = 10,
			Repetition = 10,
			Concurrency = StringStaticGuid,
			UserExerciseId = 1,
			UserId = StringStaticGuid
		};
		var command = new UpdateUserExerciseHistoryCommand(userExerciseHistory);
		var result = _validator.TestValidate(command);
		result.ShouldNotHaveValidationErrorFor(x => x.UserExerciseHistory.Id);
		result.ShouldNotHaveValidationErrorFor(x => x.UserExerciseHistory.Weight);
		result.ShouldNotHaveValidationErrorFor(x => x.UserExerciseHistory.Repetition);
		result.ShouldNotHaveValidationErrorFor(x => x.UserExerciseHistory.Concurrency);
		result.ShouldNotHaveValidationErrorFor(x => x.UserExerciseHistory.UserExerciseId);
	}

	[TestCase(0, 1, 10, 10, StringStaticGuid, StringStaticGuid, "Id")]
	[TestCase(1, 0, 10, 10, StringStaticGuid, StringStaticGuid, "UserExerciseId")]
	[TestCase(1, 1, 10, 10, "", StringStaticGuid, "Concurrency")]
	[TestCase(1, 1, 10, 10, null, StringStaticGuid, "Concurrency")]
	[TestCase(1, 1, 10, 10, StringWith51Characters, StringStaticGuid, "Concurrency")]
	[TestCase(1, 1, 10, 10, StringWith51Characters, StringStaticGuid, "Concurrency")]
	[TestCase(1, 1, 10, 10, StringStaticGuid, "", "UserId")]
	[TestCase(1, 1, 10, 10, StringStaticGuid, null, "UserId")]
	[TestCase(1, 1, 10, 10, StringStaticGuid, StringWith51Characters, "UserId")]
	[TestCase(1, 1, -1, 10, StringStaticGuid, StringStaticGuid, "Weight")]
	[TestCase(1, 1, 10, -1, StringStaticGuid, StringStaticGuid, "Repetition")]
	public void UpdateUserExerciseHistoryCommandValidator_ShouldHaveError_WhenInputIsInvalid(int id, int userExerciseId,
		double weight, int repetition, string? concurrency, string? userId, string testForPropertyName)
	{
		var userExerciseHistory = new UpdateUserExerciseHistoryRequest
		{
			Id = id,
			Weight = weight,
			Repetition = repetition,
			Concurrency = concurrency!,
			UserExerciseId = userExerciseId,
			UserId = userId!
		};
		var command = new UpdateUserExerciseHistoryCommand(userExerciseHistory);
		var result = _validator.TestValidate(command);

		switch (testForPropertyName)
		{
			case "Id":
				result.ShouldHaveValidationErrorFor(x => x.UserExerciseHistory.Id);
				break;
			case "UserExerciseId":
				result.ShouldHaveValidationErrorFor(x => x.UserExerciseHistory.UserExerciseId);
				break;
			case "Weight":
				result.ShouldHaveValidationErrorFor(x => x.UserExerciseHistory.Weight);
				break;
			case "Repetition":
				result.ShouldHaveValidationErrorFor(x => x.UserExerciseHistory.Repetition);
				break;
			case "Concurrency":
				result.ShouldHaveValidationErrorFor(x => x.UserExerciseHistory.Concurrency);
				break;
			case "UserId":
				result.ShouldHaveValidationErrorFor(x => x.UserExerciseHistory.UserId);
				break;
		}
	}
}
