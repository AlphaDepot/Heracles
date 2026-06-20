using Heracles.Domain.Entities;
using Heracles.Persistence.SeedData;
using Heracles.Shared.Interfaces.Repositories;
using Heracles.Shared.Interfaces.Services;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Heracles.Application.UnitTest;

/// <summary>
///     Base class for all handler unit tests
/// </summary>
public class HandlerBaseUnitTest
{
	protected ICurrentUserService CurrentUserServiceMock = null!;
	protected IEquipmentGroupRepository EquipmentGroupRepository = null!;
	protected IEquipmentRepository EquipmentRepository = null!;
	protected IExerciseMuscleGroupsRepository ExerciseMuscleGroupsRepository = null!;
	protected IExerciseTypesRepository ExerciseTypesRepository = null!;
	protected IMuscleFunctionsRepository MuscleFunctionsRepository = null!;
	protected IMuscleGroupsRepository MuscleGroupsRepository = null!;
	protected ServiceProvider Provider = null!;
	protected IUserExerciseHistoriesRepository UserExerciseHistoriesRepository = null!;
	protected IUserExercisesRepository UserExercisesRepository = null!;
	protected IUsersRepository UsersRepository = null!;
	protected IWorkoutSessionRepository WorkoutSessionRepository = null!;


	[SetUp]
	public void Setup()
	{
		// -------------------------
		// Current User
		// -------------------------
		var defaultUser = TestUsersDataLoader.Users().First();
		SetCurrentUser(defaultUser);


		Provider = TestFactory.Create(CurrentUserServiceMock);

		EquipmentGroupRepository = Provider.GetRequiredService<IEquipmentGroupRepository>();
		WorkoutSessionRepository = Provider.GetRequiredService<IWorkoutSessionRepository>();
		UsersRepository = Provider.GetRequiredService<IUsersRepository>();
		UserExercisesRepository = Provider.GetRequiredService<IUserExercisesRepository>();
		UserExerciseHistoriesRepository = Provider.GetRequiredService<IUserExerciseHistoriesRepository>();
		MuscleGroupsRepository = Provider.GetRequiredService<IMuscleGroupsRepository>();
		MuscleFunctionsRepository = Provider.GetRequiredService<IMuscleFunctionsRepository>();
		ExerciseTypesRepository = Provider.GetRequiredService<IExerciseTypesRepository>();
		ExerciseMuscleGroupsRepository = Provider.GetRequiredService<IExerciseMuscleGroupsRepository>();
		EquipmentRepository = Provider.GetRequiredService<IEquipmentRepository>();
	}


	protected void SetCurrentUser(User user, bool isAuthenticated = true)
	{
		var mock = new Mock<ICurrentUserService>();

		mock.Setup(x => x.UserId)
			.Returns(user.UserId);

		mock.Setup(x => x.IsAuthenticated)
			.Returns(isAuthenticated);

		CurrentUserServiceMock = mock.Object;
	}

	protected void SetAnonymousUser()
	{
		var mock = new Mock<ICurrentUserService>();

		mock.Setup(x => x.UserId)
			.Returns((string?)null);

		mock.Setup(x => x.IsAuthenticated)
			.Returns(false);

		CurrentUserServiceMock = mock.Object;
	}

	[TearDown]
	public void TearDown()
	{
		Provider.Dispose();
	}
}
