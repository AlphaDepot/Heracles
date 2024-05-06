using Heracles.Application.Features.UserExercisesHistories.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.UserExerciseHistories.DTOs;
using Heracles.Domain.UserExerciseHistories.Interfaces;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.Users.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Heracles.Application.Features.UserExercisesHistories;

public class UserExerciseHistoryService : IUserExerciseHistoryService
{
    private readonly IAppLogger<UserExerciseHistoryService> _logger;
    private readonly IUserExerciseHistoryRepository _repository;
    private readonly IUserExerciseRepository _userExerciseRepository;
    private readonly IUserService _userService;


    private readonly string? _userId;
    private readonly bool _isAdmin;
    
    public UserExerciseHistoryService(
        IAppLogger<UserExerciseHistoryService> logger, 
        IHttpContextAccessor httpContextAccessor,
        IUserExerciseHistoryRepository repository,
        IUserExerciseRepository userExerciseRepository,
       IUserService userService)
    {
        _logger = logger;
        _repository = repository;
        _userExerciseRepository = userExerciseRepository;
        _userService = userService;


        // Get user id and check if user is admin from http context
        _userId = httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
        if (httpContextAccessor.HttpContext != null) _isAdmin = httpContextAccessor.HttpContext.User.IsInRole("Admin");
    }

    /// <summary>
    /// Retrieves user exercise history based on the provided query.
    /// </summary>
    /// <param name="query">The query request object.</param>
    /// <returns>DomainResponse containing the query response object.</returns>
    public async Task<DomainResponse<QueryResponse<UserExerciseHistory>>> GetAsync(QueryRequest query)
    {
        
        var filter = UserExerciseHistory.GetFilterExpression(query.SearchTerm, _userId!, _isAdmin);
        var sortExpression = UserExerciseHistory.GetSortExpression();
        
        var queryHelper = new QueryHelper().CreateQueriable(query, sortExpression, filter);
        var result = await _repository.GetAsync(queryHelper);
        
        
        return DomainResponse.Success(
            new QueryResponse<UserExerciseHistory>
            {
                Data = result,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
            }
        );
    }

    /// <summary>
    /// Retrieves a user exercise history entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the user exercise history.</param>
    /// <returns>A task representing the asynchronous operation. The result is a DomainResponse containing the user exercise history entity.</returns>
    public async Task<DomainResponse<UserExerciseHistory>> GetByIdAsync(int id)
    {
        var validator = new GetUserExerciseHistoryByIdValidator(_repository, _userService, _userId!);
        var validationResult = await validator.ValidateAsync(id);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<UserExerciseHistory>(validationResult.ToDictionary()));
            return DomainResponse.Failure<UserExerciseHistory>(EntityErrorMessage<UserExerciseHistory>.BadRequest(validationResult.ToDictionary()));
        }
        
        var result = await _repository.GetByIdAsync(id);
        
        _logger.LogInformation(ServiceMessages.EntityRetrieved<UserExerciseHistory>(id));
        return DomainResponse.Success(result);
        
    }

    /// <summary>
    /// Creates a new user exercise history record asynchronously.
    /// </summary>
    /// <param name="entity">The user exercise history entity to create.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a DomainResponse object indicating the result of the operation.</returns>
    public async Task<DomainResponse<int>> CreateAsync(UserExerciseHistory entity)
    {
        var validator = new CreateUserExerciseHistoryValidator(_userExerciseRepository, _userService, _userId!);
        var validationResult = await validator.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<UserExerciseHistory>(validationResult.ToDictionary()));
            return DomainResponse.Failure<int>(EntityErrorMessage<UserExerciseHistory>.BadRequest(validationResult.ToDictionary()));
        }
        
        var id = await _repository.CreateAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityCreated<UserExerciseHistory>(id));
        
        return DomainResponse.Success(id);
        
    }

    /// <summary>
    /// Updates a user's exercise history based on the provided information.
    /// </summary>
    /// <param name="dto">The update user exercise history DTO containing the information to update.</param>
    /// <returns>A <see cref="DomainResponse{TValue}"/> indicating the result of the update operation.</returns>
    public async Task<DomainResponse<bool>> UpdateAsync(UpdateUserExerciseHistoryDto dto)
    {
        var validator = new UpdateUserExerciseHistoryValidator(_repository, _userService, _userId!);
        var validationResult = await validator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<UserExerciseHistory>(validationResult.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<UserExerciseHistory>.BadRequest(validationResult.ToDictionary()));
        }
        
        var entity = await _repository.GetByIdAsync(dto.Id);

        entity.Repetition = dto.Repetition;
        entity.Weight = dto.Weight;
        
        await _repository.UpdateAsync(entity);
        
        _logger.LogInformation(ServiceMessages.EntityUpdated<UserExerciseHistory>(entity.Id));
        
        return DomainResponse.Success(true);
        
    }

    /// <summary>
    /// Deletes a user exercise history entry based on the provided ID.
    /// </summary>
    /// <param name="id">The ID of the user exercise history entry to delete.</param>
    /// <returns>A task representing the asynchronous operation. The task result is a DomainResponse&lt;bool&gt; indicating whether the deletion was successful.</returns>
    public async Task<DomainResponse<bool>> DeleteAsync(int id)
    {
        var validator = new DeleteUserExerciseHistoryValidator(_repository, _userService, _userId!);
        var validationResult = await validator.ValidateAsync(id);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<UserExerciseHistory>(validationResult.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<UserExerciseHistory>.BadRequest(validationResult.ToDictionary()));
        }
        
        await _repository.DeleteAsync(id);
        _logger.LogInformation(ServiceMessages.EntityDeleted<UserExerciseHistory>(id));
        return DomainResponse.Success(true); 
        
    }
}