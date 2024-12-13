using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Users;
using Application.Features.Users.Commands;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.Users;

[TestFixture(Category = "Users")]
public class RemoveUserCommandHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		// Seed data
		DbContext.Users.AddRange(_users);
		DbContext.SaveChanges();
		_handler = new RemoveUserCommandHandler(DbContext);
	}

	private readonly List<User> _users = UserData.Users();
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
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
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
