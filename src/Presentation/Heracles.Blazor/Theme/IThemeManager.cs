namespace Heracles.Blazor.Theme;

public interface IThemeManager : IAsyncDisposable
{
	ThemeMode CurrentThemeMode { get; }
	Action? OnChange { get; set; }
	bool IsDarkMode { get; set; }
	bool IsInitialized { get; }
	Task InitializeThemeMode();
	Task DarkModeToggle();
}
