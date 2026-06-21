using Heracles.Blazor.Services;
using Heracles.Blazor.Theme;
using Microsoft.Extensions.DependencyInjection;

namespace Heracles.Blazor.Extensions;

public static class ServiceRegistration
{

	public static IServiceCollection AddHeracles(this IServiceCollection services)
	{
		services.AddScoped<JavascriptUtils>();
		services.AddScoped<IThemeManager, ThemeManager>();
		return services;
	}

}
