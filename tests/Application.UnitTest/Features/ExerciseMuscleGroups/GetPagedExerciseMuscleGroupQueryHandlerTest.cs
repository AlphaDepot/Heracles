using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.ExerciseMuscleGroups;
using Application.Features.ExerciseMuscleGroups.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "ExerciseMuscleGroups")]
public class GetPagedExerciseMuscleGroupQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.ExerciseMuscleGroups.AddRange(_exerciseMuscleGroups.Take(4));
		DbContext.SaveChanges();

		_handler = new GetPagedExerciseMuscleGroupQueryHandler(DbContext);
	}

	private readonly List<ExerciseMuscleGroup> _exerciseMuscleGroups = ExerciseTypeData.ExerciseMuscleGroups();
	private GetPagedExerciseMuscleGroupQueryHandler _handler;

	[Test]
	public async Task GetExerciseMuscleGroupsQueryHandler_ShouldReturnPagedExerciseMuscleGroups()
	{
		// Arrange
		// - Seed data

		// - Create query
		var query = new GetPagedExerciseMuscleGroupQuery(new QueryRequest());


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<ExerciseMuscleGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(4));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Muscle.Name, Is.EqualTo(_exerciseMuscleGroups.First().Muscle.Name));
	}

	[Test]
	public async Task GetExerciseMuscleGroupsQueryHandler_ShouldReturnEmptyPagedExerciseMuscleGroups()
	{
		// Arrange
		DbContext.ExerciseMuscleGroups.RemoveRange(DbContext.ExerciseMuscleGroups);
		await DbContext.SaveChangesAsync();
		var query = new GetPagedExerciseMuscleGroupQuery(new QueryRequest());


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<ExerciseMuscleGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(0));
	}

	[Test]
	public async Task GetExerciseMuscleGroupsQueryHandler_ShouldReturnPagedExerciseMuscleGroupsWithSearchTerm()
	{
		// Arrange
		// - Create query
		var query = new GetPagedExerciseMuscleGroupQuery(new QueryRequest
			{ SearchTerm = _exerciseMuscleGroups.First().Muscle.Name });


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<ExerciseMuscleGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Muscle.Name, Is.EqualTo(_exerciseMuscleGroups.First().Muscle.Name));
	}

	[Test]
	public async Task GetExerciseMuscleGroupsQueryHandler_ShouldReturnPagedExerciseMuscleGroupsWithSortOrder()
	{
		// Arrange
		var sortedExerciseMuscleGroups = _exerciseMuscleGroups.OrderBy(e => e.FunctionPercentage).ToList();
		var query = new GetPagedExerciseMuscleGroupQuery(new QueryRequest
			{ SortBy = "percentage" });

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<ExerciseMuscleGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(4));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Muscle.Name, Is.EqualTo(sortedExerciseMuscleGroups.First().Muscle.Name));
	}

	[Test]
	public async Task
		GetExerciseMuscleGroupsQueryHandler_ShouldReturnPagedExerciseMuscleGroupsWithSortOrderdByDescending()
	{
		// Arrange
		var sortedExerciseMuscleGroups = _exerciseMuscleGroups.OrderByDescending(e => e.FunctionPercentage).ToList();
		var query = new GetPagedExerciseMuscleGroupQuery(new QueryRequest
			{ SortBy = "percentage", SortOrder = "Desc" });

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<ExerciseMuscleGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(4));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Muscle.Name, Is.EqualTo(sortedExerciseMuscleGroups.First().Muscle.Name));
	}
}
