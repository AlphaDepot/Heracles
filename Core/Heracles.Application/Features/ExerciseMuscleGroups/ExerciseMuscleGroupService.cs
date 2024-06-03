using System.Linq.Expressions;
using Heracles.Application.Features.ExerciseMuscleGroups.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.ExerciseMuscleGroups.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.Interfaces;
using Heracles.Domain.ExerciseMuscleGroups.Models;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.MuscleFunctions.Interfaces;
using Heracles.Domain.MuscleGroups.Interfaces;

namespace Heracles.Application.Features.ExerciseMuscleGroups;

/// <summary>
/// Service for managing exercise muscle groups.
/// </summary>
public class ExerciseMuscleGroupService : IExerciseMuscleGroupService
{
    private readonly IExerciseMuscleGroupRepository _repository;
    private readonly IMuscleGroupRepository _muscleGroupRepository;
    private readonly IMuscleFunctionRepository _muscleFunctionRepository;
    private readonly IExerciseTypeRepository _exerciseTypeRepository;

    private readonly IAppLogger<ExerciseMuscleGroupService> _logger;

    public ExerciseMuscleGroupService(
        IAppLogger<ExerciseMuscleGroupService> logger,
        IExerciseMuscleGroupRepository repository,
        IMuscleGroupRepository muscleGroupRepository,
        IMuscleFunctionRepository muscleFunctionRepository,
        IExerciseTypeRepository exerciseTypeRepository)
    {
        _repository = repository;
        _muscleGroupRepository = muscleGroupRepository;
        _muscleFunctionRepository = muscleFunctionRepository;
        _exerciseTypeRepository = exerciseTypeRepository;

        _logger = logger;
    }

    /// <summary>
    ///  Get all exercise muscle groups with optional search term, sorting and pagination.
    /// </summary>
    /// <param name="query"> The QueryRequestDto object containing the search term, sort column and sort order.</param>
    /// <returns> A DomainResponse object containing the QueryResponseDto object with the list of exercise muscle groups.</returns>
    public async Task<DomainResponse<QueryResponseDto<ExerciseMuscleGroup>>> GetAsync(QueryRequestDto? query = null)
    {
        // Set the default query values if the query is null
        query ??= new QueryRequestDto();
        
        // Filter the exercise muscle groups
        var filter = ExerciseMuscleGroup.GetFilterExpression(query?.SearchTerm);

        // Get the sort expressions for the exercise muscle groups
        var sortExpressions = ExerciseMuscleGroup.GetSortExpression();
        
        // Create the query helper
        var  queryHelper = new QueryHelper().CreateQueriable(query, sortExpressions, filter);

        // Get the exercise muscle groups
       var result = await _repository.GetAsync(queryHelper);
       
       return DomainResponse.Success(result);
        
    }
    
    /// <summary>
    ///     Get all exercise muscle groups for a specific exercise type.
    /// </summary>
    /// <param name="id"> The id of the exercise type.</param>
    /// <param name="query"> The QueryRequestDto object containing the search term, sort column and sort order.</param>
    /// <returns>  QueryResponseDto object containing the list of exercise muscle groups for the exercise type.</returns>
    public async Task<DomainResponse<QueryResponseDto<ExerciseMuscleGroup>>> GetByExerciseIdAsync(int id, QueryRequestDto? query = null)
    {
        // Set the default query values if the query is null
        query ??= new QueryRequestDto();
        
        // fixed page size and number
        // in theory no exercise should have more than 20 affected major muscle groups
        query.PageSize = 20;
        query.PageNumber = 1;
        
       // manually create the filter expression
       // filter by exercise type id as default
        Expression<Func<ExerciseMuscleGroup, bool> > filter = e => e.ExerciseTypeId == id;
        //  filter by search term if provided
        if (!string.IsNullOrEmpty(query?.SearchTerm))
        {
            filter = e => e.ExerciseTypeId == id && e.Muscle.Name.Contains(query.SearchTerm);
        }
        
        // Get the sort expressions for the exercise muscle groups
        var sortExpressions = ExerciseMuscleGroup.GetSortExpression();
        
        // Create the query helper
        var  queryHelper = new QueryHelper().CreateQueriable(query, sortExpressions, filter);
        
        var result = await _repository.GetByExerciseIdAsync(queryHelper);
        
        return DomainResponse.Success(result);
        
    }

    /// <summary>
    /// Retrieves an ExerciseMuscleGroup by its ID.
    /// </summary>
    /// <param name="id">The ID of the ExerciseMuscleGroup to retrieve.</param>
    /// <returns>A DomainResponse object containing the ExerciseMuscleGroup if successfully retrieved, or an error message if the ID is invalid or the ExerciseMuscleGroup is not found.</returns>
    public async Task<DomainResponse<ExerciseMuscleGroup>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<ExerciseMuscleGroup>());
            return DomainResponse.Failure<ExerciseMuscleGroup>(EntityErrorMessage<ExerciseMuscleGroup>.BadRequest());
        }
            
        
        var exerciseMuscleGroup = await _repository.GetEntityByIdAsync(id);
        if (exerciseMuscleGroup == null)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<ExerciseMuscleGroup>(id));
            return DomainResponse.Failure<ExerciseMuscleGroup>(EntityErrorMessage<ExerciseMuscleGroup>.NotFound(id));
        }
            
        _logger.LogInformation(ServiceMessages.EntityRetrieved<ExerciseMuscleGroup>(exerciseMuscleGroup.Id));
        return DomainResponse.Success(exerciseMuscleGroup);
    }


    /// <summary>
    /// Creates a new exercise muscle group with the specified data.
    /// </summary>
    /// <param name="entity">The CreateExerciseMuscleGroupDto object containing the data for the new exercise muscle group.</param>
    /// <returns>A DomainResponse object containing the result of the create operation.</returns>
    public async Task<DomainResponse<int>> CreateAsync(CreateExerciseMuscleGroupDto entity)
    {
        // Validate the entity
        var validator = new CreateExerciseMuscleGroupDtoValidator(
            _repository, _muscleGroupRepository, _muscleFunctionRepository, _exerciseTypeRepository);
        
        var validationResult = await validator.ValidateAsync(entity);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<ExerciseMuscleGroup>(validationResult.ToDictionary()));
            return DomainResponse.Failure<int>(EntityErrorMessage<ExerciseMuscleGroup>.BadRequest(validationResult.ToDictionary()));
        }
        
    
        var muscleGroup = await _muscleGroupRepository.GetEntityByIdAsync(entity.MuscleGroupId);
        var muscleFunction = await _muscleFunctionRepository.GetEntityByIdAsync(entity.MuscleFunctionId);
        
        
        // Create the exercise muscle group
        var exerciseMuscleGroup = new ExerciseMuscleGroup
        {
            ExerciseTypeId = entity.ExerciseTypeId,
            Muscle = muscleGroup,
            Function = muscleFunction,
            FunctionPercentage = entity.FunctionPercentage
            
        };
        
        // Add the exercise muscle group to the database
        var result = await _repository.CreateEntityAsync(exerciseMuscleGroup);
        _logger.LogInformation(ServiceMessages.EntityCreated<ExerciseMuscleGroup>(result));
        
        return DomainResponse.Success(result);

    }


    /// <summary>
    /// Update an exercise muscle group with the provided information.
    /// </summary>
    /// <param name="entity">The UpdateExerciseMuscleGroupDto object containing the updated information for the exercise muscle group.</param>
    /// <returns>A DomainResponse object indicating the success or failure of the update operation.</returns>
    public async Task<DomainResponse<bool>> UpdateAsync(UpdateExerciseMuscleGroupDto entity)
    {
        // Validate the entity
        var validator = new UpdateExerciseMuscleGroupDtoValidator(_repository);
        var validationResult = await validator.ValidateAsync(entity);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<ExerciseMuscleGroup>(validationResult.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<ExerciseMuscleGroup>.BadRequest(validationResult.ToDictionary()));
        }
            
        
        // Get exercise muscle group - existence already validated by the validator
        var exerciseMuscleGroup = await _repository.GetEntityByIdAsync(entity.Id);
        
        // Only the function percentage can be updated
        exerciseMuscleGroup.FunctionPercentage = entity.FunctionPercentage;
        
        // Update the exercise muscle group in the database
        await _repository.UpdateEntityAsync(exerciseMuscleGroup);
        _logger.LogInformation(ServiceMessages.EntityUpdated<ExerciseMuscleGroup>(entity.Id));
        
        return DomainResponse.Success(true);
        
    }

    /// <summary>
    /// Deletes an exercise muscle group by ID.
    /// </summary>
    /// <param name="id">The ID of the exercise muscle group.</param>
    /// <returns>A DomainResponse object indicating the result of the delete operation.</returns>
    public async Task<DomainResponse<bool>> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<ExerciseMuscleGroup>());
            return DomainResponse.Failure<bool>(EntityErrorMessage<ExerciseMuscleGroup>.BadRequest());
        }
        
        var itExist = await _repository.ItExist(id);
        if (!itExist)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<ExerciseMuscleGroup>(id));
            return DomainResponse.Failure<bool>(EntityErrorMessage<ExerciseMuscleGroup>.NotFound(id));
        }
        
        
        await _repository.DeleteEntityAsync(id);
        _logger.LogInformation(ServiceMessages.EntityDeleted<ExerciseMuscleGroup>(id));
        
        return DomainResponse.Success(true);
    }
    
    
    
}