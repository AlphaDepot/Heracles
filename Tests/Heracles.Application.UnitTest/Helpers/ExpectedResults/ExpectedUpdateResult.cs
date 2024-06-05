using FluentAssertions;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.Entities;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Moq;

namespace Heracles.Application.UnitTest.Helpers.ExpectedResults;

/// <summary>
/// Provides static methods to assert the expected results of update operations.
/// </summary>
public static class ExpectedUpdateResult
{
    /// <summary>
    /// Updates the success result of an update operation.
    /// </summary>
    /// <typeparam name="TService">The type of service.</typeparam>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="logger">The logger.</param>
    /// <param name="result">The update result.</param>
    /// <param name="id">The ID of the entity being updated.</param>
    public static void Success<TService, TEntity>(Mock<IAppLogger<TService>> logger, ServiceResponse<bool> result, int id) where TEntity : BaseEntity
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeTrue();
        logger.Verify(x => x.LogInformation(ServiceMessages.EntityUpdated<TEntity>(id)), Times.Once);
    }


    /// <summary>
    /// Handles the updating of a bad request serviceResponse.
    /// </summary>
    /// <typeparam name="TService">The service type.</typeparam>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="result">The serviceResponse result.</param>
    /// <param name="errors">The dictionary of errors.</param>
    /// <exception cref="InvalidOperationException">Thrown when the value of a failure serviceResponse is accessed.</exception>
    public static void BadRequest<TService, TEntity>(Mock<IAppLogger<TService>> logger, ServiceResponse<bool> result, IDictionary<string, string[]> errors) where TEntity : BaseEntity
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<Error>();
        result.Error.Code.Should().BeOfType<string>();
        logger.Verify(x => x.LogWarning(ServiceMessages.EntityValidationFailure<TEntity>(errors)), Times.Once);
    }
    

}