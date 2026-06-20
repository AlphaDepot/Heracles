using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.UserExerciseHistories;

/// <summary>
///     Represents the groupRequest to create a new <see cref="UserExerciseHistory" />.
/// </summary>
/// <param name="UserExerciseId"> The <see cref="UserExerciseHistory.UserExerciseId" /> to create.</param>
/// <param name="Repetition">  The <see cref="UserExerciseHistory.Repetition" /> to create.</param>
/// <param name="UserId">   The <see cref="UserExerciseHistory.UserId" /> to create.</param>
public record CreateUserExerciseHistoryRequest(int UserExerciseId, double Weight, int Repetition, string UserId);
