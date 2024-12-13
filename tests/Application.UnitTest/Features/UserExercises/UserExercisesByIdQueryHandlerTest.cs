using Application.Common.Errors;
using Application.Common.Responses;
using Application.Features.UserExercises;
using Application.Features.UserExercises.Queries;
using Application.UnitTest.TestData;
using Microsoft.AspNetCore.Http;

namespace Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "UserExercises")]
public class UserExercisesByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.UserExercises.AddRange(_userExercises);
		DbContext.SaveChanges();

		_handler = new UserExercisesByIdQueryHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();
	private UserExercisesByIdQueryHandler _handler;

	[Test]
	public async Task UserExercisesByIdQueryHandler_ShouldReturnUserExercise()
	{
		// Arrange
		var userExercise = _userExercises.First();
		var query = new UserExercisesByIdQuery(1);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<UserExercise>>());
		Assert.That(result.Value!.Id, Is.EqualTo(1));
		Assert.That(result.Value.UserId, Is.EqualTo(userExercise.UserId));
		Assert.That(result.Value.ExerciseTypeId, Is.EqualTo(userExercise.ExerciseTypeId));
		Assert.That(result.Value.ExerciseType.Name, Is.EqualTo(userExercise.ExerciseType.Name));
		Assert.That(result.Value.ExerciseType.Description, Is.EqualTo(userExercise.ExerciseType.Description));
		Assert.That(result.Value.ExerciseType.ImageUrl, Is.EqualTo(userExercise.ExerciseType.ImageUrl));
	}

	[Test]
	public async Task UserExercisesByIdQueryHandler_ShouldReturnErrorResult_WhenUserExerciseNotFound()
	{
		// Arrange
		var query = new UserExercisesByIdQuery(100);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<UserExercise>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.NotFound));
	}

	[Test]
	public async Task UserExercisesByIdQueryHandler_ShouldReturnErrorResult_WhenUserNotAuthenticated()
	{
		// Arrange
		// - Create a null authenticated user by overriding the HttpContextAccessor
		HttpContextAccessor.HttpContext = new DefaultHttpContext();
		var query = new UserExercisesByIdQuery(1);


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<UserExercise>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
	}
}
