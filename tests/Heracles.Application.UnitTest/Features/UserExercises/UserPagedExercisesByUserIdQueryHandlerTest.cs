using FluentResults;
using Heracles.Application.Features.UserExercises.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;

namespace Heracles.Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "UserExercises")]
public class UserPagedExercisesByUserIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_userExercises = UserExercisesRepository.QueryTracking().ToList();
		_handler = new UserPagedExercisesByUserIdQueryHandler(UserExercisesRepository, CurrentUserServiceMock);
	}

	private List<UserExercise> _userExercises;
	private UserPagedExercisesByUserIdQueryHandler _handler;

	[Test]
	public async Task UserPagedExercisesByUserIdQueryHandler_ShouldReturnPagedExercises()
	{
		// Arrange
		var query = new UserPagedExercisesByUserIdQuery(new QueryRequest());

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<UserExercise>>>());
		Assert.That(result.Value, Is.Not.Null);
		Assert.That(result.Value.Data, Is.Not.Null);
		Assert.That(result.Value.Data.Count, Is.GreaterThan(0));
	}


	[Test]
	public async Task UserPagedExercisesByUserIdQueryHandler_ReturnUnauthorizedError_WhenUserIdIsNotInContext()
	{
		// Arrange
		// Set the user to null
		SetAnonymousUser();
		var query = new UserPagedExercisesByUserIdQuery(new QueryRequest());

		// Act
		var handler = new UserPagedExercisesByUserIdQueryHandler(UserExercisesRepository, CurrentUserServiceMock);
		var result = await handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<UserExercise>>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.Unauthorized));
	}

	[Test]
	public async Task UserPagedExercisesByUserIdQueryHandler_ShouldReturnPagedExercisesWithSearchTerm()
	{
		// Arrange
		var query = new UserPagedExercisesByUserIdQuery(
			new QueryRequest { SearchTerm = _userExercises.First().ExerciseType.Name });

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<UserExercise>>>());
		Assert.That(result.Value.Data.First().UserId, Is.EqualTo(_userExercises.First().UserId));
		Assert.That(result.Value.Data.First().ExerciseTypeId, Is.EqualTo(_userExercises.First().ExerciseTypeId));
	}

	[Test]
	public async Task UserPagedExercisesByUserIdQueryHandler_ShouldReturnPagedExercisesWithSort()
	{
		// Arrange
		var sortedUserExercise = _userExercises.OrderBy(x => x.Repetitions).ToList();
		var query = new UserPagedExercisesByUserIdQuery(new QueryRequest { SortBy = "Repetitions" });

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<UserExercise>>>());
		Assert.That(result.Value.Data.First().UserId, Is.EqualTo(sortedUserExercise.First().UserId));
		Assert.That(result.Value.Data.First().Repetitions, Is.EqualTo(sortedUserExercise.First().Repetitions));
	}

	[Test]
	public async Task UserPagedExercisesByUserIdQueryHandler_ShouldReturnPagedExercisesWithSort_ByRepetitionDecending()
	{
		// Arrange
		var sortedUserExercise = _userExercises.OrderByDescending(x => x.Repetitions).ToList();
		var query = new UserPagedExercisesByUserIdQuery(new QueryRequest
			{ SortBy = "Repetitions", SortOrder = "desc" });

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<UserExercise>>>());
		Assert.That(result.Value.Data.First().UserId, Is.EqualTo(sortedUserExercise.First().UserId));
		Assert.That(result.Value.Data.First().Repetitions, Is.EqualTo(sortedUserExercise.First().Repetitions));
	}
}
