using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.Users.Models;

namespace Heracles.Domain.Users.Interfaces;

public interface IUserService
{
    Task<DomainResponse<QueryResponse<User>>> GetAsync(QueryRequest query);
    Task<DomainResponse<User>> GetUserByUserIdAsync(string userId);
    Task<DomainResponse<int>> CreateUserAsync(User newUser);
    Task<DomainResponse<bool>> UpdateUserAsync(User updatedUser);
    Task<DomainResponse<bool>> DeleteUserAsync(string userId);
    
    Task<bool> IsUserAuthorized(string userId, string currentUserId);
    Task<bool> DoesUserExist(string userId);
   
}