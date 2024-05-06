using Heracles.Application.Features.MuscleGroups;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.MuscleGroups.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;
using Moq;
using Xunit.Abstractions;

namespace Heracles.Application.UnitTest.Services;

/// <summary>
///     This class contains unit tests for the MuscleGroupService.
/// </summary>
public class TestMuscleGroupService : BaseUnitTest
{
    private readonly Mock<IAppLogger<MuscleGroupService>> _logger;
    private readonly MuscleGroupService _service;
    
    /// <summary>
    ///     Constructor for the TestMuscleGroupService class.
    /// </summary>
    public TestMuscleGroupService(ITestOutputHelper testConsole) : base(testConsole)
    {
        _logger = new Mock<IAppLogger<MuscleGroupService>>();
        _service = new MuscleGroupService(_logger.Object, MuscleGroupRepository.Object);
        
    }

    /// <summary>
    ///     Test case for the GetAsync method of the MuscleGroupService.
    /// </summary>
    /// <param name="query">The query request.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ShouldReturnFilteredAndSortedMuscleGroups(QueryRequest query)
    {
        // Arrange
        SearchTerm = "Chest";
        var expected = MuscleGroupFixture.GetQueryRequest(query);

        // Act
        var result = await _service.GetAsync(query);

        // Assert
        ExpectedQueryResult.Success(result, expected);
    }

    /// <summary>
    ///     Test case for the GetByIdAsync method of the MuscleGroupService.
    /// </summary>
    /// <param name="id">The id of the muscle group.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(GetByIdAsyncData))]
    public async Task GetByIdAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var muscleGroup = MuscleGroups.FirstOrDefault(e => e.Id == id);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, muscleGroup!, id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedGetByIdResult.BadRequest(_logger, result, EntityErrorMessage<MuscleGroup>.BadRequest());
        else if (expected == TestDomainResponse.NotFound)
            ExpectedGetByIdResult.NotFound(_logger, result, EntityErrorMessage<MuscleGroup>.NotFound(id), id);
    }

    /// <summary>
    ///     Test case for the CreateAsync method of the MuscleGroupService.
    /// </summary>
    /// <param name="name">The name of the muscle group.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(CreateMuscleGroupsData))]
    public async Task CreateAsync_ReturnsResponse(string name, string expected)
    {
        // Arrange
        var muscleGroup = new MuscleGroup
        {
            Name = name
        };

        // Act
        var result = await _service.CreateAsync(muscleGroup);
        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedCreateResult.Success<MuscleGroupService, MuscleGroup>(_logger, result, muscleGroup.Id,
                muscleGroup.Id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedCreateResult.BadRequest<MuscleGroupService, MuscleGroup>(_logger, result, result.Error.Errors!);
    }

    /// <summary>
    ///     Test case for the UpdateAsync method of the MuscleGroupService.
    /// </summary>
    /// <param name="id">The id of the muscle group.</param>
    /// <param name="name">The new name of the muscle group.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(UpdateMuscleGroupData))]
    public async Task UpdateAsync_ReturnsResponse(int id, string name, string expected)
    {
        // Arrange
        // Query the muscle group with the id or create a new one if it doesn't exist to avoid null reference exception when testing invalid or out of range ids
        var muscleGroup = MuscleGroups.FirstOrDefault(e => e.Id == id) ?? new MuscleGroup { Id = id, Name = name };
        muscleGroup.Name = name;

        // Act
        var result = await _service.UpdateAsync(muscleGroup);


        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedUpdateResult.Success<MuscleGroupService, MuscleGroup>(_logger, result, muscleGroup.Id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<MuscleGroupService, MuscleGroup>(_logger, result,
                result.Error.Errors!);
    }

    /// <summary>
    ///     Test case for the DeleteAsync method of the MuscleGroupService.
    /// </summary>
    /// <param name="id">The id of the muscle group.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(DeleteAsyncData))]
    public async Task DeleteAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var muscleGroup = MuscleGroups.FirstOrDefault(e => e.Id == id);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedDeleteResult.Success<MuscleGroupService, MuscleGroup>(_logger, result, muscleGroup!.Id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedDeleteResult.BadRequest<MuscleGroupService, MuscleGroup>(_logger, result,
                EntityErrorMessage<MuscleGroup>.BadRequest());
        else if (expected == TestDomainResponse.NotFound)
            ExpectedDeleteResult.NotFound<MuscleGroupService, MuscleGroup>(_logger, result,
                EntityErrorMessage<MuscleGroup>.NotFound(id), id);
    }
    

    /// <summary>
    ///     Provides data for the CreateAsync test case.
    /// </summary>
    public static IEnumerable<object?[]> CreateMuscleGroupsData()
    {
        yield return new object[] { "Calfs", TestDomainResponse.Success }; // Valid MuscleGroup
        yield return new object?[] { null, TestDomainResponse.BadRequest }; // Null name
        yield return new object?[] { "a", TestDomainResponse.BadRequest }; // To Short name
        yield return new object?[] { new string('a', 101), TestDomainResponse.BadRequest }; // To Long name
        yield return new object?[] { "Chest", TestDomainResponse.BadRequest }; // Duplicate name
    }

    /// <summary>
    ///     Provides data for the UpdateAsync test case.
    /// </summary>
    public static IEnumerable<object?[]> UpdateMuscleGroupData()
    {
        yield return new object[] { 1, "Updated Name", TestDomainResponse.Success }; // Valid MuscleGroup

        yield return new object[] { 0, "Calf", TestDomainResponse.BadRequest }; // Null id
        yield return new object?[] { 1, null, TestDomainResponse.BadRequest }; // Null name
        yield return new object[] { 1, "a", TestDomainResponse.BadRequest }; // To Short name
        yield return new object[] { 1, new string('a', 101), TestDomainResponse.BadRequest }; // To Long name
        yield return new object[] { 1, "Chest", TestDomainResponse.BadRequest }; // Duplicate name
        yield return new object[] { 100, "Chest", TestDomainResponse.BadRequest }; // Id out of range
    }
}