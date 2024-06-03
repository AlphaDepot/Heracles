using Heracles.Application.Features.Users.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.Users.Models;
using Microsoft.AspNetCore.Http;

namespace Heracles.Application.Features.Users;

/// <summary>
///  This class contains methods for user related operations.
/// </summary>
public class UserService :IUserService
{
    private readonly IAppLogger<UserService> _logger;
    private readonly IUserRepository _userRepository;

    private readonly string? _userId;
    private readonly bool _isAdmin;
    
    public UserService(IAppLogger<UserService> logger, 
        IUserRepository userRepository,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _userRepository = userRepository;
        
        // Get user id and check if user is admin from http context
        _userId = httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
        if (httpContextAccessor.HttpContext != null) _isAdmin = httpContextAccessor.HttpContext.User.IsInRole("Admin");
    }

    /// <summary>
    /// Retrieves data from the repository based on the given query.
    /// </summary>
    /// <param name="query">The query request object containing search and pagination parameters.</param>
    /// <returns>A domain response containing the query response with the data, page number, and page size.</returns>
    public async Task<DomainResponse<QueryResponseDto<User>>> GetAsync(QueryRequestDto query)
    {
        var filter = User.GetFilterExpression(query.SearchTerm, _userId!, _isAdmin);
        var sortExpression = User.GetSortExpression();
        
        var queryHelper = new QueryHelper().CreateQueriable(query, sortExpression, filter);
        var result = await  _userRepository.GetAsync(queryHelper);
        
        return DomainResponse.Success(result);
    }

    /// <summary>
    ///  Gets a user by their user id.
    /// </summary>
    /// <param name="userId"> The user id of the user.</param>
    /// <returns> A domain response with the user object.</returns>
    public async Task<DomainResponse<User>> GetUserByUserIdAsync(string userId)
    {
        var user = await _userRepository.GetUserByUserIdAsync(userId);
        
        if (user == null)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<User>(userId));
            return DomainResponse.Failure<User>(EntityErrorMessage<User>.NotFound(userId));
        }
        
        return DomainResponse.Success(user);
       
    }

    /// <summary>
    ///  Creates a new user in the database.
    /// </summary>
    /// <param name="newUser"> The new user object.</param>
    /// <returns> A domain response with the id of the new user.</returns>
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

    /// <summary>
    ///  Updates a user in the database.
    /// </summary>
    /// <param name="updatedUser"> The updated user object.</param>
    /// <returns> A domain response with a boolean value indicating success or failure.</returns>
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

    /// <summary>
    /// Deletes a user by their user id.
    /// </summary>
    /// <param name="userId">The user id of the user.</param>
    /// <returns>A domain response indicating the success of the deletion operation.</returns>
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

    /// <summary>
    /// Determines whether a user is authorized or not.
    /// </summary>
    /// <param name="userId">The user id of the user to check.</param>
    /// <param name="currentUserId">The current user id.</param>
    /// <returns>True if the user is authorized, otherwise false.</returns>
    public async Task<bool> IsUserAuthorized(string userId, string currentUserId)
    {
        var isSameUser = userId == currentUserId;
        var isUserAdmin = await _userRepository.UserIsAdmin(currentUserId);
        return isSameUser || isUserAdmin;
    }

    /// <summary>
    /// Checks if a user with the specified user id exists.
    /// </summary>
    /// <param name="userId">The user id to check.</param>
    /// <returns>A task that represents the asynchronous operation and returns a boolean value indicating whether the user exists or not.</returns>
    public Task<bool> DoesUserExist(string userId)
    {
        return _userRepository.UserIdExistsAsync(userId);
    }
}