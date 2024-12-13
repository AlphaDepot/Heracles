namespace Application.Common.Interfaces;

/// <summary>
///     Interface for entities that have a user id
/// </summary>
public interface IUserEntity
{
	/// <summary>
	///     User ID obtained from the identity service, specifically Azure Entra ID in this application.
	/// </summary>
	string UserId { get; }
}
