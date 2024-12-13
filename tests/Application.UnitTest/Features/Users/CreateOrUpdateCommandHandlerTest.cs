using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Users;
using Application.Features.Users.Commands;
using Application.UnitTest.TestData;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTest.Features.Users;

[TestFixture(Category = "Users")]
public class CreateOrUpdateCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.Users.AddRange(_users);
		DbContext.SaveChanges();
		_handler = new CreateOrUpdateCommandHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<User> _users = UserData.Users();
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
		var newUser = await DbContext.Users.FirstOrDefaultAsync();

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
		var updatedUser = await DbContext.Users.FirstOrDefaultAsync();

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
		var user = _users.First();
		var createRequest = new CreateOrUpdateRequest(user.UserId, user.Email, user.IsAdmin);
		var command = new CreateOrUpdateCommand(createRequest);
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = _users.Last().ToClaimsPrincipal();
		}

		// Act
		var result = await _handler.Handle(command, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<bool>>());
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
