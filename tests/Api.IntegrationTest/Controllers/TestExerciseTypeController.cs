using NUnit.Framework;
using System.Net.Http.Json;
using Application.Common.Responses;
using Application.Features.ExerciseTypes;
using Application.Features.ExerciseTypes.Commands;

namespace Api.IntegrationTest.Controllers;

/// <summary>
/// This class contains integration tests for the ExerciseTypeController.
/// </summary>
public class TestExerciseTypeController : BaseIntegrationTest
{
    private const string BaseUrl = "/api/exercisetypes";

    [SetUp]
    public void SetUp()
    {
        // Initialization code if needed
    }

    /// <summary>
    /// Test to ensure that the GetExerciseTypes endpoint returns a list of ExerciseTypes.
    /// </summary>
    [Test]
    public async Task GetExerciseTypes_ReturnsExerciseTypes()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync(BaseUrl);
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<ExerciseType>>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Is.Not.Empty);
    }

    /// <summary>
    /// Test to ensure that the GetExerciseTypeById endpoint returns the correct ExerciseType.
    /// </summary>
    [Test]
    public async Task GetExerciseTypeById_ReturnsExerciseType()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/3");
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<ExerciseType>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(3));
    }

    /// <summary>
    /// Test to ensure that the CreateExerciseType endpoint correctly creates a new ExerciseType.
    /// </summary>
    [Test]
    public async Task CreateExerciseType_ReturnsCreatedExerciseType()
    {
        // Arrange
        var random = new Random();
        var exerciseType = new CreateExerciseTypeRequest(
	        "Test Exercise Type" + random.Next(10, 100).ToString(),
	        "Test Description",
	        "https://test.com/image.jpg");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, exerciseType);
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        Assert.That(result, Is.GreaterThan(0));
    }

    /// <summary>
    /// Test to ensure that the UpdateExerciseType endpoint correctly updates an existing ExerciseType.
    /// </summary>
    [Test]
    public async Task UpdateExerciseType_ReturnsUpdatedExerciseType()
    {
        // Arrange
       var existingExerciseType = await Client.GetFromJsonAsync<ExerciseType>($"{BaseUrl}/2");
        var exerciseType = new UpdateExerciseTypeRequest(
	        2,
	        "Test Exercise Type Updated",
	         existingExerciseType?.Concurrency,
	        "Test Description Updated",
	         "https://test.com/image.jpg");
        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/2", exerciseType);
        HandleResponseFailure(response);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Test to ensure that the DeleteExerciseType endpoint correctly deletes an existing ExerciseType.
    /// </summary>
    [Test]
    public async Task DeleteExerciseType_ReturnsDeletedExerciseType()
    {
        // Arrange
        var id = 1;

        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/{id}");
        HandleResponseFailure(response);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    ///  Test to ensure that the AttachMuscleGroup endpoint correctly attaches a MuscleGroup to an ExerciseType.
    /// </summary>
    [Test, Order(1)]
    public async Task AttachMuscleGroup_ReturnsAttachedMuscleGroup()
	{
		// Arrange
		var request = new AttachExerciseMuscleGroupRequest(1, 1);

		// Act
		var response = await Client.PatchAsJsonAsync($"{BaseUrl}/1/add", request);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}

    /// <summary>
    ///  Test to ensure that the DetachMuscleGroup endpoint correctly detaches a MuscleGroup from an ExerciseType.
    ///  Requires the AttachMuscleGroup test to be run first.
    /// </summary>
	[Test, Order(2)]
	public async Task DetachMuscleGroup_ReturnsDetachedMuscleGroup()
	{
		// Arrange
		var request = new DetachExerciseMuscleGroupRequest(1, 1);

		// Act
		var response = await Client.PatchAsJsonAsync($"{BaseUrl}/1/remove", request);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}
}
