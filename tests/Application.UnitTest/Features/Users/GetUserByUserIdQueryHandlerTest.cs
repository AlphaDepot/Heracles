using System.Text.Json;
using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.Users;
using Application.Features.Users.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.Users;

[TestFixture(Category = "Users")]
public class GetUserByUserIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public new void Setup()
	{
		// Seed data
		DbContext.Users.AddRange(_users);
		DbContext.SaveChanges();
		_handler = new GetUserByUserIdQueryHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<User> _users = UserData.Users();
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

		Console.WriteLine(JsonSerializer.Serialize(result.Error));
		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<User>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task GetUserByUserIdQueryHandler_ShouldReturnUnauthorized_WhenUserIsNotAdmin()
	{
		// Arrange
		var user = _users.Last();
		// override the current user to be non admin
		if (HttpContextAccessor.HttpContext != null)
		{
			HttpContextAccessor.HttpContext.User = user.ToClaimsPrincipal();
		}

		var query = new GetUserByUserIdQuery("12345678-1234-1234-1234-123456789012");

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<User>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
