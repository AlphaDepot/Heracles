namespace Heracles.Api.IntegrationTests;

public static class Routes
{
	private const string BaseUrl = "/api";

	public static class V1Endpoints
	{
		private const string V1BaseUrl = $"{BaseUrl}/v1";

		public const string Equipment = $"{V1BaseUrl}/equipment";
		public const string EquipmentGroups = $"{V1BaseUrl}/equipment-groups";

		public const string ExerciseMuscleGroups = $"{V1BaseUrl}/exercise-muscle-groups";
		public const string ExerciseType = $"{V1BaseUrl}/exercise-types";
		public const string MuscleFunctions = $"{V1BaseUrl}/muscle-functions";
		public const string MuscleGroups = $"{V1BaseUrl}/muscle-groups";
		public const string UserExerciseHistories = $"{V1BaseUrl}/user-exercise-histories";
		public const string UserExercises = $"{V1BaseUrl}/user-exercises";



		public const string Users = $"{V1BaseUrl}/users";
		public const string WorkoutSessions = $"{V1BaseUrl}/workout-sessions";
	}
}
