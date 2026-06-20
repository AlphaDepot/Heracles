using Serilog;

namespace Heracles.Api.Extensions;

public static class LoggingExtensions
{
	public static IHostBuilder UseSerilog(this IHostBuilder hostBuilder)
	{
		hostBuilder.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));
		return hostBuilder;
	}
}
