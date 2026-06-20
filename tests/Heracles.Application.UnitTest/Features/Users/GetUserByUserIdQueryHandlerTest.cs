using FluentResults;
using Heracles.Application.Features.Users.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.Users;

[TestFixture(Category = "Users")]
public class GetUserByUserIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public new void Setup()
	{
		_users = UsersRepository.Query().ToList();
		_handler = new GetUserByUserIdQueryHandler(UsersRepository, CurrentUserServiceMock);
	}

	private List<User> _users;
	private GetUserByUserIdQueryHandler _handler;

	[Test]
	public async Task GetUserByUserIdQueryHandler_ShouldReturnUser_WhenInputIsValid()
	{
		// Arrange
		var user = _users.First();
		var query = new GetUserByUserIdQuery(user.UserId);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<User>>());
		Assert.That(result.Value.UserId, Is.EqualTo(user.UserId));
		Assert.That(result.Value.Email, Is.EqualTo(user.Email));
		Assert.That(result.Value.IsAdmin, Is.EqualTo(user.IsAdmin));
	}

	[Test]
	public async Task GetUserByUserIdQueryHandler_ShouldReturnNull_WhenUserNotFound()
	{
		// Arrange
		var query = new GetUserByUserIdQuery("12345678-1234-1234-1234-123456789012");

		// Act
		// new handler since the user is not admin
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<User>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task GetUserByUserIdQueryHandler_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
	{
		// Arrange
		SetAnonymousUser();
		var user = _users.First(x => !x.IsAdmin);


		var query = new GetUserByUserIdQuery(user.UserId);

		// Act
		var handler = new GetUserByUserIdQueryHandler(UsersRepository, CurrentUserServiceMock);
		var result = await handler.Handle(query, CancellationToken.None);


		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<User>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
