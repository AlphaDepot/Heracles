using Heracles.Domain.Entities;

namespace Heracles.Shared.Requests.Users;

/// <summary>
///     Represents the groupRequest to update a <see cref="User" />
/// </summary>
/// <param name="UserId"> The unique identifier of the user.</param>
/// <param name="Email"> The email of the user.</param>
/// <param name="IsAdmin"> If true, the user will be updated as an admin.</param>
public record UpdateUserRequest(string UserId, string Email, bool IsAdmin);
