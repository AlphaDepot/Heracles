using FluentResults;
using Heracles.Application.Features.ExerciseTypes.Queries;
using Heracles.Domain.Entities;
using Heracles.Shared.Errors;

namespace Heracles.Application.UnitTest.Features.ExerciseTypes;

[TestFixture(Category = "ExerciseTypes")]
public class GetExerciseTypeByIdQueryHandlerTest : HandlerBaseUnitTest
{
	[SetUp]
	public void SetUp()
	{
		_exerciseTypes = ExerciseTypesRepository.Query().ToList();
		_handler = new GetExerciseTypeByIdQueryHandler(ExerciseTypesRepository);
	}

	private List<ExerciseType> _exerciseTypes;
	private GetExerciseTypeByIdQueryHandler _handler;


	[Test]
	public async Task GetExerciseTypeByIdQueryHandler_ShouldReturnExerciseType()
	{
		// Arrange
		var exerciseType = _exerciseTypes.First();
		var query = new GetExerciseTypeByIdQuery(1);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<ExerciseType>>());
		Assert.That(result.Value.Id, Is.EqualTo(1));
		Assert.That(result.Value.Name, Is.EqualTo(exerciseType.Name));
		Assert.That(result.Value.Description, Is.EqualTo(exerciseType.Description));
		Assert.That(result.Value.Images, Is.EqualTo(exerciseType.Images));
	}

	[Test]
	public async Task GetExerciseTypeByIdQueryHandler_ShouldReturnErrorResult_WhenExerciseTypeNotFound()
	{
		// Arrange
		var query = new GetExerciseTypeByIdQuery(1000);

		// Act
		var result = await _handler.Handle(query, CancellationToken.None);

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result, Is.InstanceOf<Result<ExerciseType>>());
		Assert.That(result.IsFailed, Is.True);
		Assert.That(result.Errors.First().Metadata["Type"], Is.Not.Null);
		Assert.That(result.Errors.First().Metadata["Type"], Is.EqualTo(ErrorCodes.NotFound));
	}
}
