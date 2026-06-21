namespace Heracles.Blazor.Components.UI.Icons;

public readonly struct IconValue
{
	public string Svg { get; }

	public IconValue(string svg)
	{
		Svg = svg;
	}

	public static implicit operator IconValue(string svg)
	{
		return new IconValue(svg);
	}

	public static implicit operator string(IconValue icon)
	{
		return icon.Svg;
	}
}
