using FluentResults;
using Heracles.Application.Features.UserExercises.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "UserExercises")]
public class UserExercisesByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_userExercises = UserExercisesRepository.QueryTracking().ToList();
		_handler = new UserExercisesByIdQueryHandler(UserExercisesRepository, CurrentUserServiceMock);
	}

	private List<UserExercise> _userExercises;
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
		Assert.That(result.Value.ExerciseType.Images, Is.EqualTo(userExercise.ExerciseType.Images));
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
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}

	[Test]
	public async Task UserExercisesByIdQueryHandler_ShouldReturnErrorResult_WhenUserNotAuthenticated()
	{
		// Arrange
		// - Create a null authenticated user by overriding the HttpContextAccessor
		SetAnonymousUser();
		var query = new UserExercisesByIdQuery(1);


		// Act
		var handler = new UserExercisesByIdQueryHandler(UserExercisesRepository, CurrentUserServiceMock);
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<UserExercise>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}
}
