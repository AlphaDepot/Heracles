using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Users;
using Application.Features.Users.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.Users;

[TestFixture(Category = "Users")]
public class CreateUserCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.Users.AddRange(_users);
		DbContext.SaveChanges();
		_handler = new CreateUserCommandHandler(DbContext);
	}

	private readonly List<User> _users = UserData.Users();
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
		Assert.That(await DbContext.Users.FindAsync(result.Value), Is.Not.Null);
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NamingConflict));
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
