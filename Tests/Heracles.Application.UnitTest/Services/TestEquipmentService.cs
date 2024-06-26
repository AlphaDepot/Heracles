using Heracles.Application.Features.Equipments;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.Equipments.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;
using Moq;
using Xunit.Abstractions;

namespace Heracles.Application.UnitTest.Services;
/// <summary>
/// This class is used to test the EquipmentService.
/// </summary>
public class TestEquipmentService : BaseUnitTest
{


    private readonly Mock<IAppLogger<EquipmentService>> _logger;
    private readonly EquipmentService _service;


    /// <summary>
    /// Constructor for the TestEquipmentService class.
    /// </summary>
    public TestEquipmentService(ITestOutputHelper testConsole) : base(testConsole)
    {
        _logger = new Mock<IAppLogger<EquipmentService>>();
        _service = new EquipmentService(_logger.Object, EquipmentRepository.Object);
    }

    /// <summary>
    /// Test for the GetAllPagedAsync method of the EquipmentService.
    /// </summary>
    /// <param name="query">The query to filter and sort equipments.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_WithFilterAndSort_ShouldReturnFilteredAndSortedEquipments(QueryRequestDto query)
    {
        // Arrange
        var expected = EquipmentFixture.Query(query);

        // Act
        var result = await _service.GetAsync(query);

        // Assert
        ExpectedQueryResult.Success(result, expected);
    }

    /// <summary>
    /// Test for the GetEntityByIdAsync method of the EquipmentService.
    /// </summary>
    /// <param name="id">The id of the equipment to get.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(GetByIdAsyncData))]
    public async Task GetByIdAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var expectedEquipment = Equipments.FirstOrDefault(q => q.Id == id);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, expectedEquipment!, id);
        else if (expected == TestServiceResponse.NotFound)
            ExpectedGetByIdResult.NotFound(_logger, result, EntityErrorMessage<Equipment>.NotFound(id), id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedGetByIdResult.BadRequest(_logger, result, EntityErrorMessage<Equipment>.BadRequest());
    }

    /// <summary>
    /// Test for the CreateEntityAsync method of the EquipmentService.
    /// </summary>
    /// <param name="type">The type of the equipment to create.</param>
    /// <param name="weight">The weight of the equipment to create.</param>
    /// <param name="resistance">The resistance of the equipment to create.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(CreateData))]
    public async Task CreateAsync_ReturnsResponse(string type, double weight, double resistance, string expected)
    {
        // Arrange
        var equipment = new Equipment
        {
            Type = type,
            Weight = weight,
            Resistance = resistance
        };

        // Act
        var result = await _service.CreateAsync(equipment);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedCreateResult.Success<EquipmentService, Equipment>(_logger, result, equipment.Id, result.Value);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedCreateResult.BadRequest<EquipmentService, Equipment>(_logger, result, result.Error.Errors!);
    }
    /// <summary>
    /// Test for the UpdateEntityAsync method of the EquipmentService.
    /// </summary>
    /// <param name="id">The id of the equipment to update.</param>
    /// <param name="type">The new type of the equipment.</param>
    /// <param name="weight">The new weight of the equipment.</param>
    /// <param name="resistance">The new resistance of the equipment.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(UpdateData))]
    public async Task UpdateAsync_ReturnsResponse(int id, string type, double weight, double resistance,
        string expected)
    {
        // Arrange
        var equipment = Equipments.FirstOrDefault(q => q.Id == id)
                        ?? new Equipment { Id = id, Type = type, Weight = weight, Resistance = resistance };
        equipment.Type = type;
        equipment.Weight = weight;
        equipment.Resistance = resistance;

        // Act
        var result = await _service.UpdateAsync(equipment);

        // Assert
        if (expected == TestServiceResponse.Success)
           ExpectedUpdateResult.Success<EquipmentService, Equipment>(_logger, result, equipment.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<EquipmentService, Equipment>(_logger, result, result.Error.Errors!);
    }

    /// <summary>
    /// Test for the DeleteEntityAsync method of the EquipmentService.
    /// </summary>
    /// <param name="id">The id of the equipment to delete.</param>
    /// <param name="expected">The expected result.</param>
    [Theory]
    [MemberData(nameof(GetByIdAsyncData))]
    public async Task DeleteAsync_ReturnsResponse(int id, string expected)
    {
        // Arrange
        var expectedEquipment = Equipments.FirstOrDefault(q => q.Id == id);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedDeleteResult.Success<EquipmentService, Equipment>(_logger, result, id);
        else if (expected == TestServiceResponse.NotFound)
            ExpectedDeleteResult.NotFound<EquipmentService, Equipment>(_logger, result,
                EntityErrorMessage<Equipment>.NotFound(id), id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedDeleteResult.BadRequest<EquipmentService, Equipment>(_logger, result,
                EntityErrorMessage<Equipment>.BadRequest());
    }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a test class, so we need to test null values

    /// <summary>
    /// Provides data for the CreateEntityAsync test.
    /// </summary>
    public static TheoryData<string, double, double, string> CreateData()
    {
        return new TheoryData<string, double, double, string>
        {
            { "NOT A DUPLICATE", 10, 10, TestServiceResponse.Success },
            { "Dumbbell", 10, 10, TestServiceResponse.BadRequest }, // duplicate
            { "", 20, 10, TestServiceResponse.BadRequest }, // invalid type to short
            { null, 20, 10, TestServiceResponse.BadRequest }, // invalid type null
            { "Dumbbell", -1, 10, TestServiceResponse.BadRequest }, // invalid weight
            { "Dumbbell", 20, -1, TestServiceResponse.BadRequest } // invalid resistance
        };
    }

    /// <summary>
    /// Provides data for the UpdateEntityAsync test.
    /// </summary>
    public static TheoryData<int, string, double, double, string> UpdateData()
    {
        return new TheoryData<int, string, double, double, string>
        {
            { 1, "NOT A DUPLICATE", 10, 10, TestServiceResponse.Success },
            { 0, "NOT A DUPLICATE", 10, 10, TestServiceResponse.BadRequest }, // invalid id
            { -1, "NOT A DUPLICATE", 10, 10, TestServiceResponse.BadRequest }, // invalid id
            { 1000000, "NOT A DUPLICATE", 10, 10, TestServiceResponse.BadRequest }, // out of bound id
            { 1, "Dumbbell", 10, 10, TestServiceResponse.BadRequest }, // duplicate
            { 1, "", 20, 10, TestServiceResponse.BadRequest }, // invalid type to short
            { 1, null, 20, 10, TestServiceResponse.BadRequest }, // invalid type null
            { 1, "Dumbbell", -1, 10, TestServiceResponse.BadRequest }, // invalid weight
            { 1, "Dumbbell", 20, -1, TestServiceResponse.BadRequest } // invalid resistance
        };
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

}