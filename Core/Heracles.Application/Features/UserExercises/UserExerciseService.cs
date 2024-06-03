using System.Linq.Expressions;
using Heracles.Application.Features.UserExercises.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.UserExercises.DTOs;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.UserExercises.Models;
using Heracles.Domain.Users.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Heracles.Application.Features.UserExercises;

public class UserExerciseService : IUserExerciseService
{
    private readonly IAppLogger<UserExerciseService> _logger;
    private readonly IUserExerciseRepository _repository;
    private readonly IExerciseTypeRepository _exerciseRepository;
    private readonly IEquipmentGroupRepository _equipmentGroupRepository;
    private readonly IUserService _userService;

    private readonly string? _userId;
    private readonly bool _isAdmin;

    public UserExerciseService(
        IAppLogger<UserExerciseService> logger, 
        IHttpContextAccessor httpContextAccessor,
        IUserExerciseRepository repository,
        IExerciseTypeRepository exerciseRepository, 
        IEquipmentGroupRepository equipmentGroupRepository,
        IUserService userService)
    {
        _logger = logger;
        
        _repository = repository;
        _exerciseRepository = exerciseRepository;
        _equipmentGroupRepository = equipmentGroupRepository;
        _userService = userService;


        // Get user id and check if user is admin from http context
        _userId = httpContextAccessor.HttpContext?.Items["UserId"]?.ToString();
        if (httpContextAccessor.HttpContext != null) _isAdmin = httpContextAccessor.HttpContext.User.IsInRole("Admin");
    }

    /// <summary>
    /// Retrieves user exercises based on the specified query.
    /// </summary>
    /// <param name="query">The query request.</param>
    /// <returns>A task representing the asynchronous operation that returns a domain response containing the query response with the filtered and sorted user exercises.</returns>
    public async Task<DomainResponse<QueryResponseDto<UserExercise>>> GetAsync(QueryRequestDto query)
    {
        var filter = UserExercise.GetFilterExpression(query.SearchTerm, _userId!, _isAdmin);
        var sortExpression = UserExercise.GetSortExpression();
        
        var queryHelper = new QueryHelper().CreateQueriable(query, sortExpression, filter);
        var result = await _repository.GetAsync(queryHelper);
        
       
        return DomainResponse.Success(result);

    }

    /// <summary>
    /// Retrieves a user exercise by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the user exercise to retrieve.</param>
    /// <returns>A task representing the asynchronous operation that returns a domain response containing the retrieved user exercise.</returns>
    public async Task<DomainResponse<UserExercise>> GetByIdAsync(int id)
    {
        var validator = new GetUserExerciseByIdValidator(_repository, _userService, _userId);
        var validationResult = await validator.ValidateAsync(id);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<UserExercise>(validationResult.ToDictionary()));
            return DomainResponse.Failure<UserExercise>(EntityErrorMessage<UserExercise>.BadRequest(validationResult.ToDictionary()));
        }
        
        var userExercise = await _repository.GetEntityByIdAsync(id);
        // return the user exercise
        _logger.LogInformation(ServiceMessages.EntityRetrieved<UserExercise>(userExercise.Id));
        return DomainResponse.Success(userExercise);
        
    }

    /// <summary>
    /// Creates a new user exercise based on the specified data transfer object.
    /// </summary>
    /// <param name="dto">The data transfer object containing the user exercise details.</param>
    /// <returns>A task representing the asynchronous operation that returns a domain response containing the identifier of the created user exercise.</returns>
    public async Task<DomainResponse<int>> CreateAsync(CreateUserExerciseDto dto)
    {
        var validator = new CreateUserExerciseValidator(_exerciseRepository, _userService, _userId);
        var validationResult = await validator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<UserExercise>(validationResult.ToDictionary()));
            return DomainResponse.Failure<int>(EntityErrorMessage<UserExercise>.BadRequest(validationResult.ToDictionary()));
        }
        
        int version = 1;
        
        var userExerciseExist = await _repository.GetUserExerciseByExerciseTypeIdAsync(dto.ExerciseId);
        if (userExerciseExist != null)
        {
            version = userExerciseExist.Version + 1;
        }
        
        var exercise = await _exerciseRepository.GetEntityByIdAsync(dto.ExerciseId);
        
        var userExercise = new UserExercise
        {
            UserId = dto.UserId,
            ExerciseTypeId = dto.ExerciseId,
            ExerciseType = exercise!,
            Version = version,
        };
        
        var userExerciseId = await _repository.CreateEntityAsync(userExercise);
        
        _logger.LogInformation(ServiceMessages.EntityCreated<UserExercise>(userExercise.Id));
        return DomainResponse.Success(userExerciseId);
        
    }

    /// <summary>
    /// Updates a user exercise based on the provided DTO.
    /// </summary>
    /// <param name="dto">The DTO containing the updated user exercise data.</param>
    /// <returns>A task representing the asynchronous operation that returns a domain response indicating the success or failure of the update.</returns>
    public async Task<DomainResponse<bool>> UpdateAsync(UpdateUserExerciseDto dto)
    {
        var validator = new UpdateUserExerciseValidator(_repository, _equipmentGroupRepository, _userService, _userId);
        var validationResult = await validator.ValidateAsync(dto);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<UserExercise>(validationResult.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<UserExercise>.BadRequest(validationResult.ToDictionary()));
        }
        
        var userExercise = await _repository.GetEntityByIdAsync(dto.Id);
        
        // check if the equipment group is valid and exists in the database then update the user exercise
        var equipmentGroup = dto.EquipmentGroupId != 0
            ? await _equipmentGroupRepository.GetEntityByIdAsync(dto.EquipmentGroupId)
            : userExercise!.EquipmentGroup;
        
        // Update the user exercise
        userExercise!.CurrentWeight =  dto.CurrentWeight ?? userExercise.CurrentWeight;
        userExercise.PersonalRecord = dto.PersonalRecord ?? userExercise.PersonalRecord;
        userExercise.DurationInSeconds = dto.DurationInSeconds ?? userExercise.DurationInSeconds;
        userExercise.SortOrder = dto.SortOrder ?? userExercise.SortOrder;
        userExercise.Repetitions = dto.Repetitions ?? userExercise.Repetitions;
        userExercise.Sets = dto.Sets ?? userExercise.Sets;
        userExercise.Timed = dto.Timed ?? userExercise.Timed;
        userExercise.BodyWeight = dto.BodyWeight ?? userExercise.BodyWeight;
        userExercise.EquipmentGroup = equipmentGroup;
        
        await _repository.UpdateEntityAsync(userExercise);
        
        _logger.LogInformation(ServiceMessages.EntityUpdated<UserExercise>(userExercise.Id));
        
        return DomainResponse.Success(true);
    }

    /// <summary>
    /// Deletes a user exercise based on the specified ID.
    /// </summary>
    /// <param name="id">The ID of the user exercise to delete.</param>
    /// <returns>A task representing the asynchronous operation that returns a domain response indicating the success or failure of the delete operation.</returns>
    public async Task<DomainResponse<bool>> DeleteAsync(int id)
    {
        var validator = new DeleteUserExerciseValidator(_repository, _userService, _userId);
        var validationResult = await validator.ValidateAsync(id);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<UserExercise>(validationResult.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<UserExercise>.BadRequest(validationResult.ToDictionary()));
        }
        
        await _repository.DeleteEntityAsync(id);
        
        _logger.LogInformation(ServiceMessages.EntityDeleted<UserExercise>(id));
        return DomainResponse.Success(true);
        
    }
}