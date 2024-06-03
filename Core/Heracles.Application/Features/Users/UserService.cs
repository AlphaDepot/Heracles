using Heracles.Application.Features.Users.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.Users.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

namespace Heracles.Application.Features.Users;

/// <summary>
///  This class contains methods for user related operations.
/// </summary>
public class UserService :IUserService
{
    private readonly IAppLogger<UserService> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IMemoryCache _memoryCache;

    private readonly string? _userId;
    private readonly bool _isAdmin;
    
    public UserService(IAppLogger<UserService> logger, 
        IUserRepository userRepository,
        IMemoryCache memoryCache,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _userRepository = userRepository;
        _memoryCache = memoryCache;

        // Get user id and check if user is admin from http context
        _userId = httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
        if (httpContextAccessor.HttpContext != null) _isAdmin = httpContextAccessor.HttpContext.User.IsInRole("Admin");
    }


    public async Task<DomainResponse<QueryResponseDto<User>>> GetAsync(QueryRequestDto query)
    {
        var filter = User.GetFilterExpression(query.SearchTerm, _userId!, _isAdmin);
        var sortExpression = User.GetSortExpression();
        
        var queryHelper = new QueryHelper().CreateQueriable(query, sortExpression, filter);
        var result = await  _userRepository.GetAsync(queryHelper);
        
        return DomainResponse.Success(result);
    }


    public async Task<DomainResponse<User>> GetUserByUserIdAsync(string userId)
    {
        var userFromCache = GetUserFromCache(userId);
        if (userFromCache != null)
        {
            return DomainResponse.Success(userFromCache);
        }
        
        var user = await _userRepository.GetUserByUserIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<User>(userId));
            return DomainResponse.Failure<User>(EntityErrorMessage<User>.NotFound(userId));
        }
        
        StoreUserInCache(user);
        
        return DomainResponse.Success(user);
       
    }


    public async Task<DomainResponse<int>> CreateUserAsync(User newUser)
    {

        var validator = new CreateUserValidator(_userRepository);
        var validationResult = await validator.ValidateAsync(newUser);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<User>(validationResult.ToDictionary()));
            return DomainResponse.Failure<int>(EntityErrorMessage<User>.BadRequest(validationResult.ToDictionary()));
        }

        var result = await _userRepository.CreateEntityAsync(newUser);
        _logger.LogInformation(ServiceMessages.EntityCreated<User>(result));
        
        return DomainResponse.Success(result);
    }


    public async  Task<DomainResponse<bool>> UpdateUserAsync(User updatedUser)
    {
        var validator = new UpdateUserValidator(_userRepository);
        var validationResult = await validator.ValidateAsync(updatedUser);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<User>(validationResult.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<User>.BadRequest(validationResult.ToDictionary()));
        }
        
        var result = await _userRepository.UpdateEntityAsync(updatedUser);
        _logger.LogInformation(ServiceMessages.EntityUpdated<User>(result));
        
        return DomainResponse.Success(true);
    }

  
    public async Task<DomainResponse<bool>> DeleteUserAsync(string userId)
    {
        var user = await _userRepository.GetUserByUserIdAsync(userId);
        
        if (user == null)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<User>(userId));
            return DomainResponse.Failure<bool>(EntityErrorMessage<User>.NotFound(userId));
        }
        
        var result = await _userRepository.DeleteUserAsync(user.Id, user.UserId);
        _logger.LogInformation(ServiceMessages.EntityDeleted<User>(result));
        
        return DomainResponse.Success(true);
    }


    public async Task<bool> IsUserAuthorized(string userId, string currentUserId)
    {
        var isSameUser = userId == currentUserId;
        var isUserAdmin = await _userRepository.IsAdminUser(currentUserId);
        return isSameUser || isUserAdmin;
    }


    public Task<bool> DoesUserExist(string userId)
    {
        return _userRepository.UserIdExistsAsync(userId);
    }
    
    private void StoreUserInCache(User user)
    {
        _memoryCache.Set(user.UserId, user, TimeSpan.FromHours(24));
    }
    
    private User? GetUserFromCache(string userId)
    {
        return _memoryCache.Get<User>(userId);
    }
}