using System.Linq.Expressions;
using Heracles.Application.Features.WorkoutSessions.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.WorkoutSessions.DTOs;
using Heracles.Domain.WorkoutSessions.Interfaces;
using Heracles.Domain.WorkoutSessions.Models;
using Microsoft.AspNetCore.Http;

namespace Heracles.Application.Features.WorkoutSessions;

public class WorkoutSessionService : IWorkoutSessionService
{
    private readonly IAppLogger<WorkoutSessionService> _logger;
    private readonly IWorkoutSessionRepository _repository;
    private readonly IUserExerciseRepository _userExerciseRepository;
    private readonly IUserService _userService;

    private readonly string? _userId;
    private readonly bool _isAdmin;
    public WorkoutSessionService(
        IAppLogger<WorkoutSessionService> logger, 
         IHttpContextAccessor httpContextAccessor,
        IWorkoutSessionRepository repository, IUserExerciseRepository userExerciseRepository, IUserService userService)
    {
        
        
        _logger = logger;
        _repository = repository;
        _userExerciseRepository = userExerciseRepository;
        _userService = userService;

        // Get user id and check if user is admin from http context
        _userId = httpContextAccessor.HttpContext.Items["UserId"]?.ToString();
        _isAdmin = httpContextAccessor.HttpContext.User.IsInRole("Admin");
    }

    public async Task<ServiceResponse<QueryResponseDto<WorkoutSession>>> GetAsync(QueryRequestDto query)
    {
       var filter = WorkoutSession.GetFilterExpression(query.SearchTerm, _userId!, _isAdmin);
       var sortExpression = WorkoutSession.GetSortExpression();
       
       var queryHelper = new QueryHelper().CreateQueriable(query, sortExpression, filter);
       var result = await _repository.GetAsync(queryHelper);

        return ServiceResponse.Success(result);
    }
    
    public async Task<ServiceResponse<WorkoutSession>> GetByIdAsync(int id)
    {
       
        var validator = new GetWorkoutSessionByIdValidator(_repository, _userService, _userId);
        var validationResult = await validator.ValidateAsync(id);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<WorkoutSession>(validationResult.ToDictionary()));
            return ServiceResponse.Failure<WorkoutSession>(EntityErrorMessage<WorkoutSession>.BadRequest(validationResult.ToDictionary()));
        }
        
        var result = await _repository.GetEntityByIdAsync(id);
        
        _logger.LogInformation(ServiceMessages.EntityRetrieved<WorkoutSession>(id));
        return ServiceResponse.Success(result);
    }

    public async Task<ServiceResponse<int>> CreateAsync(WorkoutSession entity)
    {
        var result = new CreateWorkoutSessionValidator(_repository, _userService, _userId);
        var validationResult = await result.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<WorkoutSession>(validationResult.ToDictionary()));
            return ServiceResponse.Failure<int>(EntityErrorMessage<WorkoutSession>.BadRequest(validationResult.ToDictionary()));
        }
        
        var id = await  _repository.CreateEntityAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityCreated<WorkoutSession>(id));
        return ServiceResponse.Success(id);
        
    }

    public async Task<ServiceResponse<bool>> UpdateAsync(WorkoutSession entity)
    {
        var result = new UpdateWorkoutSessionValidator(_repository, _userService, _userId);
        var validationResult = await result.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<WorkoutSession>(validationResult.ToDictionary()));
            return ServiceResponse.Failure<bool>(EntityErrorMessage<WorkoutSession>.BadRequest(validationResult.ToDictionary()));
        }
        
        var workoutSession = await _repository.GetEntityByIdAsync(entity.Id);
        
        workoutSession.Name = entity.Name ?? workoutSession.Name;
        workoutSession.DayOfWeek = entity.DayOfWeek;
        workoutSession.SortOrder = entity.SortOrder ?? workoutSession.SortOrder;
        
        
        await _repository.UpdateEntityAsync(workoutSession);
        
        
        _logger.LogInformation(ServiceMessages.EntityUpdated<WorkoutSession>(entity.Id));
        return ServiceResponse.Success(true);
        
        
    }

    public async Task<ServiceResponse<bool>> DeleteAsync(int id)
    {
       var validator = new DeleteWorkoutSessionValidator(_repository, _userService, _userId);
       var validationResult = await validator.ValidateAsync(id);

       if (!validationResult.IsValid)
       {
           _logger.LogWarning(ServiceMessages.EntityValidationFailure<WorkoutSession>(validationResult.ToDictionary()));
           return ServiceResponse.Failure<bool>(
               EntityErrorMessage<WorkoutSession>.BadRequest(validationResult.ToDictionary()));
       }
       
       await _repository.DeleteEntityAsync(id);
        
        _logger.LogInformation(ServiceMessages.EntityDeleted<WorkoutSession>(id));
        return ServiceResponse.Success(true);
    }

    public async Task<ServiceResponse<bool>> AddUserExerciseAsync(WorkoutSessionExerciseDto entity)
    {
        var validator = new WorkoutSessionExerciseValidator(_repository, _userExerciseRepository, _userService, _userId);
        var validationResult = await validator.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<WorkoutSession>(validationResult.ToDictionary()));
            return ServiceResponse.Failure<bool>(EntityErrorMessage<WorkoutSession>.BadRequest(validationResult.ToDictionary()));
        }
        
        var workoutSession = await _repository.GetEntityByIdAsync(entity.WorkoutSessionId);
        var userExercise = await _userExerciseRepository.GetEntityByIdAsync(entity.UserExerciseId);
        
        workoutSession!.UserExercises?.Add(userExercise!);
        
        await _repository.UpdateEntityAsync(workoutSession);
        
        _logger.LogInformation(ServiceMessages.EntityUpdated<WorkoutSession>(entity.WorkoutSessionId));
        
        return ServiceResponse.Success(true);
    }

    public async Task<ServiceResponse<bool>> RemoveUserExerciseAsync(WorkoutSessionExerciseDto entity)
    {
        
        var validator = new WorkoutSessionExerciseValidator(_repository, _userExerciseRepository, _userService, _userId);
        var validationResult = await validator.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<WorkoutSession>(validationResult.ToDictionary()));
            return ServiceResponse.Failure<bool>(EntityErrorMessage<WorkoutSession>.BadRequest(validationResult.ToDictionary()));
        }

        var workoutSession = await _repository.GetEntityByIdAsync(entity.WorkoutSessionId);
        var userExercise = await _userExerciseRepository.GetEntityByIdAsync(entity.UserExerciseId);
        
        workoutSession!.UserExercises?.Remove(userExercise!);
        
        await _repository.UpdateEntityAsync(workoutSession);
        
        _logger.LogInformation(ServiceMessages.EntityUpdated<WorkoutSession>(entity.WorkoutSessionId));
        
        return ServiceResponse.Success(true);
    }
}