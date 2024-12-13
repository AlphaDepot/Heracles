namespace Application.Common.Interfaces;

/// <summary>
///     Interface for entities that have a type
/// </summary>
public interface IHasType
{
	/// <summary>
	///     Equipment Type (e.g. "Dumbbell", "Barbell", "Kettlebell")
	/// </summary>
	string Type { get; }
}
