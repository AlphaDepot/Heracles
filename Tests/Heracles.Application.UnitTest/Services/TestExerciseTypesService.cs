using System.Text.Json;
using Heracles.Application.Features.ExerciseTypes;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.ExercisesType.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;
using Moq;
using Xunit.Abstractions;

namespace Heracles.Application.UnitTest.Services;

/// <summary>
///     This class provides unit testing for the TestExerciseTypesService class.
/// </summary>
public class TestExerciseTypesService : BaseUnitTest
{
    
    private readonly Mock<IAppLogger<ExerciseTypeService>> _logger;
    private readonly ExerciseTypeService _service;
    
    /// <summary>
    ///     Constructor for the TestExerciseTypesService class.
    /// </summary>
    public TestExerciseTypesService(ITestOutputHelper testConsole) : base(testConsole)
    {
        _logger = new Mock<IAppLogger<ExerciseTypeService>>();
        _service = new ExerciseTypeService(_logger.Object, ExerciseTypeRepository.Object);
    }

    /// <summary>
    ///     Test for the GetAllPagedAsync method of the ExerciseTypeService.
    /// </summary>
    /// <param name="query">The query to be used for the test.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedExerciseTypes(QueryRequestDto query)
    {
        // Arrange
        SearchTerm = "Bench Press";
        var expected = ExerciseTypeFixture.Query(query);

        // Act
        var result = await _service.GetAsync(query);

        // Assert
        ExpectedQueryResult.Success(result, expected);
    }

    /// <summary>
    ///     Test for the GetEntityByIdAsync method of the ExerciseTypeService.
    /// </summary>
    /// <param name="id">The id to be used for the test.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(GetByIdAsyncData))]
    public async Task GetByIdAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var exerciseType = ExerciseTypes.FirstOrDefault(x => x.Id == id);

        // Act
        var result = await _service.GetByIdAsync(id);


        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, exerciseType!, id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedGetByIdResult.BadRequest(_logger, result, EntityErrorMessage<ExerciseType>.BadRequest());
        else if (expected == TestServiceResponse.NotFound)
            ExpectedGetByIdResult.NotFound(_logger, result, EntityErrorMessage<ExerciseType>.NotFound(id), id);
    }

    /// <summary>
    ///     Test for the CreateEntityAsync method of the ExerciseTypeService.
    /// </summary>
    /// <param name="name">The name to be used for the test.</param>
    /// <param name="description">The description to be used for the test.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(CreateData))]
    public async Task CreateAsync_ReturnsResponse(string name, string description, string expected)
    {
        // Arrange
        var exerciseType = new ExerciseType
        {
            Name = name,
            Description = description
        };

        // Act
        var result = await _service.CreateAsync(exerciseType);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedCreateResult.Success<ExerciseTypeService, ExerciseType>(_logger, result, exerciseType.Id,
                result.Value);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedCreateResult.BadRequest<ExerciseTypeService, ExerciseType>(_logger, result, result.Error.Errors!);
    }

    /// <summary>
    ///     Test for the UpdateEntityAsync method of the ExerciseTypeService.
    /// </summary>
    /// <param name="id">The id to be used for the test.</param>
    /// <param name="name">The name to be used for the test.</param>
    /// <param name="description">The description to be used for the test.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(UpdateData))]
    public async Task UpdateAsync_ReturnsResponse(int id, string name, string description, string expected)
    {
        // Arrange
        // Query the exercise type with the id or create a new one if it doesn't exist to avoid null reference exception when testing invalid or out of range ids
        var exerciseType = ExerciseTypes.FirstOrDefault(x => x.Id == id)
                           ?? new ExerciseType { Id = id, Name = name, Description = description };
        exerciseType.Name = name;
        exerciseType.Description = description;


        // Act
        var result = await _service.UpdateAsync(exerciseType);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedUpdateResult.Success<ExerciseTypeService, ExerciseType>(_logger, result, exerciseType.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<ExerciseTypeService, ExerciseType>(_logger, result, result.Error.Errors!);
    }

    /// <summary>
    ///     Test for the DeleteEntityAsync method of the ExerciseTypeService.
    /// </summary>
    /// <param name="id">The id to be used for the test.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(DeleteAsyncData))]
    public async Task DeleteAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedDeleteResult.Success<ExerciseTypeService, ExerciseType>(_logger, result, id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedDeleteResult.BadRequest<ExerciseTypeService, ExerciseType>(_logger, result,
                EntityErrorMessage<ExerciseType>.BadRequest());
        else if (expected == TestServiceResponse.NotFound)
            ExpectedDeleteResult.NotFound<ExerciseTypeService, ExerciseType>(_logger, result,
                EntityErrorMessage<ExerciseType>.NotFound(id), id);
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a test class, so we need to test null values

    /// <summary>
    ///     Provides data for the CreateData test.
    /// </summary>
    public static TheoryData<string, string, string> CreateData()
    {
        return new TheoryData<string, string, string>
        {
            { "New Exercise Type", "New Exercise Type Description", TestServiceResponse.Success },
            { "Bench Press", "New Exercise Type Description", TestServiceResponse.BadRequest },
            { null, null, TestServiceResponse.BadRequest },
            { "name", null, TestServiceResponse.BadRequest },
            { null, "description", TestServiceResponse.BadRequest },
            { new string('a', 256), "description", TestServiceResponse.BadRequest },
            { "name", new string('a', 1001), TestServiceResponse.BadRequest }
        };
    }

    /// <summary>
    ///     Provides data for the UpdateData test.
    /// </summary>
    public static TheoryData<int, string, string, string> UpdateData()
    {
        return new TheoryData<int, string, string, string>
        {
            { 1, "Updated Name", "Updated Description", TestServiceResponse.Success },
            { 100, "name", "description", TestServiceResponse.BadRequest }, // id out of range
            { 0, null, null, TestServiceResponse.BadRequest }, // all null
            { 1, "name", null, TestServiceResponse.BadRequest }, // null description
            { 1, null, "description", TestServiceResponse.BadRequest }, // null name
            { 1, new string('a', 256), "description", TestServiceResponse.BadRequest }, // to long name
            { 1, "name", new string('a', 1001), TestServiceResponse.BadRequest } // to long description
        };
    }

#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

}