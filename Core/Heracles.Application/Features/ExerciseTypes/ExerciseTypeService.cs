using System.Linq.Expressions;
using Heracles.Application.Features.ExerciseTypes.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.ExercisesType.Models;

namespace Heracles.Application.Features.ExerciseTypes;

public class ExerciseTypeService : IExerciseTypeService
{
    private readonly IAppLogger<ExerciseTypeService> _logger;
    private readonly IExerciseTypeRepository _repository;

    public ExerciseTypeService(
        IAppLogger<ExerciseTypeService> logger,
        IExerciseTypeRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    /// <summary>
    /// Represents a query request for filtering and sorting a collection of ExerciseType entities asynchronously.
    /// </summary>
    public async Task<ServiceResponse<QueryResponseDto<ExerciseType>>> GetAsync(QueryRequestDto query)
    {
        var filter = ExerciseType.GetFilterExpression(query?.SearchTerm);
        var sortExpressions = ExerciseType.GetSortExpression();

        var queryHelper = new QueryHelper().CreateQueriable(query, sortExpressions, filter);
        var result = await _repository.GetAsync(queryHelper);
       
        return ServiceResponse.Success(result);
    }


    /// <summary>
    /// Retrieves an ExerciseType entity asynchronously based on the provided ID.
    /// </summary>
    /// <param name="id">The ID of the ExerciseType entity to retrieve.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a ServiceResponse object containing the retrieved ExerciseType entity.</returns>
    public async Task<ServiceResponse<ExerciseType>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<ExerciseType>());
            return ServiceResponse.Failure<ExerciseType>(EntityErrorMessage<ExerciseType>.BadRequest());
        }
            
        
        var exerciseType = await _repository.GetEntityByIdAsync(
            id,"MuscleGroups.Muscle", "MuscleGroups.Function" );

        if (exerciseType == null)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<ExerciseType>(id));
            return ServiceResponse.Failure<ExerciseType>(EntityErrorMessage<ExerciseType>.NotFound(id));
        }
           
        _logger.LogInformation(ServiceMessages.EntityRetrieved<ExerciseType>(exerciseType.Id));
        return ServiceResponse.Success(exerciseType);
        
    }

    /// <summary>
    /// Creates a new ExerciseType entity asynchronously.
    /// </summary>
    /// <param name="entity">The ExerciseType entity to create.</param>
    /// <returns>A Task representing the asynchronous operation. The Task's result contains a ServiceResponse<int> object that represents the result of the creation operation.</returns>
    public async Task<ServiceResponse<int>> CreateAsync(ExerciseType entity)
    {
        // validate exercise structure
        var validator = new CreateExerciseTypeValidator(_repository);
        var validationResult = await validator.ValidateAsync(entity);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<ExerciseType>(validationResult.ToDictionary()));
            return ServiceResponse.Failure<int>(EntityErrorMessage<ExerciseType>.BadRequest(validationResult.ToDictionary()));
        }
        
        // add exercise to the database
        var exerciseId = await _repository.CreateEntityAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityCreated<ExerciseType>(entity.Id));
        
        return ServiceResponse.Success(exerciseId);
    }

    /// <summary>
    /// Represents an asynchronous method for updating an ExerciseType entity.
    /// </summary>
    /// <param name="entity">The ExerciseType entity to be updated.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains a ServiceResponse<bool> indicating the success or failure of the update operation.</returns>
    public async Task<ServiceResponse<bool>> UpdateAsync(ExerciseType entity)
    {
        var validator = new UpdateExerciseTypeValidator(_repository);
        var validationResult = await validator.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<ExerciseType>(validationResult.ToDictionary()));
            return ServiceResponse.Failure<bool>(EntityErrorMessage<ExerciseType>.BadRequest(validationResult.ToDictionary()));
        }
        
        await _repository.UpdateEntityAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityUpdated<ExerciseType>(entity.Id));
        return ServiceResponse.Success(true);
    }

    /// <summary>
    /// Represents a delete request for deleting an ExerciseType entity asynchronously.
    /// </summary>
    /// <param name="id">The identifier of the ExerciseType entity to delete.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains a ServiceResponse with a boolean value indicating the success of the delete operation.</returns>
    public async Task<ServiceResponse<bool>> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<ExerciseType>());
            return ServiceResponse.Failure<bool>(EntityErrorMessage<ExerciseType>.BadRequest());
        }
        
        var itExist = await _repository.ItExist(id);
        if (!itExist)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<ExerciseType>(id));
            return ServiceResponse.Failure<bool>(EntityErrorMessage<ExerciseType>.NotFound(id));
        }
        
        await _repository.DeleteEntityAsync(id);
        _logger.LogInformation(ServiceMessages.EntityDeleted<ExerciseType>(id));
        return ServiceResponse.Success(true);
    }
}