using Api.Extensions;
using Heracles.Api.Extensions;
using Heracles.Api.Middlewares;
using Heracles.Api.Services;
using Heracles.Application;
using Heracles.Common.Extensions;
using Heracles.Persistence;
using Heracles.Shared.Interfaces.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Serilog;

var builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddHttpContextAccessor();
// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);

// Cors policy
const string corsPolicy = "CorsPolicy";
builder.Services.AddCorsPolicy(corsPolicy);


// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Host.UseSerilog();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.LoadTestingSeedData();
}


app.UseMiddleware<RequestLogContextMiddleware>();
app.UseSerilogRequestLogging();

app.UseCorsPolicy(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapApiEndpoints();

app.Run();
