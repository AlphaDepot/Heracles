using FluentResults;
using Heracles.Application.Features.Users.Commands;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests.Users;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Application.UnitTest.Features.Users;

[TestFixture(Category = "Users")]
public class CreateOrUpdateCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_users = UsersRepository.QueryTracking().ToList();
		_handler = new CreateOrUpdateCommandHandler(UsersRepository, CurrentUserServiceMock);
	}

	private List<User> _users;
	private CreateOrUpdateCommandHandler _handler;

	[Test]
	public async Task CreateOrUpdateCommandHandler_ShouldCreateNewUser_WhenUserDoesNotExist()
	{
		// Arrange
		var user = _users.First();
		var createRequest = new CreateOrUpdateRequest(user.UserId, user.Email, user.IsAdmin);
		var command = new CreateOrUpdateCommand(createRequest);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var newUser = await UsersRepository.Query().FirstOrDefaultAsync();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(newUser, Is.Not.Null);
		Assert.That(newUser.UserId, Is.EqualTo(createRequest.UserId));
		Assert.That(newUser.Email, Is.EqualTo(createRequest.Email));
		Assert.That(newUser.IsAdmin, Is.EqualTo(createRequest.IsAdmin));
	}

	[Test]
	public async Task CreateOrUpdateCommandHandler_ShouldUpdateUser_WhenUserExists()
	{
		// Arrange
		var user = _users.First();
		const string newEmail = "newemail@test.com";
		var createRequest = new CreateOrUpdateRequest(user.UserId, newEmail, user.IsAdmin);
		var command = new CreateOrUpdateCommand(createRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var updatedUser = await UsersRepository.Query().FirstOrDefaultAsync();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Value, Is.True);
		Assert.That(updatedUser, Is.Not.Null);
		Assert.That(updatedUser.UserId, Is.EqualTo(user.UserId));
		Assert.That(updatedUser.Email, Is.EqualTo(newEmail));
		Assert.That(updatedUser.IsAdmin, Is.EqualTo(user.IsAdmin));
	}

	[Test]
	public async Task CreateOrUpdateCommandHandler_ShouldReturnUnauthorized_WhenUserIsNotAuthorized()
	{
		// Arrange
		var user = _users.Last();
		var createRequest = new CreateOrUpdateRequest(user.UserId, user.Email, user.IsAdmin);
		var command = new CreateOrUpdateCommand(createRequest);

		SetCurrentUser(_users.Last());

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
