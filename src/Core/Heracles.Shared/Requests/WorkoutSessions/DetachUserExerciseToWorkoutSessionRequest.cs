using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.WorkoutSessions;

/// <summary>
///     Request to detach a <see cref="UserExercise" /> from a <see cref="WorkoutSession" />.
/// </summary>
/// <param name="WorkoutSessionId"> The id of the <see cref="WorkoutSession" />.</param>
/// <param name="UserExerciseId"> The id of the <see cref="UserExercise" />.</param>
public record DetachUserExerciseToWorkoutSessionRequest(int WorkoutSessionId, int UserExerciseId);
