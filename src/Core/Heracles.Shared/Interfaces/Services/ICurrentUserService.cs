namespace Heracles.Shared.Interfaces.Services;

public interface ICurrentUserService
{
	string? UserId { get; }
	bool IsAuthenticated { get; }
}
