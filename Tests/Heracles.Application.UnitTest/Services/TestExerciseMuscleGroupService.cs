using System.Text.Json;
using Heracles.Application.Features.ExerciseMuscleGroups;
using Heracles.Application.UnitTest.Helpers.ExpectedResults;
using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Abstractions.Errors;
using Heracles.Domain.Abstractions.Logging;
using Heracles.Domain.ExerciseMuscleGroups.DTOs;
using Heracles.Domain.ExerciseMuscleGroups.Models;
using Heracles.TestUtilities.Fixtures;
using Heracles.TestUtilities.Helpers;
using Moq;
using Xunit.Abstractions;

namespace Heracles.Application.UnitTest.Services;

/// <summary>
///     This class is responsible for testing the ExerciseMuscleGroupService.
/// </summary>
public class TestExerciseMuscleGroupService : BaseUnitTest
{
    private readonly Mock<IAppLogger<ExerciseMuscleGroupService>> _logger;
    private readonly ExerciseMuscleGroupService _service;
    
    /// <summary>
    ///     Constructor for the TestExerciseMuscleGroupService class.
    /// </summary>
    public TestExerciseMuscleGroupService(ITestOutputHelper testConsole) : base(testConsole)
    {
        _logger = new Mock<IAppLogger<ExerciseMuscleGroupService>>();
        
        _service = new ExerciseMuscleGroupService(
            _logger.Object,
            ExerciseMuscleGroupRepository.Object,
            MuscleGroupRepository.Object,
            MuscleFunctionRepository.Object,
            ExerciseTypeRepository.Object
        );
    }

    /// <summary>
    ///     Test for the GetAllPagedAsync method of the ExerciseMuscleGroupService.
    /// </summary>
    /// <param name="query">The query to be used for the test.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetAsync_ReturnsExerciseMuscleGroups(QueryRequestDto query)
    {
        // Arrange
        SearchTerm = "Chest";
        var expected = ExerciseMuscleGroupFixture.Query(query);

        // Act
        var result = await _service.GetAsync(query);

        // Assert
        ExpectedQueryResult.Success(result, expected);
    }

    /// <summary>
    ///     Test for the GetByExerciseIdAsync method of the ExerciseMuscleGroupService.
    /// </summary>
    /// <param name="query">The query to be used for the test.</param>
    [Theory]
    [MemberData(nameof(QueryData))]
    public async Task GetByExerciseIdAsync_ReturnsResult(QueryRequestDto query)
    {
        // Arrange
        var exerciseTypeId = 1;
        query.PageNumber = 1;
        query.PageSize = 20;

        var expected = ExerciseMuscleGroupFixture.Query(query, exerciseTypeId);

        // Act
        var result = await _service.GetByExerciseIdAsync(exerciseTypeId, query);

        // Assert
        ExpectedQueryResult.Success(result, expected);
    }

    /// <summary>
    ///     Test for the GetEntityByIdAsync method of the ExerciseMuscleGroupService.
    /// </summary>
    /// <param name="id">The id to be used for the test.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(GetByIdAsyncData))]
    public async Task GetByIdAsync_ReturnsResult(int id, string expected)
    {
        // Arrange
        var exerciseMuscleGroups = ExerciseMuscleGroups.FirstOrDefault(q => q.Id == id);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedGetByIdResult.Success(_logger, result, exerciseMuscleGroups!, id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedGetByIdResult.BadRequest(_logger, result, EntityErrorMessage<ExerciseMuscleGroup>.BadRequest());
        else if (expected == TestServiceResponse.NotFound)
            ExpectedGetByIdResult.NotFound(_logger, result, EntityErrorMessage<ExerciseMuscleGroup>.NotFound(id), id);
    }

    /// <summary>
    ///     Test for the CreateEntityAsync method of the ExerciseMuscleGroupService.
    /// </summary>
    /// <param name="entity">The entity to be used for the test.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(CreateExerciseMuscleGroupDtoData))]
    public async Task CreateAsync_ReturnsResult(CreateExerciseMuscleGroupDto entity, string expected)
    {
        // Arrange

        // Act
        var result = await _service.CreateAsync(entity);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedCreateResult.Success<ExerciseMuscleGroupService, ExerciseMuscleGroup>(_logger, result, result.Value,
                result.Value);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedCreateResult.BadRequest<ExerciseMuscleGroupService, ExerciseMuscleGroup>(_logger, result,
                result.Error.Errors!);
    }

    /// <summary>
    ///     Test for the UpdateEntityAsync method of the ExerciseMuscleGroupService.
    /// </summary>
    /// <param name="entity">The entity to be used for the test.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(UpdateExerciseMuscleGroupDtoData))]
    public async Task UpdateAsync_ReturnsResult(UpdateExerciseMuscleGroupDto entity, string expected)
    {
        // Arrange
        //var exerciseMuscleGroups = _exerciseMuscleGroups.FirstOrDefault(q => q.Id == entity.Id);

        // Act
        var result = await _service.UpdateAsync(entity);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedUpdateResult.Success<ExerciseMuscleGroupService, ExerciseMuscleGroup>(_logger, result, entity.Id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedUpdateResult.BadRequest<ExerciseMuscleGroupService, ExerciseMuscleGroup>(_logger, result,
                result.Error.Errors!);
    }

    /// <summary>
    ///     Test for the DeleteEntityAsync method of the ExerciseMuscleGroupService.
    /// </summary>
    /// <param name="id">The id to be used for the test.</param>
    /// <param name="expected">The expected result of the test.</param>
    [Theory]
    [MemberData(nameof(DeleteAsyncData))]
    public async Task DeleteAsync_ReturnsResult(int id, string expected)
    {
        // Arrange
        //var exerciseMuscleGroups = _exerciseMuscleGroups.FirstOrDefault(q => q.Id == id);

        // Act
        var result = await _service.DeleteAsync(id);

        // Assert
        if (expected == TestServiceResponse.Success)
            ExpectedDeleteResult.Success<ExerciseMuscleGroupService, ExerciseMuscleGroup>(_logger, result, id);
        else if (expected == TestServiceResponse.BadRequest)
            ExpectedDeleteResult.BadRequest<ExerciseMuscleGroupService, ExerciseMuscleGroup>(_logger, result,
                EntityErrorMessage<ExerciseMuscleGroup>.BadRequest());
        else if (expected == TestServiceResponse.NotFound)
            ExpectedDeleteResult.NotFound<ExerciseMuscleGroupService, ExerciseMuscleGroup>(_logger, result,
                EntityErrorMessage<ExerciseMuscleGroup>.NotFound(id), id);
    }



#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type. This is a test class, so we need to test null values

    /// <summary>
    ///     Provides data for the CreateExerciseMuscleGroupDtoData test.
    /// </summary>
    /// <returns> TheoryData </returns>
    public static TheoryData<CreateExerciseMuscleGroupDto, string> CreateExerciseMuscleGroupDtoData()
    {
        return new TheoryData<CreateExerciseMuscleGroupDto, string>
        {
            { new CreateExerciseMuscleGroupDto { ExerciseTypeId = 1, MuscleGroupId = 2, MuscleFunctionId = 3, FunctionPercentage = 50 }, TestServiceResponse.Success },
            { new CreateExerciseMuscleGroupDto { ExerciseTypeId = 0, MuscleGroupId = 1, MuscleFunctionId = 1, FunctionPercentage = 50 }, TestServiceResponse.BadRequest },
            { new CreateExerciseMuscleGroupDto { ExerciseTypeId = 1, MuscleGroupId = 0, MuscleFunctionId = 1, FunctionPercentage = 50 }, TestServiceResponse.BadRequest },
            { new CreateExerciseMuscleGroupDto { ExerciseTypeId = 1, MuscleGroupId = 1, MuscleFunctionId = 0, FunctionPercentage = 50 }, TestServiceResponse.BadRequest },
            { new CreateExerciseMuscleGroupDto { ExerciseTypeId = 1, MuscleGroupId = 1, MuscleFunctionId = 1, FunctionPercentage = 0 }, TestServiceResponse.BadRequest }
        };
    }
    

    /// <summary>
    ///     Provides data for the UpdateExerciseMuscleGroupDtoData test.
    /// </summary>
    /// <returns> TheoryData </returns>
    public static TheoryData<UpdateExerciseMuscleGroupDto, string> UpdateExerciseMuscleGroupDtoData()
    {
        return new TheoryData<UpdateExerciseMuscleGroupDto, string>
        {
            { new UpdateExerciseMuscleGroupDto { Id = 1, FunctionPercentage = 50 }, TestServiceResponse.Success },
            { new UpdateExerciseMuscleGroupDto { Id = 0, FunctionPercentage = 50 }, TestServiceResponse.BadRequest },
            { new UpdateExerciseMuscleGroupDto { Id = 1, FunctionPercentage = 0 }, TestServiceResponse.BadRequest }
        };
    }
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.

}