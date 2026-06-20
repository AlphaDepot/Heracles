using System.Net.Http.Json;
using Heracles.Application.Features.MuscleGroups.Commands;
using Heracles.Common.Responses;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests.MuscleGroups;
using Heracles.Shared.Responses;

namespace Heracles.Api.IntegrationTests.Endpoints.V1;

/// <summary>
///     This class contains integration tests for the MuscleGroupsController.
/// </summary>
public class TestMuscleGroupsEndpoints : BaseIntegrationTest
{
	private const string BaseUrl = Routes.V1Endpoints.MuscleGroups;

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
	}

	[Test, Order(1)]
	public void Database_ShouldContainSeededMuscleGroups()
	{
		using var context = GetDbContext();

		var count = context.MuscleGroups.Count();

		Console.WriteLine("Muscle Groups count in DB: " + count);

		Assert.That(count, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the GetMuscleGroups endpoint returns a list of MuscleGroups.
	/// </summary>
	[Test]
	public async Task GetMuscleGroups_ReturnsMuscleGroups()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync(BaseUrl);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<MuscleGroup>>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Data, Is.Not.Empty);
	}

	/// <summary>
	///     Test to ensure that the GetMuscleGroupById endpoint returns the correct MuscleGroup.
	/// </summary>
	[Test]
	public async Task GetMuscleGroupById_ReturnsMuscleGroup()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/3");
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<MuscleGroup>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Id, Is.EqualTo(3));
	}

	/// <summary>
	///     Test to ensure that the CreateMuscleGroup endpoint creates a new MuscleGroup and returns its Id.
	/// </summary>
	[Test]
	public async Task CreateMuscleGroup_ReturnsCreatedMuscleGroup()
	{
		// Arrange
		var muscleGroup = new CreateMuscleGroupRequest("Test Muscle Group");

		// Act
		var response = await Client.PostAsJsonAsync(BaseUrl, muscleGroup);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

		// Assert
		Assert.That(result.Data, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateMuscleGroup endpoint updates a MuscleGroup correctly.
	/// </summary>
	[Test]
	public async Task UpdateMuscleGroup_ReturnsUpdatedMuscleGroup()
	{
		// Arrange
		var newName = "Updated Muscle Group";
		var existingMuscleGroup = await Client.GetFromJsonAsync<ApiResponse<MuscleGroup>>($"{BaseUrl}/2");
		var muscleGroup = new UpdateMuscleGroupRequest(2, newName, existingMuscleGroup?.Data?.Concurrency);

		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/2", muscleGroup);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
		var result = await Client.GetFromJsonAsync<ApiResponse<MuscleGroup>>($"{BaseUrl}/2");
		Assert.That(result.Data.Name, Is.EqualTo(newName));
	}

	/// <summary>
	///     Test to ensure that the DeleteMuscleGroup endpoint deletes a MuscleGroup correctly.
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
