using System.ComponentModel.DataAnnotations;
using Application.Common.Interfaces;

namespace Application.Features.Users;

/// <summary>
///     Model for User entity
/// </summary>
public class User : IUserEntity
{
	public int Id { get; set; }
	[MaxLength(255)] public required string Email { get; set; }
	public bool IsAdmin { get; set; }
	[MaxLength(36)] public required string UserId { get; set; }
}
