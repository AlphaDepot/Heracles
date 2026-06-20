using FluentResults;
using Heracles.Application.Features.Users.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.Users;

[TestFixture(Category = "Users")]
public class RemoveUserCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_users = UsersRepository.QueryTracking().ToList();
		_handler = new RemoveUserCommandHandler(UsersRepository);
	}

	private List<User> _users;
	private RemoveUserCommandHandler _handler;

	[Test]
	public async Task RemoveUserCommandHandler_ShouldReturnTrue()
	{
		// Arrange
		var command = new RemoveUserCommand(1);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
	}

	[Test]
	public async Task RemoveUserCommandHandler_ShouldReturnErrorResult_WhenUserNotFound()
	{
		// Arrange
		var command = new RemoveUserCommand(100);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task RemoveUserCommandHandler_ShouldReturnErrorResult_WhenUserIsNotAdmin()
	{
		// Arrange
		var command = new RemoveUserCommand(100, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
