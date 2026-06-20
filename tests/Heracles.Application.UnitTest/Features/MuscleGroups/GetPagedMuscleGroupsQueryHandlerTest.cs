using FluentResults;
using Heracles.Application.Features.MuscleGroups.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests;
using Heracles.Shared.Responses;

namespace Heracles.Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "MuscleGroups")]
public class GetPagedMuscleGroupsQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_muscleGroups = MuscleGroupsRepository.Query().ToList();
		_handler = new GetPagedMuscleGroupsQueryHandler(MuscleGroupsRepository);
	}

	private List<MuscleGroup> _muscleGroups;
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
		Assert.That(result.Value.Data.Count, Is.EqualTo(_muscleGroups.Count));
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
