using FluentResults;
using Heracles.Application.Features.MuscleGroups.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.MuscleGroups;

[TestFixture(Category = "MuscleGroups")]
public class GetMuscleGroupByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_muscleGroups = MuscleGroupsRepository.Query().ToList();

		_handler = new GetMuscleGroupByIdQueryHandler(MuscleGroupsRepository);
	}

	private List<MuscleGroup> _muscleGroups;
	private GetMuscleGroupByIdQueryHandler _handler;


	[Test]
	public async Task GetMuscleGroupByIdQueryHandler_ShouldReturnMuscleGroup()
	{
		// Arrange
		var muscleGroup = _muscleGroups.First();
		var query = new GetMuscleGroupByIdQuery(1);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<MuscleGroup>>());
		Assert.That(result.Value.Id, Is.EqualTo(1));
		Assert.That(result.Value.Name, Is.EqualTo(muscleGroup.Name));
	}


	[Test]
	public async Task GetMuscleGroupByIdQueryHandler_ShouldReturnErrorResult_WhenMuscleGroupNotFound()
	{
		// Arrange
		var query = new GetMuscleGroupByIdQuery(100);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<MuscleGroup>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
