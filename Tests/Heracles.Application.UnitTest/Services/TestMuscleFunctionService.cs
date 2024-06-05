using Heracles.Application.Features.MuscleFunctions;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.MuscleFunctions.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;
using Moq;
using Xunit.Abstractions;

namespace Heracles.Application.UnitTest.Services;

/// <summary>
///     This class contains unit tests for the MuscleFunctionService.
/// </summary>
public class TestMuscleFunctionService : BaseUnitTest
{
    private readonly Mock<IAppLogger<MuscleFunctionService>> _logger;
    private readonly MuscleFunctionService _service;

    /// <summary>
    ///     Constructor for the TestMuscleFunctionService class.
    /// </summary>
    public TestMuscleFunctionService(ITestOutputHelper testConsole) : base(testConsole)
    {
        _logger = new Mock<IAppLogger<MuscleFunctionService>>();
        _service = new MuscleFunctionService(_logger.Object, MuscleFunctionRepository.Object);
    }

    /// <summary>
    ///     Test case for the GetAllPagedAsync method of the MuscleFunctionService.
    /// </summary>
    /// <param name="query">The query request.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ShouldReturnFilteredAndSortedMuscleFunctions(QueryRequestDto query)
    {
        // Arrange
        SearchTerm = "Stabilizer";
        var expected = MuscleFunctionFixture.GetQueryRequest(query);

        // Act
        var result = await _service.GetAsync(query);

        // Assert
        ExpectedQueryResult.Success(result, expected);
    }

    /// <summary>
    ///     Test case for the GetEntityByIdAsync method of the MuscleFunctionService.
    /// </summary>
    /// <param name="id">The id of the muscle function.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(GetByIdAsyncData))]
    public async Task GetByIdAsync_ReturnsSuccessResponse(int id, string expected)
    {
        // Arrange
        var muscleFunction = MuscleFunctions.FirstOrDefault(e => e.Id == id);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, muscleFunction!, id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedGetByIdResult.BadRequest(_logger, result, EntityErrorMessage<MuscleFunction>.BadRequest());
        else if (expected == TestServiceResponse.NotFound)
            ExpectedGetByIdResult.NotFound(_logger, result, EntityErrorMessage<MuscleFunction>.NotFound(id), id);
    }

    /// <summary>
    ///     Test case for the CreateEntityAsync method of the MuscleFunctionService.
    /// </summary>
    /// <param name="name">The name of the muscle function.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(CreateMuscleFunctionsData))]
    public async Task CreateAsync_ReturnsResponse(string name, string expected)
    {
        // Arrange
        var muscleFunction = new MuscleFunction
        {
            Name = name
        };

        // Act
        var result = await _service.CreateAsync(muscleFunction);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedCreateResult.Success<MuscleFunctionService, MuscleFunction>(_logger, result, muscleFunction.Id,
                muscleFunction.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedCreateResult.BadRequest<MuscleFunctionService, MuscleFunction>(_logger, result,
                result.Error.Errors!);
    }

    /// <summary>
    ///     Test case for the UpdateEntityAsync method of the MuscleFunctionService.
    /// </summary>
    /// <param name="id">The id of the muscle function.</param>
    /// <param name="name">The new name of the muscle function.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(UpdateMuscleFunctionsData))]
    public async Task UpdateAsync_ReturnsResponse(int id, string name, string expected)
    {
        // Arrange
        var muscleFunction = MuscleFunctions.FirstOrDefault(e => e.Id == id)
                             ?? new MuscleFunction { Id = id, Name = name };
        muscleFunction.Name = name;

        // Act
        var result = await _service.UpdateAsync(muscleFunction);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedUpdateResult.Success<MuscleFunctionService, MuscleFunction>(_logger, result, muscleFunction.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<MuscleFunctionService, MuscleFunction>(_logger, result,
                result.Error.Errors!);
    }

    /// <summary>
    ///     Test case for the DeleteEntityAsync method of the MuscleFunctionService.
    /// </summary>
    /// <param name="id">The id of the muscle function.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(DeleteAsyncData))]
    public async Task DeleteAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var muscleFunction = MuscleFunctions.FirstOrDefault(e => e.Id == id);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedDeleteResult.Success<MuscleFunctionService, MuscleFunction>(_logger, result, muscleFunction!.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedDeleteResult.BadRequest<MuscleFunctionService, MuscleFunction>(_logger, result,
                EntityErrorMessage<MuscleFunction>.BadRequest());
        else if (expected == TestServiceResponse.NotFound)
            ExpectedDeleteResult.NotFound<MuscleFunctionService, MuscleFunction>(_logger, result,
                EntityErrorMessage<MuscleFunction>.NotFound(id), id);
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a test class, so we need to test null values

    /// <summary>
    ///     Provides data for the CreateEntityAsync test case.
    /// </summary>
    public static TheoryData<string, string> CreateMuscleFunctionsData()
    {
        return new TheoryData<string, string>
        {
            { NotADuplicateName, TestServiceResponse.Success }, // Valid name
            { null, TestServiceResponse.BadRequest }, // Null name
            { "a", TestServiceResponse.BadRequest }, // To Short name
            { new string('a', 101), TestServiceResponse.BadRequest }, // To Long name
            { "Stabilizer", TestServiceResponse.BadRequest } // Duplicate name 
        };
    }

    /// <summary>
    ///     Provides data for the UpdateEntityAsync test case.
    /// </summary>
    public static TheoryData<int, string, string> UpdateMuscleFunctionsData()
    {
        return new TheoryData<int, string, string>
        {
            { 1, NotADuplicateName, TestServiceResponse.Success }, // Valid MuscleFunction
            { 0, NotADuplicateName, TestServiceResponse.BadRequest }, // Null id
            { 1, null, TestServiceResponse.BadRequest }, // Null name
            { 1, "a", TestServiceResponse.BadRequest }, // To Short name
            { 1, new string('a', 101), TestServiceResponse.BadRequest }, // To Long name
            { 1, "Stabilizer", TestServiceResponse.BadRequest }, // Duplicate name 
            { 100, "Synergist", TestServiceResponse.BadRequest } // Id out of range
        };
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

}