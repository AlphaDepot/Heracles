using Application.Features.Users.Commands;

namespace Application.Features.Users;

/// <summary>
///     <see cref="User" /> Extensions
/// </summary>
public static class UserExtensions
{
	/// <summary>
	///     Map User create request to a <see cref="User" /> entity
	/// </summary>
	/// <param name="request"><see cref="CreateUserRequest" /> request</param>
	/// <returns><see cref="User" /> entity</returns>
	public static User MapCreateRequestToDbEntity(this CreateUserRequest request)
	{
		return new User
		{
			UserId = request.UserId,
			Email = request.Email,
			IsAdmin = request.IsAdmin
		};
	}
}
