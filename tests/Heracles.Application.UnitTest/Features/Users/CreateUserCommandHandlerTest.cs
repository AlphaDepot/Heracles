using FluentResults;
using Heracles.Application.Features.Users.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.Users;

namespace Heracles.Application.UnitTest.Features.Users;

[TestFixture(Category = "Users")]
public class CreateUserCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_users = UsersRepository.Query().ToList();
		_handler = new CreateUserCommandHandler(UsersRepository);
	}

	private List<User> _users;
	private CreateUserCommandHandler _handler;

	[Test]
	public async Task CreateUserCommandHandler_ShouldReturnIntId()
	{
		// Arrange
		var createRequest = new CreateUserRequest("Unique User Id", "Test@test.email", true);
		var command = new CreateUserCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.GreaterThan(0));
	}

	[Test]
	public async Task CreateUserCommandHandler_ShouldReturnErrorResult_WhenUserIdIsDuplicated()
	{
		// Arrange
		var user = _users.First();
		var createRequest = new CreateUserRequest(user.UserId, user.Email, user.IsAdmin);
		var command = new CreateUserCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NamingConflict));
	}

	[Test]
	public async Task CreateUserCommandHandler_ShouldReturnErrorResult_WhenNotAdmin()
	{
		// Arrange
		var user = _users.First();
		var createRequest = new CreateUserRequest(user.UserId, user.Email, user.IsAdmin);
		var command = new CreateUserCommand(createRequest, false);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
