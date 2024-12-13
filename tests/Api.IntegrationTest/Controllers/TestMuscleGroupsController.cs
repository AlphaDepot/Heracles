using NUnit.Framework;
using System.Net.Http.Json;
using Application.Common.Responses;
using Application.Features.MuscleGroups;
using Application.Features.MuscleGroups.Commands;

namespace Api.IntegrationTest.Controllers;

/// <summary>
/// This class contains integration tests for the MuscleGroupsController.
/// </summary>
public class TestMuscleGroupsController : BaseIntegrationTest
{
    private const string BaseUrl = "/api/MuscleGroups";

    [SetUp]
    public void SetUp()
    {
        // Initialization code if needed
    }

    /// <summary>
    /// Test to ensure that the GetMuscleGroups endpoint returns a list of MuscleGroups.
    /// </summary>
    [Test]
    public async Task GetMuscleGroups_ReturnsMuscleGroups()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync(BaseUrl);
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<PagedResponse<MuscleGroup>>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data, Is.Not.Empty);
    }

    /// <summary>
    /// Test to ensure that the GetMuscleGroupById endpoint returns the correct MuscleGroup.
    /// </summary>
    [Test]
    public async Task GetMuscleGroupById_ReturnsMuscleGroup()
    {
        // Arrange
        // Act
        var response = await Client.GetAsync($"{BaseUrl}/3");
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<MuscleGroup>();

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(3));
    }

    /// <summary>
    /// Test to ensure that the CreateMuscleGroup endpoint creates a new MuscleGroup and returns its Id.
    /// </summary>
    [Test]
    public async Task CreateMuscleGroup_ReturnsCreatedMuscleGroup()
    {
        // Arrange
        var muscleGroup = new CreateMuscleGroupRequest("Test Muscle Group");

        // Act
        var response = await Client.PostAsJsonAsync(BaseUrl, muscleGroup);
        HandleResponseFailure(response);

        var result = await response.Content.ReadFromJsonAsync<int>();

        // Assert
        Assert.That(result, Is.GreaterThan(0));
    }

    /// <summary>
    /// Test to ensure that the UpdateMuscleGroup endpoint updates a MuscleGroup correctly.
    /// </summary>
    [Test]
    public async Task UpdateMuscleGroup_ReturnsUpdatedMuscleGroup()
    {
        // Arrange
        var existingMuscleGroup = await Client.GetFromJsonAsync<MuscleGroup>($"{BaseUrl}/2");
        var muscleGroup = new UpdateMuscleGroupRequest(2, "Updated Muscle Group", existingMuscleGroup?.Concurrency);

        // Act
        var response = await Client.PutAsJsonAsync($"{BaseUrl}/2", muscleGroup);
        HandleResponseFailure(response);

        // Assert
        response.EnsureSuccessStatusCode();
    }

    /// <summary>
    /// Test to ensure that the DeleteMuscleGroup endpoint deletes a MuscleGroup correctly.
    /// </summary>
    [Test]
    public async Task DeleteMuscleGroup_ReturnsDeletedMuscleGroup()
    {
        // Arrange
        // Act
        var response = await Client.DeleteAsync($"{BaseUrl}/1");
        HandleResponseFailure(response);

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
