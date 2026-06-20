using System.Net.Http.Json;
using Heracles.Application.Features.ExerciseTypes.Commands;
using Heracles.Common.Responses;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests.ExerciseTypes;
using Heracles.Shared.Responses;

namespace Heracles.Api.IntegrationTests.Endpoints.V1;

/// <summary>
///     This class contains integration tests for the ExerciseTypeController.
/// </summary>
public class TestExerciseTypeEndpoints : BaseIntegrationTest
{
	private const string BaseUrl = Routes.V1Endpoints.ExerciseType;

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
	}

	[Test, Order(1)]
	public void Database_ShouldContainSeededExerciseTypes()
	{
		using var context = GetDbContext();

		var count = context.ExerciseTypes.Count();

		Console.WriteLine("Exercise Types count in DB: " + count);

		Assert.That(count, Is.GreaterThan(0));
	}


	/// <summary>
	///     Test to ensure that the GetExerciseTypes endpoint returns a list of ExerciseTypes.
	/// </summary>
	[Test]
	public async Task GetExerciseTypes_ReturnsExerciseTypes()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync(BaseUrl);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<ExerciseType>>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Data, Is.Not.Empty);
	}

	/// <summary>
	///     Test to ensure that the GetExerciseTypeById endpoint returns the correct ExerciseType.
	/// </summary>
	[Test]
	public async Task GetExerciseTypeById_ReturnsExerciseType()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/3");
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<ExerciseType>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Id, Is.EqualTo(3));
	}

	/// <summary>
	///     Test to ensure that the CreateExerciseType endpoint correctly creates a new ExerciseType.
	/// </summary>
	[Test]
	public async Task CreateExerciseType_ReturnsCreatedExerciseType()
	{
		// Arrange
		var random = new Random();
		var exerciseType = new CreateExerciseTypeRequest(
			"Test Exercise Type" + random.Next(10, 100),
			"Test Description",
			["https://test.com/image.jpg"]);

		// Act
		var response = await Client.PostAsJsonAsync(BaseUrl, exerciseType);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

		// Assert
		Assert.That(result.Data, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateExerciseType endpoint correctly updates an existing ExerciseType.
	/// </summary>
	[Test]
	public async Task UpdateExerciseType_ReturnsUpdatedExerciseType()
	{
		// Arrange
		var newName = "Test Exercise Type Updated";
		var newDescription = "Test Description Updated";
		var newImageList = new List<string>()
		{
			"https://test.com/image.jpg"
		};

		var existingExerciseType = await Client.GetFromJsonAsync<ApiResponse<ExerciseType>>($"{BaseUrl}/2");
		var exerciseType = new UpdateExerciseTypeRequest(
			2,
			newName,
			existingExerciseType?.Data?.Concurrency,
			newDescription,
			newImageList);
		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/2", exerciseType);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
		var result = await Client.GetFromJsonAsync<ApiResponse<ExerciseType>>($"{BaseUrl}/2");
		Assert.That(result.Data.Id, Is.EqualTo(2));
		Assert.That(result.Data.Name, Is.EqualTo(newName));
		Assert.That(result.Data.Description, Is.EqualTo(newDescription));
		Assert.That(result.Data.Images, Is.EqualTo(newImageList));
	}

	/// <summary>
	///     Test to ensure that the DeleteExerciseType endpoint correctly deletes an existing ExerciseType.
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
	///     Test to ensure that the AttachMuscleGroup endpoint correctly attaches a MuscleGroup to an ExerciseType.
	/// </summary>
	[Test]
	[Order(1)]
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
	///     Test to ensure that the DetachMuscleGroup endpoint correctly detaches a MuscleGroup from an ExerciseType.
	///     Requires the AttachMuscleGroup test to be run first.
	/// </summary>
	[Test]
	[Order(2)]
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
