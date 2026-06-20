using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.MuscleFunctions;

/// <summary>
///     Represents the groupRequest to create a new <see cref="MuscleFunction" />.
/// </summary>
/// <param name="Name">The name of the <see cref="MuscleFunction" />.</param>
public record CreateMuscleFunctionRequest(string Name);
