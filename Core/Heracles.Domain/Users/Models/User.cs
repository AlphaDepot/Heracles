using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Entities;

namespace Heracles.Domain.Users.Models;

public class User : BaseEntity
{
    public  string UserId { get; set; } = null!;
    public  string Name { get; set; } = null!;
    public  string Email { get; set; } = null!;
    public DateTime? LastLogin { get; set; } = DateTime.UtcNow;
    public List<string>? Roles { get; set; }


    public User() { }
    
    public User(string userId, string name, string email)
    {
        UserId = userId;
        Name = name;
        Email = email;
    }



    public static Dictionary<string, Expression<Func<User, object>>> GetSortExpression()
    {
        return new Dictionary<string, Expression<Func<User, object>>>
        {
            { "id", e => e.Id },
            { "created", e => e.CreatedAt },
            { "updated", e => e.UpdatedAt },
            { "userId", e => e.UserId },
            { "name", e => e.Name },
            { "email", e => e.Email },
            { "lastLogin", e => e.LastLogin },
            { "roles", e => e.Roles },
        };
    }
    
    
    public static Expression<Func<User, bool>> GetFilterExpression(string? searchTerm, string userId,
        bool isAdmin = false)
    {
        // Initialize filter as null
        Expression<Func<User, bool>>? filter = null;

        // if admin and search term is not empty
        if (isAdmin && !string.IsNullOrEmpty(searchTerm))
        {
            filter = e => e.Id.ToString().Contains(searchTerm)
                          || e.CreatedAt.ToString().Contains(searchTerm)
                          || e.UpdatedAt.ToString().Contains(searchTerm);
        }

        // if admin and search term is empty
        if (isAdmin && string.IsNullOrEmpty(searchTerm))
        {
            filter = null;
        }

        // if not admin and search term is not empty
        if (!isAdmin && !string.IsNullOrEmpty(searchTerm))
        {
            filter = e => e.UserId == userId
                          && (e.Id.ToString().Contains(searchTerm)
                              || e.CreatedAt.ToString().Contains(searchTerm)
                              || e.UpdatedAt.ToString().Contains(searchTerm));
        }

        // if not admin and search term is empty
        if (!isAdmin && string.IsNullOrEmpty(searchTerm))
        {
            filter = e => e.UserId == userId;
        }

        return filter;
    }
    
}