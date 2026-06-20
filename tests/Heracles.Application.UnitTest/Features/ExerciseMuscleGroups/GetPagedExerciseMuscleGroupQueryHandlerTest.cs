using FluentResults;
using Heracles.Application.Features.ExerciseMuscleGroups.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;

namespace Heracles.Application.UnitTest.Features.ExerciseMuscleGroups;

[TestFixture(Category = "ExerciseMuscleGroups")]
public class GetPagedExerciseMuscleGroupQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_exerciseMuscleGroups = ExerciseMuscleGroupsRepository.QueryTracking().ToList();
		_handler = new GetPagedExerciseMuscleGroupQueryHandler(ExerciseMuscleGroupsRepository);
	}

	private List<ExerciseMuscleGroup> _exerciseMuscleGroups;
	private GetPagedExerciseMuscleGroupQueryHandler _handler;

	[Test]
	public async Task GetExerciseMuscleGroupsQueryHandler_ShouldReturnPagedExerciseMuscleGroups()
	{
		// Arrange
		// - Create query
		var query = new GetPagedExerciseMuscleGroupQuery(new QueryRequest());


		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<ExerciseMuscleGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(_exerciseMuscleGroups.Count));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));

	}

	[Test]
	public async Task GetExerciseMuscleGroupsQueryHandler_ShouldReturnEmptyPagedExerciseMuscleGroups()
	{
		// Arrange
		// Remove all items
		foreach (var item in _exerciseMuscleGroups)
		{
			await ExerciseMuscleGroupsRepository.RemoveAsync(item);
		}

		await ExerciseMuscleGroupsRepository.SaveChangesAsync();
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
		Assert.That(result.Value.Data.Count, Is.EqualTo(sortedExerciseMuscleGroups.Count));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));

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
		Assert.That(result.Value.Data.Count, Is.EqualTo(sortedExerciseMuscleGroups.Count));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));

	}
}
