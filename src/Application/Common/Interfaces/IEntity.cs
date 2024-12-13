namespace Application.Common.Interfaces;

/// <summary>
///     Base Entity Interface for all entities
/// </summary>
public interface IEntity
{
	/// <summary>
	///     Unique identifier for the entity
	///     And is the primary key for the entity in the database.
	/// </summary>
	int Id { get; set; }

	/// <summary>
	///     Date and time the entity was created
	///     This are automatically set to the current date and time when the entity is created.
	/// </summary>
	DateTime CreatedAt { get; set; }

	/// <summary>
	///     Date and time the entity was last updated
	///     This is automatically set to the current date and time when the entity is updated.
	/// </summary>
	DateTime UpdatedAt { get; set; }

	/// <summary>
	///     Concurrency token to ensure data has not been updated during interim operations.
	///     This is a string with a length of 36 characters and is generated
	///     using the Guid.NewGuid().ToString() method during the creation and update of the entity.
	/// </summary>
	string? Concurrency { get; set; }
}
