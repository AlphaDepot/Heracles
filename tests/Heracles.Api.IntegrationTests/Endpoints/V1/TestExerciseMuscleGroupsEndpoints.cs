using System.Net.Http.Json;
using Heracles.Application.Features.ExerciseMuscleGroups.Commands;
using Heracles.Common.Responses;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests.ExerciseMuscleGroups;
using Heracles.Shared.Responses;

namespace Heracles.Api.IntegrationTests.Endpoints.V1;

/// <summary>
///     This class contains integration tests for the ExerciseMuscleGroupsController.
/// </summary>
public class TestExerciseMuscleGroupsEndpoints : BaseIntegrationTest
{
	private const string BaseUrl = Routes.V1Endpoints.ExerciseMuscleGroups;

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
	}

	[Test, Order(1)]
	public void Database_ShouldContainSeededExerciseMuscleGroups()
	{
		using var context = GetDbContext();

		var count = context.ExerciseMuscleGroups.Count();

		Console.WriteLine("Equipment count in DB: " + count);

		Assert.That(count, Is.GreaterThan(0));
	}


	/// <summary>
	///     Test to ensure that the GetExerciseMuscleGroups API endpoint returns a list of ExerciseMuscleGroups.
	/// </summary>
	[Test]
	public async Task GetExerciseMuscleGroups_ReturnsExerciseMuscleGroups()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync(BaseUrl);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<ExerciseMuscleGroup>>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Data, Is.Not.Empty);
		Assert.That(result.Data.Data.Count, Is.GreaterThan(0));
		Assert.That(result.Data.Data.First().Id, Is.EqualTo(1));
	}

	/// <summary>
	///     Test to ensure that the GetExerciseMuscleGroupById API endpoint returns the correct ExerciseMuscleGroup.
	/// </summary>
	[Test]
	public async Task GetExerciseMuscleGroupById_ReturnsExerciseMuscleGroup()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/3");
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<ExerciseMuscleGroup>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Id, Is.EqualTo(3));
	}


	/// <summary>
	///     Test to ensure that the CreateExerciseMuscleGroup API endpoint correctly creates an ExerciseMuscleGroup.
	/// </summary>
	[Test]
	public async Task CreateExerciseMuscleGroup_ReturnsCreatedExerciseMuscleGroup()
	{
		await using var context = GetDbContext();
		// Existing combinations
		var existing = context.ExerciseMuscleGroups
			.Select(x => new
			{
				x.ExerciseTypeId,
				x.MuscleId,
				x.FunctionId
			})
			.ToList();

		// Find first combination that does NOT already exist
		var combo =
			(from et in context.ExerciseTypes
				from m in context.MuscleGroups
				from f in context.MuscleFunctions
				select new
				{
					ExerciseTypeId = et.Id,
					MuscleId = m.Id,
					FunctionId = f.Id
				})
			.AsEnumerable()
			.FirstOrDefault(c =>
				!existing.Any(e =>
					e.ExerciseTypeId == c.ExerciseTypeId &&
					e.MuscleId == c.MuscleId &&
					e.FunctionId == c.FunctionId));

		Assert.That(combo, Is.Not.Null, "No unique ExerciseMuscleGroup combination found.");

		var request = new CreateExerciseMuscleGroupRequest(
			combo.ExerciseTypeId,
			combo.MuscleId,
			combo.FunctionId,
			98
		);

		var response = await Client.PostAsJsonAsync(BaseUrl, request);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

		Assert.That(result.Data, Is.GreaterThan(0));
	}


	/// <summary>
	///     Test to ensure that the UpdateExerciseMuscleGroup API endpoint correctly updates an ExerciseMuscleGroup.
	/// </summary>
	[Test]
	public async Task UpdateExerciseMuscleGroup_ReturnsUpdatedExerciseMuscleGroup()
	{
		// Arrange
		var newPercentage = 98;
		var res = await Client.GetFromJsonAsync<ApiResponse<ExerciseMuscleGroup>>($"{BaseUrl}/1");
		var exerciseMuscleGroup = new UpdateExerciseMuscleGroupRequest(res.Data.Id, res.Data?.Concurrency, newPercentage);

		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/{exerciseMuscleGroup.Id}", exerciseMuscleGroup);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();

		// Get the updated ExerciseMuscleGroup again
		var result = await Client.GetFromJsonAsync<ApiResponse<ExerciseMuscleGroup>>($"{BaseUrl}/1");

		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Id, Is.EqualTo(exerciseMuscleGroup.Id));
		Assert.That(result.Data.FunctionPercentage, Is.EqualTo(newPercentage));
	}

	/// <summary>
	///     Test to ensure that the DeleteExerciseMuscleGroup API endpoint correctly deletes an ExerciseMuscleGroup.
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
