using FluentAssertions;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.Entities;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Moq;

namespace Heracles.Application.UnitTest.Helpers.ExpectedResults;

/// <summary>
/// Helper class for defining expected results of delete operations.
/// </summary>
public static class ExpectedDeleteResult
{
    /// <summary>
    /// Deletes an entity successfully.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="logger">The logger mock.</param>
    /// <param name="result">The domainResponse result.</param>
    /// <param name="id">The ID of the entity.</param>
    public static void Success<TService, TEntity>(Mock<IAppLogger<TService>> logger, DomainResponse<bool> result, int id) where TEntity : BaseEntity
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        logger.Verify(x => x.LogInformation(ServiceMessages.EntityDeleted<TEntity>(id)), Times.Once);
    }


    /// <summary>
    /// Validates the delete result when a bad request occurs.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="logger">The logger.</param>
    /// <param name="result">The delete result.</param>
    /// <param name="expectedError">The expected error.</param>
    /// <returns>Nothing.</returns>
    public static void BadRequest<TService, TEntity>(Mock<IAppLogger<TService>> logger, DomainResponse<bool> result, Error expectedError) where TEntity : BaseEntity
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(expectedError);
        logger.Verify(x => x.LogWarning(ServiceMessages.EntityIdInvalid<TEntity>()), Times.Once);
    }
    
    /// <summary>
    ///  Handles the case where a validation failure is encountered when deleting an entity.
    /// </summary>
    /// <param name="logger"> The logger instance.</param>
    /// <param name="result"> The domainResponse object.</param>
    /// <param name="errors"> The validation errors.</param>
    /// <typeparam name="TService"> The type of the service.</typeparam>
    /// <typeparam name="TEntity"> The type of the entity.</typeparam>
    public static void ValidationFail<TService, TEntity>(Mock<IAppLogger<TService>> logger, DomainResponse<bool> result, IDictionary<string, string[]> errors) where TEntity : BaseEntity
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<Error>();
        result.Error.Code.Should().BeOfType<string>();
        logger.Verify(x => x.LogWarning(ServiceMessages.EntityValidationFailure<TEntity>(errors)), Times.Once);
    }


    /// <summary>
    /// Deletes an entity with the specified ID and returns a domainResponse indicating whether the delete operation was successful.
    /// If the entity is not found, it logs a warning and sets the domainResponse error accordingly.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="result">The domainResponse result.</param>
    /// <param name="expectedError">The expected error if the entity is not found.</param>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <exception cref="System.InvalidOperationException">The value of a failure domainResponse can't be accessed.</exception>
    public static void NotFound<TService, TEntity>(Mock<IAppLogger<TService>> logger, DomainResponse<bool> result, Error expectedError, int id) where TEntity : BaseEntity
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(expectedError);
        logger.Verify(x => x.LogWarning(ServiceMessages.EntityNotFound<TEntity>(id)), Times.Once);
    }
    
    
}