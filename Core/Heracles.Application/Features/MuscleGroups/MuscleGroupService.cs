using System.Linq.Expressions;
using Heracles.Application.Features.MuscleGroups.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Queries;
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
    /// <returns>A Task representing the asynchronous operation. The result is a DomainResponse containing a QueryResponse of MuscleGroup objects.</returns>
    public async Task<DomainResponse<QueryResponse<MuscleGroup>>> GetAsync(QueryRequest query)
    {
        var filter = MuscleGroup.GetFilterExpression(query.SearchTerm);
       var sortExpressions = MuscleGroup.GetSortExpression();
       
       var  queryHelper = new QueryHelper().CreateQueriable(query, sortExpressions, filter);
       var result = await _repository.GetAsync(queryHelper);
        
        return DomainResponse.Success(
            new QueryResponse<MuscleGroup>
            {
                Data = result,
                PageNumber = query.PageNumber,
                PageSize = query.PageSize,
            }
        );
        
    }

    /// <summary>
    /// Retrieves a MuscleGroup with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the MuscleGroup to retrieve.</param>
    /// <returns>A Task representing the asynchronous operation. The result is a DomainResponse containing a MuscleGroup object.</returns>
    public async Task<DomainResponse<MuscleGroup>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<MuscleGroup>());
            return DomainResponse.Failure<MuscleGroup>(EntityErrorMessage<MuscleGroup>.BadRequest());
        }
        
        var muscleGroup = await _repository.GetByIdAsync(id);

        if (muscleGroup == null)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<MuscleGroup>(id));
            return DomainResponse.Failure<MuscleGroup>(EntityErrorMessage<MuscleGroup>.NotFound(id));
        }
        
        _logger.LogInformation(ServiceMessages.EntityRetrieved<MuscleGroup>(muscleGroup.Id));
        return DomainResponse.Success(muscleGroup);
    }

    /// <summary>
    /// Creates a new MuscleGroup asynchronously.
    /// </summary>
    /// <param name="entity">The MuscleGroup entity to be created.</param>
    /// <returns>A Task representing the asynchronous operation. The result is a DomainResponse containing an integer representing the ID of the created MuscleGroup.</returns>
    public async Task<DomainResponse<int>> CreateAsync(MuscleGroup entity)
    {
        // validate muscle group
        var validate = new CreateMuscleGroupValidator(_repository);
        var validationResult = await validate.ValidateAsync(entity);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<MuscleGroup>(validationResult.ToDictionary()));
            return DomainResponse.Failure<int>(EntityErrorMessage<MuscleGroup>.BadRequest(validationResult.ToDictionary()));
        }
        
        // create muscle group
        var id = await _repository.CreateAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityCreated<MuscleGroup>(id));
        
        return DomainResponse.Success(id);
    }

    /// <summary>
    /// Updates a MuscleGroup entity.
    /// </summary>
    /// <param name="entity">The MuscleGroup entity to update.</param>
    /// <returns>A Task representing the asynchronous operation. The result is a DomainResponse containing a boolean value indicating whether the entity was successfully updated.</returns>
    public async Task<DomainResponse<bool>> UpdateAsync(MuscleGroup entity)
    {
        var validate = new UpdateMuscleGroupValidator(_repository);
        var validationResult = await validate.ValidateAsync(entity);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<MuscleGroup>(validationResult.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<MuscleGroup>.BadRequest(validationResult.ToDictionary()));
        }
        
        
        await _repository.UpdateAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityUpdated<MuscleGroup>(entity.Id));
        return DomainResponse.Success(true);
    }

    /// <summary>
    /// Deletes a MuscleGroup object with the provided ID.
    /// </summary>
    /// <param name="id">The ID of the MuscleGroup to delete.</param>
    /// <returns>A Task representing the asynchronous operation. The result is a DomainResponse of type bool. If the operation is successful, the response object will have a value of true. Otherwise, it will contain an error message.</returns>
    public async Task<DomainResponse<bool>> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<MuscleGroup>());
            return DomainResponse.Failure<bool>(EntityErrorMessage<MuscleGroup>.BadRequest());
        }
        
        var itExist = await _repository.ItExist(id);
        if (!itExist)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<MuscleGroup>(id));
            return DomainResponse.Failure<bool>(EntityErrorMessage<MuscleGroup>.NotFound(id));
        }
        
        await _repository.DeleteAsync(id);
        _logger.LogInformation(ServiceMessages.EntityDeleted<MuscleGroup>(id));
        
        return DomainResponse.Success(true);
    }
}