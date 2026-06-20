using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.WorkoutSessions;

/// <summary>
///     Represents the groupRequest to create a new <see cref="WorkoutSession" />.
/// </summary>
/// <param name="Name">The <see cref="CreateWorkoutSessionRequest.Name" /> to create.</param>
/// <param name="DayOfWeek">The <see cref="CreateWorkoutSessionRequest.DayOfWeek" /> to create.</param>
/// <param name="SortOrder">The <see cref="CreateWorkoutSessionRequest.SortOrder" /> to create.</param>
/// <param name="UserId">The <see cref="CreateWorkoutSessionRequest.UserId" /> to create.</param>
public record CreateWorkoutSessionRequest(string Name, string DayOfWeek, int SortOrder, string UserId);
