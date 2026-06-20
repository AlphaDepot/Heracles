using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.ExerciseTypes;

/// <summary>
///     Represents the groupRequest to update an <see cref="ExerciseType" />.
/// </summary>
/// <param name="Id"> The id of the <see cref="ExerciseType" /> to update.</param>
/// <param name="Name"> The name of the <see cref="ExerciseType" /> to update.</param>
/// <param name="Concurrency"> The concurrency stamp of the <see cref="ExerciseType" /> to update.</param>
/// <param name="Description"> The description of the <see cref="ExerciseType" /> to update.</param>
/// <param name="Images"> The image url of the <see cref="ExerciseType" /> to update.</param>
public record UpdateExerciseTypeRequest(
	int Id,
	string Name,
	string? Concurrency,
	string? Description,
	List<string>? Images);
