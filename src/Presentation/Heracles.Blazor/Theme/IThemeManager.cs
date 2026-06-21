namespace Heracles.Blazor.Theme;

/// <summary>
/// Defines the contract for the application's theme management service.
///
/// This interface supports three theme modes:
/// - <see cref="ThemeMode.Light"/>  → Force light mode
/// - <see cref="ThemeMode.Dark"/>   → Force dark mode
/// - <see cref="ThemeMode.System"/> → Follow the user's OS preference
///
/// Implementations are responsible for:
/// - Loading the saved theme mode from localStorage
/// - Applying the theme via JavaScript interop
/// - Detecting system preference when in System mode
/// - Persisting theme mode changes
/// - Notifying subscribers when the theme changes
/// </summary>
public interface IThemeManager
{
    /// <summary>
    /// Gets the currently selected theme mode.
    /// This value determines how the theme is applied:
    /// Light, Dark, or System (OS preference).
    /// </summary>
    ThemeMode CurrentThemeMode { get; }

    /// <summary>
    /// Gets a value indicating whether dark mode is currently active.
    /// This reflects the *effective* theme, not just the selected mode.
    /// </summary>
    bool IsDarkMode { get; }

    /// <summary>
    /// Gets a value indicating whether the theme manager has completed initialization.
    /// Initialization loads saved preferences and applies the theme.
    /// </summary>
    bool IsInitialized { get; }

    /// <summary>
    /// Raised whenever the theme mode or active dark/light state changes.
    /// Components should subscribe and call <c>StateHasChanged</c> to update the UI.
    /// </summary>
    event Action? OnChange;

    /// <summary>
    /// Initializes the theme manager by loading the saved theme mode,
    /// applying it, and detecting system preference if needed.
    /// Should be called once after the first render.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Cycles through the available theme modes in order:
    /// Light → Dark → System → Light.
    /// </summary>
    Task ToggleAsync();

    /// <summary>
    /// Sets the theme mode explicitly and applies it immediately.
    /// </summary>
    /// <param name="mode">The theme mode to apply.</param>
    Task SetModeAsync(ThemeMode mode);
}
