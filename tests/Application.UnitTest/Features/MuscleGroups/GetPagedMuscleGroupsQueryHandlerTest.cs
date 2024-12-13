using Application.Common.Requests;
using Application.Common.Responses;
using Application.Features.MuscleGroups;
using Application.Features.MuscleGroups.Queries;
using Application.UnitTest.TestData;

namespace Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "MuscleGroups")]
public class GetPagedMuscleGroupsQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		DbContext.MuscleGroups.AddRange(_muscleGroups);
		DbContext.SaveChangesAsync();

		_handler = new GetPagedMuscleGroupsQueryHandler(DbContext);
	}

	private readonly List<MuscleGroup> _muscleGroups = ExerciseTypeData.MuscleGroups().Take(3).ToList();
	private GetPagedMuscleGroupsQueryHandler _handler;

	[Test]
	public async Task GetMuscleGroupsQueryHandler_ShouldReturnPagedMuscleGroups()
	{
		// Arrange
		var muscleGroup = _muscleGroups.First();
		var query = new GetPagedMuscleGroupsQuery(new QueryRequest());

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<MuscleGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(3));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Name, Is.EqualTo(muscleGroup.Name));
	}


	[Test]
	public async Task GetMuscleGroupsQueryHandler_ShouldReturnPagedMuscleGroupsWithSearchTerm()
	{
		// Arrange
		var muscleGroup = _muscleGroups.First();
		var query = new GetPagedMuscleGroupsQuery(new QueryRequest { SearchTerm = muscleGroup.Name });

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<PagedResponse<MuscleGroup>>>());
		Assert.That(result.Value.Data.Count, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Id, Is.EqualTo(1));
		Assert.That(result.Value.Data.First().Name, Is.EqualTo(muscleGroup.Name));
	}
}
