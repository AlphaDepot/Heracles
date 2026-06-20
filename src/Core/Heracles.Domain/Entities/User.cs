using System.ComponentModel.DataAnnotations;
using Heracles.Domain.Interfaces;

namespace Heracles.Domain.Entities;

/// <summary>
///     Model for User entity
/// </summary>
public class User : IEntity, IUserEntity
{
	[MaxLength(255)] public required string Email { get; set; }
	public bool IsAdmin { get; set; }
	public int Id { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
	public string? Concurrency { get; set; }
	[MaxLength(36)] public required string UserId { get; set; }
}
