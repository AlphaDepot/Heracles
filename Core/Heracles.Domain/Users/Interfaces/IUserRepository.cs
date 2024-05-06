using Heracles.Domain.Abstractions.Interfaces;
using Heracles.Domain.Users.Models;

namespace Heracles.Domain.Users.Interfaces;

public interface IUserRepository  : IGenericRepository<User>
{
    
    Task<User?> GetUserByUserIdAsync(string userId); 
    
    // Check if user id exists
    Task<bool> UserIdExistsAsync(string userId);
    
    // Check if user id exists with the given entity id
    Task<bool> UserIdWithIdExistsAsync(string userId, int id);
    
    Task<bool> UserIsAdmin(string userId);
    
    Task<int> DeleteUserAsync(int id, string userId);
    
}