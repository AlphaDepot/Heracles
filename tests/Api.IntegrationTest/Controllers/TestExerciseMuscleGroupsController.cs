using NUnit.Framework;
using System.Net.Http.Json;
using Application.Common.Responses;
using Application.Features.ExerciseMuscleGroups;
using Application.Features.ExerciseMuscleGroups.Commands;

namespace Api.IntegrationTest.Controllers;

/// <summary>
/// This class contains integration tests for the ExerciseMuscleGroupsController.
/// </summary>
public class TestExerciseMuscleGroupsController : BaseIntegrationTest
{
    private const string BaseUrl = "/api/ExerciseMuscleGroups";

    [SetUp]
    public void SetUp()
    {
        // Initialization code if needed
    }

    /// <summary>
    /// Test to ensure that the GetExerciseMuscleGroups API endpoint returns a list of ExerciseMuscleGroups.
    /// </summary>
    [Test]
    public async Task GetExerciseMuscleGroups_ReturnsExerciseMuscleGroups()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync(BaseUrl);
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<ExerciseMuscleGroup>>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Is.Not.Empty);
    }

    /// <summary>
    /// Test to ensure that the GetExerciseMuscleGroupById API endpoint returns the correct ExerciseMuscleGroup.
    /// </summary>
    [Test]
    public async Task GetExerciseMuscleGroupById_ReturnsExerciseMuscleGroup()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/3");
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<ExerciseMuscleGroup>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(3));
    }



    /// <summary>
    /// Test to ensure that the CreateExerciseMuscleGroup API endpoint correctly creates an ExerciseMuscleGroup.
    /// </summary>
    [Test]
    public async Task CreateExerciseMuscleGroup_ReturnsCreatedExerciseMuscleGroup()
    {
        // Arrange
        var exerciseMuscleGroup = new CreateExerciseMuscleGroupRequest( 1, 2, 1, 98);

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, exerciseMuscleGroup);
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        Assert.That(result, Is.GreaterThan(0));
    }

    /// <summary>
    /// Test to ensure that the UpdateExerciseMuscleGroup API endpoint correctly updates an ExerciseMuscleGroup.
    /// </summary>
    [Test]
    public async Task UpdateExerciseMuscleGroup_ReturnsUpdatedExerciseMuscleGroup()
    {
        // Arrange
        var existingExerciseMuscleGroup = await Client.GetFromJsonAsync<ExerciseMuscleGroup>($"{BaseUrl}/1");
        var exerciseMuscleGroup = new UpdateExerciseMuscleGroupRequest( 1, existingExerciseMuscleGroup?.Concurrency, 98);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/{exerciseMuscleGroup.Id}", exerciseMuscleGroup);
        HandleResponseFailure(response);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Test to ensure that the DeleteExerciseMuscleGroup API endpoint correctly deletes an ExerciseMuscleGroup.
    /// </summary>
    [Test]
    public async Task DeleteExerciseMuscleGroup_ReturnsNoContent()
    {
        // Arrange
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/2");
        HandleResponseFailure(response);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
