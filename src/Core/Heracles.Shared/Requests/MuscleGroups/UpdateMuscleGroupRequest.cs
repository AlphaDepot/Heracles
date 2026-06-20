using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.MuscleGroups;

/// <summary>
///     Updates a <see cref="MuscleGroup" />.
/// </summary>
/// <param name="Id"> The Id of the <see cref="MuscleGroup" /> to update.</param>
/// <param name="Name">The new name of the <see cref="MuscleGroup" />.</param>
/// <param name="Concurrency">The concurrency token of the <see cref="MuscleGroup" />.</param>
public record UpdateMuscleGroupRequest(int Id, string Name, string? Concurrency);
