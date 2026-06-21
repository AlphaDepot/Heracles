using System.ComponentModel;

namespace Heracles.Blazor.Components.Enums;

public enum Color
{
	/// <summary>
	///     Neutral default color.
	/// </summary>
	[Description("default")] Default,

	/// <summary>
	///     Primary brand color.
	/// </summary>
	[Description("primary")] Primary,

	/// <summary>
	///     Secondary accent color.
	/// </summary>
	[Description("secondary")] Secondary,

	/// <summary>
	///     Destructive or dangerous actions.
	/// </summary>
	[Description("destructive")] Destructive,

	/// <summary>
	///     Muted surfaces and subtle backgrounds.
	/// </summary>
	[Description("muted")] Muted,

	/// <summary>
	///     Accent color for highlighting UI elements.
	/// </summary>
	[Description("accent")] Accent,

	/// <summary>
	///     Outlined/border color.
	/// </summary>
	[Description("outline")] Outline,

	/// <summary>
	///     Background color.
	/// </summary>
	[Description("background")] Background,

	/// <summary>
	///     Foreground color (text/icons).
	/// </summary>
	[Description("foreground")] Foreground,

	/// <summary>
	///     Focus ring color.
	/// </summary>
	[Description("ring")] Ring,

	/// <summary>
	///     Informational messages (blue semantic).
	/// </summary>
	[Description("info")] Info,

	/// <summary>
	///     Success messages (green semantic).
	/// </summary>
	[Description("success")] Success,

	/// <summary>
	///     Warning messages (amber/yellow semantic).
	/// </summary>
	[Description("warning")] Warning
}
