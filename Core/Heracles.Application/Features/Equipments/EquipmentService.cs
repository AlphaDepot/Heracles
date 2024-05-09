using System.Linq.Expressions;
using Heracles.Application.Features.Equipments.Validators;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Heracles.Domain.Equipments.Interfaces;
using Heracles.Domain.Equipments.Models;

namespace Heracles.Application.Features.Equipments;

/// <summary>
///  Equipment service
/// </summary>
public class EquipmentService : IEquipmentService
{
    
    private readonly IAppLogger<EquipmentService> _logger;
    private readonly IEquipmentRepository _repository;
    
    public EquipmentService(
        IAppLogger<EquipmentService> logger,
        IEquipmentRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    
    /// <summary>
    ///  Get all equipments with pagination, sorting and search 
    /// </summary>
    /// <param name="query"> Query request </param>
    /// <returns> Query response </returns>
    public async Task<DomainResponse<QueryResponse<Equipment>>> GetAsync(QueryRequest query)
    {
        
        var filter = Equipment.GetFilterExpression(query?.SearchTerm);
        var sortExpressions = Equipment.GetSortExpression();
        
        var  queryHelper = new QueryHelper().CreateQueriable(query, sortExpressions, filter);
        //var result =  await _repository.GetAsync(queryHelper.Filter, queryHelper.Sorter, query.PageSize, query.PageNumber);
        var result = await _repository.GetAsync(queryHelper);
      
        return DomainResponse.Success(result);
    }
    /// <summary>
    ///  Get equipment by id
    /// </summary>
    /// <param name="id"> Equipment id </param>
    /// <returns> Equipment </returns>
    public async Task<DomainResponse<Equipment>> GetByIdAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<Equipment>());
            return DomainResponse.Failure<Equipment>(EntityErrorMessage<Equipment>.BadRequest());
        }
        
        var result = await _repository.GetByIdAsync(id);
        
        if (result == null)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<Equipment>(id));
            return DomainResponse.Failure<Equipment>(EntityErrorMessage<Equipment>.NotFound(id));
        }
        
        _logger.LogInformation(ServiceMessages.EntityRetrieved<Equipment>(id));
        return DomainResponse.Success(result);
        
        
    }

    /// <summary>
    ///  Create equipment
    /// </summary>
    /// <param name="entity"> Equipment </param>
    /// <returns> Id </returns>
    public async Task<DomainResponse<int>> CreateAsync(Equipment entity)
    {
        var validator = new CreateEquipmentValidator(_repository);
        var validationResult = await validator.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<Equipment>(validationResult.ToDictionary()));
            return DomainResponse.Failure<int>(EntityErrorMessage<Equipment>.BadRequest(validationResult.ToDictionary()));
        }
        
        var id = await _repository.CreateAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityCreated<Equipment>(id));
        
        return DomainResponse.Success(id);
    }

    /// <summary>
    ///  Update equipment 
    /// </summary>
    /// <param name="entity"> Equipment </param>
    /// <returns> Success </returns>
    public async Task<DomainResponse<bool>> UpdateAsync(Equipment entity)
    {
        var validator = new UpdateEquipmentValidator(_repository);
        var validationResult = await validator.ValidateAsync(entity);
        
        if (!validationResult.IsValid)
        {
            _logger.LogWarning(ServiceMessages.EntityValidationFailure<Equipment>(validationResult.ToDictionary()));
            return DomainResponse.Failure<bool>(EntityErrorMessage<Equipment>.BadRequest(validationResult.ToDictionary()));
        }
        
        await _repository.UpdateAsync(entity);
        _logger.LogInformation(ServiceMessages.EntityUpdated<Equipment>(entity.Id));
        
        return DomainResponse.Success(true);
    }

    /// <summary>
    ///  Delete equipment
    /// </summary>
    /// <param name="id"> Equipment id </param>
    /// <returns> Success or failure </returns>
    public async  Task<DomainResponse<bool>> DeleteAsync(int id)
    {
        if (id <= 0)
        {
            _logger.LogWarning(ServiceMessages.EntityIdInvalid<Equipment>());
            return DomainResponse.Failure<bool>(EntityErrorMessage<Equipment>.BadRequest());
        }
        
        var exist = await _repository.ItExist(id);
        if (!exist)
        {
            _logger.LogWarning(ServiceMessages.EntityNotFound<Equipment>(id));
            return DomainResponse.Failure<bool>(EntityErrorMessage<Equipment>.NotFound(id));
        }
        
        await _repository.GetByIdAsync(id);
        
        _logger.LogInformation(ServiceMessages.EntityDeleted<Equipment>(id));
        return DomainResponse.Success(true);
    }
}