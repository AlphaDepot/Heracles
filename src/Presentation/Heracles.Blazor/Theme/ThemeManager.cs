using Microsoft.JSInterop;

namespace Heracles.Blazor.Theme;

/// <summary>
/// Provides dark/light/system theme management for the application.
///
/// This service:
/// - Loads the saved theme mode from localStorage
/// - Applies the theme via a JavaScript module
/// - Detects system preference when in System mode
/// - Persists theme mode changes
/// - Notifies subscribers when the theme changes
///
/// Only dark/light/system modes are supported — no color palettes or radius.
///
/// <para>
/// This class implements <see cref="IThemeManager"/> to provide the public
/// theme management API, and <see cref="IAsyncDisposable"/> to ensure the
/// underlying JavaScript module is properly disposed.
/// </para>
/// </summary>
public class ThemeManager(IJSRuntime js) : IThemeManager, IAsyncDisposable
{
    /// <summary>
    /// Path to the JavaScript module that implements theme logic.
    /// </summary>
    private const string ModulePath = "/_content/Heracles.Blazor/js/theme.js";

    /// <summary>
    /// Lazily loads the JS module on first use.
    /// </summary>
    private readonly Lazy<Task<IJSObjectReference>> _module =
        new(() => js.InvokeAsync<IJSObjectReference>("import", ModulePath).AsTask());

    /// <inheritdoc />
    public ThemeMode CurrentThemeMode { get; private set; } = ThemeMode.System;

    /// <inheritdoc />
    public bool IsDarkMode { get; private set; }

    /// <inheritdoc />
    public bool IsInitialized { get; private set; }

    /// <inheritdoc />
    public event Action? OnChange;

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        var module = await _module.Value;

        // Load saved theme mode from JS/localStorage
        var saved = await module.InvokeAsync<ThemeState?>("loadTheme");

        // Convert string → enum
        CurrentThemeMode = saved is not null
	        ? ParseMode(saved.Mode)
	        : ThemeMode.System;

        // Apply theme immediately
        await ApplyThemeModeAsync();

        IsInitialized = true;
        OnChange?.Invoke();
    }

    /// <inheritdoc />
    public async Task ToggleAsync()
    {
        CurrentThemeMode = CurrentThemeMode switch
        {
            ThemeMode.Light => ThemeMode.Dark,
            ThemeMode.Dark => ThemeMode.System,
            _ => ThemeMode.Light
        };

        await ApplyThemeModeAsync();
        await SaveAsync();
        OnChange?.Invoke();
    }

    /// <inheritdoc />
    public async Task SetModeAsync(ThemeMode mode)
    {
        CurrentThemeMode = mode;

        await ApplyThemeModeAsync();
        await SaveAsync();
        OnChange?.Invoke();
    }

    /// <summary>
    /// Applies the current theme mode by invoking JS:
    /// - "light"  → force light mode
    /// - "dark"   → force dark mode
    /// - "system" → detect OS preference
    /// </summary>
    private async Task ApplyThemeModeAsync()
    {
        var module = await _module.Value;

        // Determine active dark/light state
        if (CurrentThemeMode == ThemeMode.System)
        {
            IsDarkMode = await module.InvokeAsync<bool>("getSystemPreference");
        }
        else
        {
            IsDarkMode = CurrentThemeMode == ThemeMode.Dark;
        }

        // JS expects a lowercase string ("light", "dark", "system")
        var jsMode = CurrentThemeMode.ToString().ToLowerInvariant();

        await module.InvokeVoidAsync("applyTheme", jsMode);
    }

    /// <summary>
    /// Persists the current theme mode to localStorage via JS.
    /// </summary>
    private async Task SaveAsync()
    {
        var module = await _module.Value;

        var jsMode = CurrentThemeMode.ToString().ToLowerInvariant();

        await module.InvokeVoidAsync("saveTheme", jsMode);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module.IsValueCreated)
        {
            var module = await _module.Value;
            await module.DisposeAsync();
        }
    }

    /// <summary>
    /// Converts a JS string ("light", "dark", "system") into a ThemeMode enum.
    /// Uses a switch expression for clarity and safety.
    /// </summary>
    private static ThemeMode ParseMode(string? mode) =>
	    mode?.ToLowerInvariant() switch
	    {
		    "light"  => ThemeMode.Light,
		    "dark"   => ThemeMode.Dark,
		    _        => ThemeMode.System
	    };

    /// <summary>
    /// Internal DTO used ONLY for JS interop.
    /// JS returns: { "mode": "light" | "dark" | "system" }
    /// </summary>
    private sealed class ThemeState
    {
	    public string Mode { get; set; } = "system";
    }
}
