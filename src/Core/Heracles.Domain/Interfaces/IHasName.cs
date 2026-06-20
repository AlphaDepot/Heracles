namespace Heracles.Domain.Interfaces;

/// <summary>
///     Interface for entities that have a name
/// </summary>
public interface IHasName
{
	/// <summary>
	///     Name of the entity
	/// </summary>
	string Name { get; }
}
