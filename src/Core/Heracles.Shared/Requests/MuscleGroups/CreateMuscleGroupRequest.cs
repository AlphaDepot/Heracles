using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.MuscleGroups;

/// <summary>
///     Represents the groupRequest to create a new <see cref="MuscleGroup" />.
/// </summary>
/// <param name="Name">The name of the <see cref="MuscleGroup" />.</param>
public record CreateMuscleGroupRequest(string Name);
