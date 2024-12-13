using System.Net.Http.Json;
using Application.Common.Responses;
using Application.Features.MuscleFunctions;
using Application.Features.MuscleFunctions.Commands;

namespace Api.IntegrationTest.Controllers;

/// <summary>
///     This class contains integration tests for the MuscleFunctionsController.
/// </summary>
public class TestMuscleFunctionsController : BaseIntegrationTest
{
	private const string BaseUrl = "/api/MuscleFunctions";

	[SetUp]
	public void SetUp()
	{
		// Initialization code if needed
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

		var result = await response.Content.ReadFromJsonAsync<PagedResponse<MuscleFunction>>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Data, Is.Not.Empty);
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

		var result = await response.Content.ReadFromJsonAsync<MuscleFunction>();

		// Assert
		Assert.That(result, Is.Not.Null);
		Assert.That(result.Id, Is.EqualTo(2));
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

		var result = await response.Content.ReadFromJsonAsync<int>();

		// Assert
		Assert.That(result, Is.GreaterThan(0));
	}

	/// <summary>
	///     Test to ensure that the UpdateMuscleFunction endpoint updates a specific MuscleFunction.
	/// </summary>
	[Test]
	public async Task UpdateMuscleFunction_ReturnsUpdatedMuscleFunction()
	{
		// Arrange
		var existingMuscleFunction = await Client.GetFromJsonAsync<MuscleFunction>($"{BaseUrl}/2");
		var muscleFunction =
			new UpdateMuscleFunctionRequest(2, "Test Muscle Function", existingMuscleFunction?.Concurrency);
		// Act
		var response = await Client.PutAsJsonAsync($"{BaseUrl}/2", muscleFunction);
		HandleResponseFailure(response);

		// Assert
		response.EnsureSuccessStatusCode();
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
