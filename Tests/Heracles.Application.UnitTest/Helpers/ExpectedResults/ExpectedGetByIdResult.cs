using FluentAssertions;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;
using Moq;

namespace Heracles.Application.UnitTest.Helpers.ExpectedResults;

/// <summary>
/// This class provides helper methods for asserting the expected results of the GetByIdAsync method in the ExerciseTypeService class.
/// </summary>
public static class ExpectedGetByIdResult
{
    /// <summary>
    /// Asserts that the GetById method of a service returns a successful result.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="logger">The mock logger instance.</param>
    /// <param name="result">The domainResponse from the GetById method execution.</param>
    /// <param name="expected">The expected entity retrieved from the database.</param>
    /// <param name="id">The ID used to retrieve the entity.</param>
    /// <returns></returns>
    public static void Success<TService, TEntity>(Mock<IAppLogger<TService>> logger, DomainResponse<TEntity> result, TEntity expected, int id) where TEntity : BaseEntity
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(expected);
        logger.Verify(x => x.LogInformation(ServiceMessages.EntityRetrieved<TEntity>(id)), Times.Once);
    }
    
    /// <summary>
    /// Handles the case where a bad request is encountered when retrieving an entity by its ID.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="result">The domainResponse object.</param>
    /// <param name="expectedError">The expected error.</param>
    /// <returns>None</returns>
    public static void BadRequest<TService, TEntity>(Mock<IAppLogger<TService>> logger, DomainResponse<TEntity> result, Error expectedError) where TEntity : BaseEntity
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(expectedError);
        logger.Verify(x => x.LogWarning(ServiceMessages.EntityIdInvalid<TEntity>()), Times.Once);
    }

    /// <summary>
    ///  Handles the case where a validation failure is encountered when retrieving an entity by its ID.
    /// </summary>
    /// <param name="logger"> The logger instance.</param>
    /// <param name="result"> The domainResponse object.</param>
    /// <param name="errors"> The validation errors.</param>
    /// <typeparam name="TService"> The type of the service.</typeparam>
    /// <typeparam name="TEntity"> The type of the entity.</typeparam>
    public static void ValidationFail<TService, TEntity>(Mock<IAppLogger<TService>> logger, DomainResponse<TEntity> result, IDictionary<string, string[]> errors) where TEntity : BaseEntity
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<Error>();
        result.Error.Code.Should().BeOfType<string>();
        logger.Verify(x => x.LogWarning(ServiceMessages.EntityValidationFailure<TEntity>(errors)), Times.Once);
    }
    
    /// <summary>
    /// Method for handling the scenario when an entity is not found by its ID.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="result">The domainResponse result.</param>
    /// <param name="expectedError">The expected error.</param>
    /// <param name="id">The ID of the entity.</param>
    /// <remarks>
    /// This method verifies that the domainResponse is a failure and the error matches the expected error.
    /// It also logs a warning message indicating that the entity with the specified ID was not found.
    /// </remarks>
    public static void NotFound<TService, TEntity>(Mock<IAppLogger<TService>> logger, DomainResponse<TEntity> result, Error expectedError, int id) where TEntity : BaseEntity
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeEquivalentTo(expectedError);
        logger.Verify(x => x.LogWarning(ServiceMessages.EntityNotFound<TEntity>(id)), Times.Once);
    }
}