using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.WorkoutSessions;

/// <summary>
///     Request to attach a <see cref="UserExercise" /> to a <see cref="WorkoutSession" />.
/// </summary>
/// <param name="UserExerciseId"> The id of the <see cref="UserExercise" /> to attach to the <see cref="WorkoutSession" />.</param>
/// <param name="WorkoutSessionId">
///     The id of the <see cref="WorkoutSession" /> to attach the <see cref="UserExercise" />
///     to.
/// </param>
public record AttachUserExerciseToWorkoutSessionRequest(int UserExerciseId, int WorkoutSessionId);
