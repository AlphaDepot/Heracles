using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.MuscleFunctions;

/// <summary>
///     Updates a <see cref="MuscleFunction" />.
/// </summary>
/// <param name="Id"> The Id of the <see cref="MuscleFunction" /> to update.</param>
/// <param name="Name">The new name of the <see cref="MuscleFunction" />.</param>
/// <param name="Concurrency">The concurrency token of the <see cref="MuscleFunction" />.</param>
public record UpdateMuscleFunctionRequest(int Id, string Name, string? Concurrency);
