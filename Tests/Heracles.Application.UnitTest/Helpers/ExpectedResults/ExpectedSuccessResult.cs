using FluentAssertions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Abstractions.Responses;

namespace Heracles.Application.UnitTest.Helpers.ExpectedResults;

/// <summary>
/// Contains methods to validate the expected query results.
/// </summary>
public static class ExpectedQueryResult
{
    /// <summary>
    /// Checks if the success result matches the expected result.
    /// </summary>
    /// <typeparam name="T">The type of the objects in the success result.</typeparam>
    /// <param name="result">The success result to check.</param>
    /// <param name="expected">The expected objects.</param>
    public static void Success<T>(DomainResponse<QueryResponse<T>> result, IEnumerable<T> expected) where T : BaseEntity
    {
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.Value.Data.Should().BeEquivalentTo(expected);
    }
}