namespace Api.Extensions;

public static class CorsExtension
{
	public static IServiceCollection AddCorsPolicy(this IServiceCollection services, string policyName)
	{
		services.AddCors(options =>
		{
			options.AddPolicy(policyName, builder =>
			{
				builder.AllowAnyOrigin()
					.AllowAnyMethod()
					.AllowAnyHeader();
			});
		});

		return services;
	}

	public static IApplicationBuilder UseCorsPolicy(this IApplicationBuilder app, string policyName)
	{
		app.UseCors(policyName);
		return app;
	}
}
