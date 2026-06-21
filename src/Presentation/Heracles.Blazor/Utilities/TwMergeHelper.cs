using TailwindMerge;

namespace Heracles.Blazor.Utilities;

public static class Tw
{
	private static readonly TwMerge Instance = new();

	// Generic params-based surface
	public static string Merge(params string?[] classes)
	{
		// Filter out null or empty values
		var filtered = classes
			.Where(c => !string.IsNullOrWhiteSpace(c))
			.Cast<string>()
			.ToArray();

		return Instance.Merge(filtered);
	}

	// Convenience overload for two parts (optional)
	public static string Merge(string baseClass, string? additional)
	{
		return additional is null
			? baseClass
			: Instance.Merge(baseClass, additional);
	}
}
