using System.Security.Claims;
using Heracles.Shared.Interfaces.Services;

namespace Heracles.Api.Services;

public class CurrentUserService : ICurrentUserService
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public CurrentUserService(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public string? UserId =>
		_httpContextAccessor.HttpContext?
			.User.FindFirst(ClaimTypes.NameIdentifier)?
			.Value;

	public bool IsAuthenticated =>
		_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
