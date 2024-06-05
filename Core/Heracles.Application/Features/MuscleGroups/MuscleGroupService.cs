using System.Linq.Expressions;
using Heracles.Application.Features.MuscleGroups.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.MuscleGroups.Interfaces;
using Heracles.Domain.MuscleGroups.Models;

namespace Heracles.Application.Features.MuscleGroups;

public class MuscleGroupService : IMuscleGroupService
{
    private readonly IAppLogger<MuscleGroupService> _logger;
    private readonly IMuscleGroupRepository _repository;

    public MuscleGroupService(IAppLogger<MuscleGroupService> logger, IMuscleGroupRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }


    /// <summary>
    /// Retrieves a collection of MuscleGroups based on the provided query parameters.
    /// </summary>
    /// <param name="query">The query containing the search term, page number, and page size.</param>
    /// <returns>A Task representing the asynchronous operation. The result is a ServiceResponse containing a QueryResponseDto of MuscleGroup objects.</returns>
    public async Task<ServiceResponse<QueryResponseDto<MuscleGroup>>> GetAsync(QueryRequestDto query)
    {
        var filter = MuscleGroup.GetFilterExpression(query.SearchTerm);
       var sortExpressions = MuscleGroup.GetSortExpression();
       
       var  queryHelper = new QueryHelper().CreateQueriable(query, sortExpressions, filter);
       var result = await _repository.GetAsync(queryHelper);
        
       return ServiceResponse.Success(result);
        
    }

    /// <summary>
    /// Retrieves a MuscleGroup with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the MuscleGroup to retrieve.</param>
    /// <returns>A Task representing the asynchronous operation. The result is a ServiceResponse containing a MuscleGroup object.</returns>
    public async Task<ServiceResponse<MuscleGroup>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<MuscleGroup>());
            return ServiceResponse.Failure<MuscleGroup>(EntityErrorMessage<MuscleGroup>.BadRequest());
        }
        
        var muscleGroup = await _repository.GetEntityByIdAsync(id);

        if (muscleGroup == null)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<MuscleGroup>(id));
            return ServiceResponse.Failure<MuscleGroup>(EntityErrorMessage<MuscleGroup>.NotFound(id));
        }
        
        _logger.LogInformation(ServiceMessages.EntityRetrieved<MuscleGroup>(muscleGroup.Id));
        return ServiceResponse.Success(muscleGroup);
    }

    /// <summary>
    /// Creates a new MuscleGroup asynchronously.
    /// </summary>
    /// <param name="entity">The MuscleGroup entity to be created.</param>
    /// <returns>A Task representing the asynchronous operation. The result is a ServiceResponse containing an integer representing the ID of the created MuscleGroup.</returns>
    public async Task<ServiceResponse<int>> CreateAsync(MuscleGroup entity)
    {
        // validate muscle group
        var validate = new CreateMuscleGroupValidator(_repository);
        var validationResult = await validate.ValidateAsync(entity);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<MuscleGroup>(validationResult.ToDictionary()));
            return ServiceResponse.Failure<int>(EntityErrorMessage<MuscleGroup>.BadRequest(validationResult.ToDictionary()));
        }
        
        // create muscle group
        var id = await _repository.CreateEntityAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityCreated<MuscleGroup>(id));
        
        return ServiceResponse.Success(id);
    }

    /// <summary>
    /// Updates a MuscleGroup entity.
    /// </summary>
    /// <param name="entity">The MuscleGroup entity to update.</param>
    /// <returns>A Task representing the asynchronous operation. The result is a ServiceResponse containing a boolean value indicating whether the entity was successfully updated.</returns>
    public async Task<ServiceResponse<bool>> UpdateAsync(MuscleGroup entity)
    {
        var validate = new UpdateMuscleGroupValidator(_repository);
        var validationResult = await validate.ValidateAsync(entity);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<MuscleGroup>(validationResult.ToDictionary()));
            return ServiceResponse.Failure<bool>(EntityErrorMessage<MuscleGroup>.BadRequest(validationResult.ToDictionary()));
        }
        
        
        await _repository.UpdateEntityAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityUpdated<MuscleGroup>(entity.Id));
        return ServiceResponse.Success(true);
    }

    /// <summary>
    /// Deletes a MuscleGroup object with the provided ID.
    /// </summary>
    /// <param name="id">The ID of the MuscleGroup to delete.</param>
    /// <returns>A Task representing the asynchronous operation. The result is a ServiceResponse of type bool. If the operation is successful, the response object will have a value of true. Otherwise, it will contain an error message.</returns>
    public async Task<ServiceResponse<bool>> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<MuscleGroup>());
            return ServiceResponse.Failure<bool>(EntityErrorMessage<MuscleGroup>.BadRequest());
        }
        
        var itExist = await _repository.ItExist(id);
        if (!itExist)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<MuscleGroup>(id));
            return ServiceResponse.Failure<bool>(EntityErrorMessage<MuscleGroup>.NotFound(id));
        }
        
        await _repository.DeleteEntityAsync(id);
        _logger.LogInformation(ServiceMessages.EntityDeleted<MuscleGroup>(id));
        
        return ServiceResponse.Success(true);
    }
}