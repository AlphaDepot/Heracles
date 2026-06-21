namespace Heracles.Blazor.Utilities;

public class AppRoutes
{
	public const string Home = "/";

	public static class Errors
	{
		public const string NotFound = $"{Home}not-found";
		public const string Forbidden = $"{Home}forbidden";
		public const string Unauthorized = $"{Home}unauthorized";
	}

	public static class Users
	{
		private const string UsersRoot = $"{Home}users";
		public const string Profile = $"{UsersRoot}/profile";
	}

	public static class Settings
	{
		private const string SettingsRoot = $"{Home}settings";
	}
}
