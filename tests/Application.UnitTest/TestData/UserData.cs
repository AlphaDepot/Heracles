using System.Security.Claims;
using Application.Features.Users;

namespace Application.UnitTest.TestData;

public static class UserData
{
	/// <summary>
	///     Fake data for User
	/// </summary>
	/// <returns></returns>
	public static List<User> Users()
	{
		return
		[
			// admin user
			new User
			{
				UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f22",
				Email = "admin.test.user@test.com",
				IsAdmin = true
			},

			// non admin user
			new User
			{
				UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f25",
				Email = "test.user@test.com",
				IsAdmin = false
			},
			new User
			{
				UserId = "9c7e2f0a-292a-47d2-b8b7-8af9e2d34f26",
				Email = "test.user@test.com",
				IsAdmin = false
			}
		];
	}

	/// <summary>
	///     Convert User to ClaimsPrincipal
	/// </summary>
	/// <param name="user"> User</param>
	/// <returns> ClaimsPrincipal</returns>
	public static ClaimsPrincipal ToClaimsPrincipal(this User user)
	{
		var claimsIdentity = new ClaimsIdentity(new List<Claim>
		{
			new(ClaimTypes.NameIdentifier, user.UserId)
		});
		return new ClaimsPrincipal(claimsIdentity);
	}
}
