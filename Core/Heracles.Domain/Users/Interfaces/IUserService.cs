using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.Users.Models;

namespace Heracles.Domain.Users.Interfaces;

public interface IUserService
{
    Task<ServiceResponse<QueryResponseDto<User>>> GetAsync(QueryRequestDto query);
    Task<ServiceResponse<User>> GetUserByUserIdAsync(string userId);
    Task<ServiceResponse<int>> CreateUserAsync(User newUser);
    Task<ServiceResponse<bool>> UpdateUserAsync(User updatedUser);
    Task<ServiceResponse<bool>> DeleteUserAsync(string userId);
    
    Task<bool> IsUserAuthorized(string userId, string currentUserId);
    Task<bool> DoesUserExist(string userId);
   
}