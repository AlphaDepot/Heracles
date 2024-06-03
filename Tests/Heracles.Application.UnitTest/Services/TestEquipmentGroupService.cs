using System.Text.Json;
using Heracles.Application.Features.EquipmentGroups;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.EquipmentGroups.DTOs;
using Heracles.Domain.EquipmentGroups.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;
using Moq;

using Xunit.Abstractions;

namespace Heracles.Application.UnitTest.Services;
/// <summary>
/// Provides data for the UpdateAsync test.
/// </summary>
public class TestEquipmentGroupService : BaseUnitTest
{
    private readonly Mock<IAppLogger<EquipmentGroupService>> _logger;
    private readonly EquipmentGroupService _service;
   

    /// <summary>
    /// Constructor for the TestEquipmentGroupService class.
    /// </summary>
    public TestEquipmentGroupService(ITestOutputHelper testConsole) : base(testConsole)
    {
        _logger = new Mock<IAppLogger<EquipmentGroupService>>();
        _service = new EquipmentGroupService(_logger.Object, EquipmentGroupRepository.Object, EquipmentRepository.Object);
    }

    /// <summary>
    /// Test for the GetAsync method with filter and sort parameters.
    /// </summary>
    /// <param name="query">The query parameters for filtering and sorting.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ShouldReturnFilteredAndSortedEquipmentGroups(QueryRequestDto query)
    {
        // Arrange
        SearchTerm = "Home Gym";
        var expected = EquipmentGroupFixture.Query(query);

        // Act
        var result = await _service.GetAsync(query);

        // Assert
        ExpectedQueryResult.Success(result, expected);
    }
    
    /// <summary>
    /// Test for the GetByIdAsync method.
    /// </summary>
    /// <param name="id">The id of the equipment group to retrieve.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(GetByIdAsyncData))]
    public async Task GetByIdAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var expectedEquipmentGroup = EquipmentGroups.FirstOrDefault(q => q.Id == id);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        if (expected == TestDomainResponse.Success)

        {
            TestConsole.WriteLine($"Expected: {JsonSerializer.Serialize(expectedEquipmentGroup)}");
            TestConsole.WriteLine($"Result: {result.Error}");
            ExpectedGetByIdResult.Success(_logger, result, expectedEquipmentGroup!, id);
        }
            
        else if (expected == TestDomainResponse.NotFound)
            ExpectedGetByIdResult.NotFound(_logger, result, EntityErrorMessage<EquipmentGroup>.NotFound(id), id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedGetByIdResult.BadRequest(_logger, result, EntityErrorMessage<EquipmentGroup>.BadRequest());
    }

    /// <summary>
    /// Test for the CreateAsync method.
    /// </summary>
    /// <param name="name">The name of the equipment group to create.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(CreateAsyncData))]
    public async Task CreateAsync_ReturnsResponse(string? name, string expected)
    {
        // Arrange
        var equipmentGroup = new EquipmentGroup { Name = name ?? "" };

        // Act
        var result = await _service.CreateAsync(equipmentGroup);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedCreateResult.Success<EquipmentGroupService, EquipmentGroup>(_logger, result, equipmentGroup.Id,
                result.Value);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedCreateResult.BadRequest<EquipmentGroupService, EquipmentGroup>(_logger, result,
                result.Error.Errors!);
    }

    /// <summary>
    /// Test for the UpdateAsync method.
    /// </summary>
    /// <param name="id">The id of the equipment group to update.</param>
    /// <param name="name">The new name of the equipment group.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(UpdateAsyncData))]
    public async Task UpdateAsync_ReturnsResponse(int id, string name, string expected)
    {
        // Arrange
        var equipmentGroup = EquipmentGroups.FirstOrDefault(q => q.Id == id)
                             ?? new EquipmentGroup { Id = id, Name = name };

        equipmentGroup.Name = name ?? "";

        // Act
        var result = await _service.UpdateAsync(equipmentGroup);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedUpdateResult.Success<EquipmentGroupService, EquipmentGroup>(_logger, result, equipmentGroup.Id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<EquipmentGroupService, EquipmentGroup>(_logger, result,
                result.Error.Errors!);
    }

    /// <summary>
    /// Test for the DeleteAsync method.
    /// </summary>
    /// <param name="id">The id of the equipment group to delete.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(DeleteAsyncData))]
    public async Task DeleteAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var expectedEquipmentGroup = EquipmentGroups.FirstOrDefault(q => q.Id == id);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedDeleteResult.Success<EquipmentGroupService, EquipmentGroup>(_logger, result, id);
        else if (expected == TestDomainResponse.NotFound)
            ExpectedDeleteResult.NotFound<EquipmentGroupService, EquipmentGroup>(_logger, result,
                EntityErrorMessage<EquipmentGroup>.NotFound(id), id);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedDeleteResult.BadRequest<EquipmentGroupService, EquipmentGroup>(_logger, result,
                EntityErrorMessage<EquipmentGroup>.BadRequest());
    }

    /// <summary>
    /// Test for the AddEquipmentAsync method.
    /// </summary>
    /// <param name="equipmentGroupId">The id of the equipment group to add equipment to.</param>
    /// <param name="equipmentId">The id of the equipment to add.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(AddRemoveAsyncData))]
    public async Task AddEquipmentAsync_ReturnsResponse(int equipmentGroupId, int equipmentId, string expected)
    {
        // Arrange
        var entityDto = new AddRemoveEquipmentGroupDto
            { EquipmentGroupId = equipmentGroupId, EquipmentId = equipmentId };

        // Act
        var result = await _service.AddEquipmentAsync(entityDto);

        // Assert
        if (expected == TestDomainResponse.Success)
           ExpectedUpdateResult.Success<EquipmentGroupService, EquipmentGroup>(_logger, result, equipmentGroupId);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<EquipmentGroupService, EquipmentGroup>(_logger, result,
                result.Error.Errors!);
    }
    
    /// <summary>
    /// Test for the RemoveEquipmentAsync method.
    /// </summary>
    /// <param name="equipmentGroupId">The id of the equipment group to remove equipment from.</param>
    /// <param name="equipmentId">The id of the equipment to remove.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(AddRemoveAsyncData))]
    public async Task RemoveEquipmentAsync_ReturnsResponse(int equipmentGroupId, int equipmentId, string expected)
    {
        // Arrange
        var entityDto = new AddRemoveEquipmentGroupDto
            { EquipmentGroupId = equipmentGroupId, EquipmentId = equipmentId };

        // Act
        var result = await _service.RemoveEquipmentAsync(entityDto);

        // Assert
        if (expected == TestDomainResponse.Success)
            ExpectedUpdateResult.Success<EquipmentGroupService, EquipmentGroup>(_logger, result, equipmentGroupId);
        else if (expected == TestDomainResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<EquipmentGroupService, EquipmentGroup>(_logger, result,
                result.Error.Errors!);
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a test class, so we need to test null values

    /// <summary>
    ///  Test for the CreateAsync method.
    /// </summary>
    /// <returns> TheoryData</returns>
    public static TheoryData<string?, string> CreateAsyncData()
    {
        TheoryData<string?, string> theoryData = new()
        {
            { "NOT A DUPLICATE", TestDomainResponse.Success }, // valid entity
            { "Home Gym", TestDomainResponse.BadRequest }, // duplicate entity
            { "", TestDomainResponse.BadRequest }, // invalid entity
            { null, TestDomainResponse.BadRequest }, // invalid entity
            { new string('a', 256), TestDomainResponse.BadRequest }, // invalid entity
        };
        return theoryData;
    }

    /// <summary>
    ///  Test for the UpdateAsync method.
    /// </summary>
    /// <returns> TheoryData</returns>
    public static TheoryData<int, string, string> UpdateAsyncData()
    {
        return new TheoryData<int, string, string>()
        {
            { 1, "NOT A DUPLICATE", TestDomainResponse.Success }, // valid entity
            { 1, "Home Gym", TestDomainResponse.BadRequest }, // duplicate entity
            { 1, "", TestDomainResponse.BadRequest }, // invalid entity
            { 1, null, TestDomainResponse.BadRequest }, // invalid entity
            { 1, new string('a', 256), TestDomainResponse.BadRequest }, // invalid entity
            { 0, "Gym", TestDomainResponse.BadRequest }, // invalid id
        };
    }

    /// <summary>
    ///  Test for the AddEquipmentAsync and RemoveEquipmentAsync methods.
    /// </summary>
    /// <returns> TheoryData</returns>
    public static TheoryData<int, int, string> AddRemoveAsyncData()
    {
        return new TheoryData<int, int, string>()
        {
            { 1, 1, TestDomainResponse.Success }, // valid entity
            { 1, 1000000, TestDomainResponse.BadRequest }, // out of bound id
            { 1000000, 1, TestDomainResponse.BadRequest }, // out of bound id
            { 0, 1, TestDomainResponse.BadRequest }, // invalid id
            { 1, 0, TestDomainResponse.BadRequest }, // invalid id
            { 1, -1, TestDomainResponse.BadRequest }, // invalid id
        };
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

}