using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.ExerciseTypes;

/// <summary>
///     Creates a new <see cref="ExerciseType" />.
/// </summary>
/// <param name="Name"> The name of the <see cref="ExerciseType" />.</param>
/// <param name="Description"> The description of the <see cref="ExerciseType" />.</param>
/// <param name="Images"> The image urls of the <see cref="ExerciseType" />.</param>
public record CreateExerciseTypeRequest(string Name, string? Description, List<string>? Images);
