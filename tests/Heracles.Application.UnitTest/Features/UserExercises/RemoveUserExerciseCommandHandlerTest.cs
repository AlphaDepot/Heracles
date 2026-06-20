using FluentResults;
using Heracles.Application.Features.UserExercises.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "UserExercises")]
public class RemoveUserExerciseCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_userExercises = UserExercisesRepository.Query().ToList();
		_handler = new RemoveUserExerciseCommandHandler(UserExercisesRepository, CurrentUserServiceMock);
	}

	private List<UserExercise> _userExercises;
	private RemoveUserExerciseCommandHandler _handler;

	[Test]
	public async Task RemoveUserExerciseCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var userExercise = _userExercises.First();
		// - Create command
		var command = new RemoveUserExerciseCommand(userExercise.Id);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var userExerciseRemoved = await UserExercisesRepository.GetByIdAsync(userExercise.Id);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(userExerciseRemoved, Is.Null);
	}

	[Test]
	public async Task RemoveUserExerciseCommandHandler_ShouldReturnErrorResult_WhenUserExerciseNotFound()
	{
		// Arrange
		var command = new RemoveUserExerciseCommand(10000);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task RemoveUserExerciseCommandHandler_ShouldReturnErrorResult_WhenUserIsNotOwner()
	{
		// Arrange
		var userExercise = _userExercises.Last();
		var command = new RemoveUserExerciseCommand(userExercise.Id);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
