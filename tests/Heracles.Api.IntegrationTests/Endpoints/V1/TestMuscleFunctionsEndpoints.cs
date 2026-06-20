using System.Net.Http.Json;
using Heracles.Application.Features.MuscleFunctions.Commands;
using Heracles.Common.Responses;
using Heracles.Domain.Entities;
using Heracles.Shared.Requests.MuscleFunctions;
using Heracles.Shared.Responses;

namespace Heracles.Api.IntegrationTests.Endpoints.V1;

/// <summary>
///     This class contains integration tests for the MuscleFunctionsController.
/// </summary>
public class TestMuscleFunctionsEndpoints : BaseIntegrationTest
{
	private const string BaseUrl = Routes.V1Endpoints.MuscleFunctions;

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
	}

	[Test, Order(1)]
	public void Database_ShouldContainSeededMuscleFunctions()
	{
		using var context = GetDbContext();

		var count = context.MuscleFunctions.Count();

		Console.WriteLine("MuscleFunctions count in DB: " + count);

		Assert.That(count, Is.GreaterThan(0));
	}


	/// <summary>
	///     Test to ensure that the GetMuscleFunctions endpoint returns a list of MuscleFunctions.
	/// </summary>
	[Test]
	public async Task GetMuscleFunctions_ReturnsMuscleFunctions()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync(BaseUrl);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<PagedResponse<MuscleFunction>>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Data, Is.Not.Empty);
	}

	/// <summary>
	///     Test to ensure that the GetMuscleFunctionById endpoint returns a specific MuscleFunction.
	/// </summary>
	[Test]
	public async Task GetMuscleFunctionById_ReturnsMuscleFunction()
	{
		// Arrange
		// Act
		var response = await Client.GetAsync($"{BaseUrl}/2");
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<MuscleFunction>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data.Id, Is.EqualTo(2));
	}

	/// <summary>
	///     Test to ensure that the CreateMuscleFunction endpoint creates a new MuscleFunction.
	/// </summary>
	[Test]
	public async Task CreateMuscleFunction_ReturnsCreatedMuscleFunction()
	{
		// Arrange
		var random = new Random();
		var muscleFunction = new CreateMuscleFunctionRequest("Test Muscle Function" + random.Next(1, 10));

		// Act
		var response = await Client.PostAsJsonAsync(BaseUrl, muscleFunction);
		HandleResponseFailure(response);

		var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

		// Assert
		Assert.That(result.Data, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateMuscleFunction endpoint updates a specific MuscleFunction.
	/// </summary>
	[Test]
	public async Task UpdateMuscleFunction_ReturnsUpdatedMuscleFunction()
	{
		// Arrange
		var newName = "Updated Muscle Function";
		var existingMuscleFunction = await Client.GetFromJsonAsync<ApiResponse<MuscleFunction>>($"{BaseUrl}/2");
		var muscleFunction =
			new UpdateMuscleFunctionRequest(2, newName, existingMuscleFunction?.Data?.Concurrency);
		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/2", muscleFunction);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
		var result =  await Client.GetFromJsonAsync<ApiResponse<MuscleFunction>>($"{BaseUrl}/2");
		Assert.That(result.Data.Name, Is.EqualTo(newName));
	}

	/// <summary>
	///     Test to ensure that the DeleteMuscleFunction endpoint deletes a specific MuscleFunction.
	/// </summary>
	[Test]
	public async Task DeleteMuscleFunction_ReturnsDeletedMuscleFunction()
	{
		// Arrange
		// Act
		var response = await Client.DeleteAsync($"{BaseUrl}/3");
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
	}
}
