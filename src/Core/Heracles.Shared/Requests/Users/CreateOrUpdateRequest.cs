namespace Heracles.Shared.Requests.Users;

/// <summary>
///     Represents the groupRequest to create or update a user.
/// </summary>
/// <param name="UserId"> The user id.</param>
/// <param name="Email"> The email.</param>
/// <param name="IsAdmin"> The admin status.</param>
public record CreateOrUpdateRequest(string UserId, string Email, bool IsAdmin);
