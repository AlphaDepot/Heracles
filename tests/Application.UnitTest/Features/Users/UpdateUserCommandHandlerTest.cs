using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Users;
using Application.Features.Users.Commands;
using Application.UnitTest.TestData;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTest.Features.Users;

[TestFixture(Category = "Users")]
public class UpdateUserCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.Users.AddRange(_users);
		DbContext.SaveChanges();
		_handler = new UpdateUserCommandHandler(DbContext);
	}

	private readonly List<User> _users = UserData.Users();
	private UpdateUserCommandHandler _handler;

	[Test]
	public async Task UpdateUserCommandHandler_ShouldReturnSuccessResult()
	{
		// Arrange
		var user = _users.First();
		const string updatedEmail = "Updated@email.com";
		var updateRequest = new UpdateUserRequest(user.UserId, updatedEmail, user.IsAdmin);
		var command = new UpdateUserCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);
		var updatedUser = await DbContext.Users.FirstOrDefaultAsync(u => u.UserId == user.UserId);
		Console.WriteLine(user.Id);
		Console.WriteLine(result.Value);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Value, Is.EqualTo(user.Id));
		Assert.That(updatedUser, Is.Not.Null);
		Assert.That(updatedUser.UserId, Is.EqualTo(user.UserId));
		Assert.That(updatedUser.Email, Is.EqualTo(updatedEmail));
	}

	[Test]
	public async Task UpdateUserCommandHandler_ShouldReturnErrorResult_WhenUserNotFound()
	{
		// Arrange
		var updateRequest = new UpdateUserRequest("12345678-1234-1234-1234-123456789012", " email", true);
		var command = new UpdateUserCommand(updateRequest);

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task UpdateUserCommandHandler_ShouldReturnErrorResult_WhenNotAdmin()
	{
		// Arrange
		var user = _users.First();
		var createRequest = new UpdateUserRequest(user.UserId, user.Email, user.IsAdmin);
		var command = new UpdateUserCommand(createRequest, false);


		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<int>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
