using Heracles.Domain.Entities;
using Heracles.Shared.Requests.Users;

namespace Heracles.Application.Features.Users;

/// <summary>
///     <see cref="User" /> Extensions
/// </summary>
public static class UserExtensions
{
	/// <summary>
	///     Map User create groupRequest to a <see cref="User" /> entity
	/// </summary>
	/// <param name="request"><see cref="CreateUserRequest" /> groupRequest</param>
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
