using System.Text.Json;
using Heracles.Application.Features.ExerciseTypes;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Queries;
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
    ///     Test for the GetAsync method of the ExerciseTypeService.
    /// </summary>
    /// <param name="query">The query to be used for the test.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ReturnsFilteredAndSortedExerciseTypes(QueryRequest query)
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
    ///     Test for the GetByIdAsync method of the ExerciseTypeService.
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
        if (expected == TestDomainResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, exerciseType!, id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedGetByIdResult.BadRequest(_logger, result, EntityErrorMessage<ExerciseType>.BadRequest());
        else if (expected == TestDomainResponse.NotFound)
            ExpectedGetByIdResult.NotFound(_logger, result, EntityErrorMessage<ExerciseType>.NotFound(id), id);
    }

    /// <summary>
    ///     Test for the CreateAsync method of the ExerciseTypeService.
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
        if (expected == TestDomainResponse.Success)
            ExpectedCreateResult.Success<ExerciseTypeService, ExerciseType>(_logger, result, exerciseType.Id,
                result.Value);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedCreateResult.BadRequest<ExerciseTypeService, ExerciseType>(_logger, result, result.Error.Errors!);
    }

    /// <summary>
    ///     Test for the UpdateAsync method of the ExerciseTypeService.
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
        if (expected == TestDomainResponse.Success)
            ExpectedUpdateResult.Success<ExerciseTypeService, ExerciseType>(_logger, result, exerciseType.Id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<ExerciseTypeService, ExerciseType>(_logger, result, result.Error.Errors!);
    }

    /// <summary>
    ///     Test for the DeleteAsync method of the ExerciseTypeService.
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
        if (expected == TestDomainResponse.Success)
            ExpectedDeleteResult.Success<ExerciseTypeService, ExerciseType>(_logger, result, id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedDeleteResult.BadRequest<ExerciseTypeService, ExerciseType>(_logger, result,
                EntityErrorMessage<ExerciseType>.BadRequest());
        else if (expected == TestDomainResponse.NotFound)
            ExpectedDeleteResult.NotFound<ExerciseTypeService, ExerciseType>(_logger, result,
                EntityErrorMessage<ExerciseType>.NotFound(id), id);
    }

    
    /// <summary>
    ///     Provides data for the CreateData test.
    /// </summary>
    public static IEnumerable<object?[]> CreateData()
    {
        yield return new object[] { "New Exercise Type", "New Exercise Type Description", TestDomainResponse.Success };

        yield return new object[] { "Bench Press", "New Exercise Type Description", TestDomainResponse.BadRequest };
        yield return new object?[] { null, null, TestDomainResponse.BadRequest };
        yield return new object?[] { "name", null, TestDomainResponse.BadRequest };
        yield return new object?[] { null, "description", TestDomainResponse.BadRequest };
        yield return new object[] { new string('a', 256), "description", TestDomainResponse.BadRequest };
        yield return new object[] { "name", new string('a', 1001), TestDomainResponse.BadRequest };
    }

    /// <summary>
    ///     Provides data for the UpdateData test.
    /// </summary>
    public static IEnumerable<object?[]> UpdateData()
    {
        yield return new object[] { 1, "Updated Name", "Updated Description", TestDomainResponse.Success };

        yield return new object[] { 100, "name", "description", TestDomainResponse.BadRequest }; // id out of range
        yield return new object?[] { null, null, null, TestDomainResponse.BadRequest }; // all null
        yield return new object?[] { 1, "name", null, TestDomainResponse.BadRequest }; // null description
        yield return new object?[] { 1, null, "description", TestDomainResponse.BadRequest }; // null name
        yield return new object[] { 1, new string('a', 256), "description", TestDomainResponse.BadRequest }; // to long name
        yield return new object[] { 1, "name", new string('a', 1001), TestDomainResponse.BadRequest }; // to long description
    }
}