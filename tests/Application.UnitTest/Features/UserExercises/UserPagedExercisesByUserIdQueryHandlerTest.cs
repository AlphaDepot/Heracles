using Application.Common.Errors;
using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.UserExercises;
using Application.Features.UserExercises.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.UserExercises;

[TestFixture(Category = "UserExercises")]
public class UserPagedExercisesByUserIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.UserExercises.AddRange(_userExercises);
		DbContext.SaveChanges();

		_handler = new UserPagedExercisesByUserIdQueryHandler(DbContext, HttpContextAccessor);
	}

	private readonly List<UserExercise> _userExercises = UserExerciseData.UserExercises();
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
		HttpContextAccessor.HttpContext!.User = null!;
		var userId = UserData.Users().Last().UserId;
		var query = new UserPagedExercisesByUserIdQuery(new QueryRequest());

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<UserExercise>>>());
		Assert.That(result.IsFailure, Is.True);
		Assert.That(result.Error, Is.Not.Null);
		Assert.That(result.Error, Is.EqualTo(ErrorTypes.Unauthorized));
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
