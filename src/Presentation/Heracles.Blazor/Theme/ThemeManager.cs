using Microsoft.JSInterop;

namespace Heracles.Blazor.Theme;

/// <summary>
///     Service responsible for managing theme modes (Light, Dark, System)
///     and persisting the user's preference using a custom JS module.
/// </summary>
public class ThemeManager(IJSRuntime jsRuntime) : IThemeManager, IAsyncDisposable
{
	/// <summary>
	///     The key used to store the theme mode in browser localStorage.
	/// </summary>
	private const string ThemeKey = "HeraclesThemeMode";

	/// <summary>
	///     Lazily loads the JavaScript module that provides:
	///     - getSystemPreference()
	///     - setItem()
	///     - getItem()
	///     - removeItem()
	///     This avoids loading the JS file until it is actually needed.
	/// </summary>
	private readonly Lazy<Task<IJSObjectReference>> _moduleTask =
		new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
			"import", "/_content/Heracles.Blazor/js/theme.js").AsTask());

	/// <summary>
	///     The current theme mode. Defaults to System.
	/// </summary>
	public ThemeMode CurrentThemeMode { get; private set; } = ThemeMode.System;

	/// <summary>
	///     Event triggered whenever the theme mode changes.
	/// </summary>
	public Action? OnChange { get; set; }

	/// <summary>
	///     Indicates whether the theme service has completed initialization.
	/// </summary>
	public bool IsInitialized { get; set; }

	/// <summary>
	///     Indicates whether the active theme is dark mode.
	/// </summary>
	public bool IsDarkMode { get; set; }


	/// <summary>
	///     Initializes the theme by loading the saved preference
	///     or falling back to the system preference.
	/// </summary>
	public async Task InitializeThemeMode()
	{
		CurrentThemeMode = await GetMode();
		IsDarkMode = CurrentThemeMode == ThemeMode.Dark;
		IsInitialized = true;
		NotifyStateChanged();
	}

	/// <summary>
	///     Cycles through Light → Dark → System → Light.
	/// </summary>
	public async Task DarkModeToggle()
	{
		UpdateThemeMode();
		await ApplyThemeMode();
		NotifyStateChanged();
	}


	/// <summary>
	///     Disposes the JS module when the service is disposed.
	/// </summary>
	public async ValueTask DisposeAsync()
	{
		if (_moduleTask.IsValueCreated)
		{
			var module = await _moduleTask.Value;
			await module.DisposeAsync();
		}
	}

	/// <summary>
	///     Moves to the next theme mode in the sequence.
	/// </summary>
	private void UpdateThemeMode()
	{
		CurrentThemeMode = CurrentThemeMode switch
		{
			ThemeMode.Light => ThemeMode.Dark,
			ThemeMode.Dark => ThemeMode.System,
			ThemeMode.System => ThemeMode.Light,
			_ => ThemeMode.System
		};
	}

	/// <summary>
	///     Applies the selected theme mode and persists it.
	/// </summary>
	private async Task ApplyThemeMode()
	{
		if (CurrentThemeMode == ThemeMode.System)
		{
			var systemPreference = await GetSystemPreference();
			IsDarkMode = systemPreference == ThemeMode.Dark;
		}
		else
		{
			IsDarkMode = CurrentThemeMode == ThemeMode.Dark;
		}

		await SetMode(CurrentThemeMode);
	}

	/// <summary>
	///     Saves the theme mode to localStorage using the JS module.
	/// </summary>
	private async Task SetMode(ThemeMode mode)
	{
		var module = await _moduleTask.Value;

		var value = mode switch
		{
			ThemeMode.Light => "light",
			ThemeMode.Dark => "dark",
			ThemeMode.System => "system",
			_ => "system"
		};

		// Save preference
		await module.InvokeVoidAsync("setItem", ThemeKey, value);
	}

	/// <summary>
	///     Retrieves the saved theme mode from localStorage.
	///     If none is saved, falls back to the system preference.
	/// </summary>
	private async Task<ThemeMode> GetMode()
	{
		var module = await _moduleTask.Value;

		var mode = await module.InvokeAsync<string>("getItem", ThemeKey);

		if (string.IsNullOrWhiteSpace(mode))
		{
			return await GetSystemPreference();
		}

		return mode switch
		{
			"light" => ThemeMode.Light,
			"dark" => ThemeMode.Dark,
			"system" => await GetSystemPreference(),
			_ => ThemeMode.System
		};
	}

	/// <summary>
	///     Uses the JS module to detect whether the OS is currently in dark mode.
	/// </summary>
	private async Task<ThemeMode> GetSystemPreference()
	{
		var module = await _moduleTask.Value;
		var isDark = await module.InvokeAsync<bool>("getSystemPreference");
		return isDark ? ThemeMode.Dark : ThemeMode.Light;
	}

	/// <summary>
	///     Notifies subscribers that the theme state has changed.
	/// </summary>
	private void NotifyStateChanged()
	{
		OnChange?.Invoke();
	}
}
