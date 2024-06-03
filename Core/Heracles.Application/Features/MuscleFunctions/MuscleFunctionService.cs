using System.Linq.Expressions;
using Heracles.Application.Features.MuscleFunctions.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.MuscleFunctions.Interfaces;
using Heracles.Domain.MuscleFunctions.Models;

namespace Heracles.Application.Features.MuscleFunctions;

public class MuscleFunctionService : IMuscleFunctionService
{
    private readonly IAppLogger<MuscleFunctionService> _logger;
    private readonly IMuscleFunctionRepository _repository;

    public MuscleFunctionService(
        IAppLogger<MuscleFunctionService> logger, 
        IMuscleFunctionRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    /// <summary>
    /// Retrieves muscle functions based on the specified query.
    /// </summary>
    /// <param name="query">The query used to filter and sort the muscle functions.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the domain response with the filtered and sorted muscle functions.</returns>
    public async Task<DomainResponse<QueryResponseDto<MuscleFunction>>> GetAsync(QueryRequestDto query)
    {
        var filter = MuscleFunction.GetFilterExpression(query?.SearchTerm);
        var sortExpressions = MuscleFunction.GetSortExpression();
        
        var  queryHelper = new QueryHelper().CreateQueriable(query, sortExpressions, filter);
        var result = await _repository.GetAsync(queryHelper);
        
        return DomainResponse.Success(result);
        
    }

    /// <summary>
    /// Retrieves a muscle function by its ID.
    /// </summary>
    /// <param name="id">The ID of the muscle function.</param>
    /// <returns>
    /// A task representing the asynchronous operation. The task result contains a
    /// <see cref="DomainResponse{TValue}"/> object with the muscle function retrieved
    /// from the repository. If the ID is invalid or the muscle function is not found,
    /// a failure response with an appropriate error message is returned.
    /// </returns>
    public async Task<DomainResponse<MuscleFunction>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<MuscleFunction>());
            return DomainResponse.Failure<MuscleFunction>(EntityErrorMessage<MuscleFunction>.BadRequest());
        }
        
        var muscleFunction = await _repository.GetByIdAsync(id);
        
        if (muscleFunction == null)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<MuscleFunction>(id));
            return DomainResponse.Failure<MuscleFunction>(EntityErrorMessage<MuscleFunction>.NotFound(id));
        }
        
        _logger.LogInformation(ServiceMessages.EntityRetrieved<MuscleFunction>(id));
        return DomainResponse.Success(muscleFunction);
        
    }

    /// <summary>
    /// Creates a new muscle function asynchronously.
    /// </summary>
    /// <param name="entity">The muscle function entity to create.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the domain response with the ID of the created muscle function.</returns>
    public async Task<DomainResponse<int>> CreateAsync(MuscleFunction entity)
    {
        var validate = new CreateMuscleFunctionValidator(_repository);
        var validationResult = await validate.ValidateAsync(entity);

        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<MuscleFunction>(validationResult.ToDictionary()));
            return DomainResponse.Failure<int>(EntityErrorMessage<MuscleFunction>.BadRequest(validationResult.ToDictionary()));
        }
        
        // create muscle function
        var id = await _repository.CreateAsync(entity);
        
        _logger.LogInformation(ServiceMessages.EntityCreated<MuscleFunction>(id));
        return DomainResponse.Success(id);
    }

    /// <summary>
    /// Updates the specified muscle function.
    /// </summary>
    /// <param name="entity">The muscle function entity to update.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the domain response indicating the success of the update operation.</returns>
    public async Task<DomainResponse<bool>> UpdateAsync(MuscleFunction entity)
    {
        var validate = new UpdateMuscleFunctionValidator(_repository);
        var validationResult = await validate.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<MuscleFunction>(validationResult.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<MuscleFunction>.BadRequest(validationResult.ToDictionary()));
        }
        
        
        await _repository.UpdateAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityUpdated<MuscleFunction>(entity.Id));
        
        return DomainResponse.Success(true);
        
    }

    /// <summary>
    /// Deletes a muscle function based on the specified ID.
    /// </summary>
    /// <param name="id">The ID of the muscle function to be deleted.</param>
    /// <returns>A task representing the asynchronous operation. The task result contains the domain response indicating the success or failure of the deletion.</returns>
    public async Task<DomainResponse<bool>> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<MuscleFunction>());
            return DomainResponse.Failure<bool>(EntityErrorMessage<MuscleFunction>.BadRequest());
        }

        var itExist = await _repository.ItExist(id);
        if (!itExist)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<MuscleFunction>(id));
            return DomainResponse.Failure<bool>(EntityErrorMessage<MuscleFunction>.NotFound(id));
        }
        
        await _repository.DeleteAsync(id);
        _logger.LogInformation(ServiceMessages.EntityDeleted<MuscleFunction>(id));
        
        return DomainResponse.Success(true);
    }
}