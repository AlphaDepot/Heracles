using Api.Extensions;
using Application;
using Application.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));



builder.Services.AddApplicationServices(builder.Configuration);

builder.Host.UseApplicationSerilog();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();


// Cors policy
const string corsPolicy = "CorsPolicy";
builder.Services.AddCorsPolicy(corsPolicy);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.LoadTestingSeedData();
}

app.UseMiddleware<RequestLogContextMiddleware>();
app.UseSerilogRequestLogging();

//app.UseHttpsRedirection();

app.UseCorsPolicy(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

//  Create a public partial class Program to enable testing
public partial class Program
{
}
