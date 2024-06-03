using FluentAssertions;
using Heracles.Application.Helpers;
using Heracles.Domain.Abstractions.Entities;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Responses;
using Moq;

namespace Heracles.Application.UnitTest.Helpers.ExpectedResults;

/// <summary>
/// Contains methods for asserting the expected results of creating entities.
/// </summary>
public static class ExpectedCreateResult
{
    /// <summary>
    /// Creates a success domainResponse for a create operation.
    /// </summary>
    /// <typeparam name="TService">The type of the service.</typeparam>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <param name="logger">The logger instance.</param>
    /// <param name="result">The domainResponse object.</param>
    /// <param name="expected">The expected entity.</param>
    /// <param name="id">The created entity's ID.</param>
    /// <exception cref="ArgumentNullException">Thrown when the logger or result is null.</exception>
    public static void Success<TService, TEntity>(Mock<IAppLogger<TService>> logger, DomainResponse<int> result, int expected, int id) where TEntity : BaseEntity
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(expected);
        logger.Verify(x => x.LogInformation(ServiceMessages.EntityCreated<TEntity>(id)), Times.Once);
    }

    /// <summary>
    /// Creates a bad request domainResponse with validation errors.
    /// </summary>
    /// <typeparam name="TService">The type of service.</typeparam>
    /// <typeparam name="TEntity">The type of entity.</typeparam>
    /// <param name="logger">The logger to log warnings.</param>
    /// <param name="result">The domainResponse result.</param>
    /// <param name="errors">The validation errors.</param>
    /// <exception cref="InvalidOperationException">Thrown when trying to access the value of a failure domainResponse.</exception>
    public static void BadRequest<TService, TEntity>(Mock<IAppLogger<TService>> logger, DomainResponse<int> result, IDictionary<string, string[]> errors) where TEntity : BaseEntity
    {
        result.IsFailure.Should().BeTrue();
        result.Error.Should().BeOfType<Error>();
        result.Error.Code.Should().BeOfType<string>();
        logger.Verify(x => x.LogWarning(ServiceMessages.EntityValidationFailure<TEntity>(errors)), Times.Once);
    }
}