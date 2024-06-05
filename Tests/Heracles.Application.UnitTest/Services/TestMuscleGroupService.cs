using Heracles.Application.Features.MuscleGroups;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
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
    ///     Test case for the GetAllPagedAsync method of the MuscleGroupService.
    /// </summary>
    /// <param name="query">The query request.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ShouldReturnFilteredAndSortedMuscleGroups(QueryRequestDto query)
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
    ///     Test case for the GetEntityByIdAsync method of the MuscleGroupService.
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
        if (expected == TestServiceResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, muscleGroup!, id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedGetByIdResult.BadRequest(_logger, result, EntityErrorMessage<MuscleGroup>.BadRequest());
        else if (expected == TestServiceResponse.NotFound)
            ExpectedGetByIdResult.NotFound(_logger, result, EntityErrorMessage<MuscleGroup>.NotFound(id), id);
    }

    /// <summary>
    ///     Test case for the CreateEntityAsync method of the MuscleGroupService.
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
        if (expected == TestServiceResponse.Success)
            ExpectedCreateResult.Success<MuscleGroupService, MuscleGroup>(_logger, result, muscleGroup.Id,
                muscleGroup.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedCreateResult.BadRequest<MuscleGroupService, MuscleGroup>(_logger, result, result.Error.Errors!);
    }

    /// <summary>
    ///     Test case for the UpdateEntityAsync method of the MuscleGroupService.
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
        if (expected == TestServiceResponse.Success)
            ExpectedUpdateResult.Success<MuscleGroupService, MuscleGroup>(_logger, result, muscleGroup.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<MuscleGroupService, MuscleGroup>(_logger, result,
                result.Error.Errors!);
    }

    /// <summary>
    ///  Test case for the DeleteEntityAsync method of the MuscleGroupService.
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
        if (expected == TestServiceResponse.Success)
            ExpectedDeleteResult.Success<MuscleGroupService, MuscleGroup>(_logger, result, muscleGroup!.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedDeleteResult.BadRequest<MuscleGroupService, MuscleGroup>(_logger, result,
                EntityErrorMessage<MuscleGroup>.BadRequest());
        else if (expected == TestServiceResponse.NotFound)
            ExpectedDeleteResult.NotFound<MuscleGroupService, MuscleGroup>(_logger, result,
                EntityErrorMessage<MuscleGroup>.NotFound(id), id);
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a test class, so we need to test null values

    /// <summary>
    ///     Provides data for the CreateEntityAsync test case.
    /// </summary>
    public static TheoryData<string, string> CreateMuscleGroupsData()
    {
        return new TheoryData<string, string>
        {
            { "Calfs", TestServiceResponse.Success }, // Valid MuscleGroup
            { null, TestServiceResponse.BadRequest }, // Null name
            { "a", TestServiceResponse.BadRequest }, // To Short name
            { new string('a', 101), TestServiceResponse.BadRequest }, // To Long name
            { "Chest", TestServiceResponse.BadRequest } // Duplicate name
        };
    }

    /// <summary>
    ///     Provides data for the UpdateEntityAsync test case.
    /// </summary>
    public static TheoryData<int, string, string> UpdateMuscleGroupData()
    {
        return new TheoryData<int, string, string>
        {
            { 1, "Updated Name", TestServiceResponse.Success }, // Valid MuscleGroup
            { 0, "Calf", TestServiceResponse.BadRequest }, // Null id
            { 1, null, TestServiceResponse.BadRequest }, // Null name
            { 1, "a", TestServiceResponse.BadRequest }, // To Short name
            { 1, new string('a', 101), TestServiceResponse.BadRequest }, // To Long name
            { 1, "Chest", TestServiceResponse.BadRequest }, // Duplicate name
            { 100, "Chest", TestServiceResponse.BadRequest } // Id out of range
        };

    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

}