using System.Runtime.CompilerServices;
using Heracles.Application;
using Heracles.Infrastructure;
using Heracles.Infrastructure.Middlewares;
using Heracles.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
// configuration

//builder.Services.Configure<Authentication>(builder.Configuration.GetSection("AzureAd"));


// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));

//logger
builder.Host.UseSerilog((context, config) => config.ReadFrom.Configuration(context.Configuration));


builder.Services.AddApplicationServices();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddInfrastructureServices();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Cors policy
const string corsPolicy = "CorsPolicy";
builder.Services.AddCors(options =>
{
    // loa corse from configuration settings
    
    options.AddPolicy(name: corsPolicy,
        policy  =>
        {
            
            var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
            var allowedMethods = builder.Configuration.GetSection("CorsSettings:AllowedMethods").Get<string[]>();
            var allowedHeaders = builder.Configuration.GetSection("CorsSettings:AllowedHeaders").Get<string[]>();
            
            if (allowedOrigins == null || allowedMethods == null || allowedHeaders == null)
            {
                throw new Exception("Cors settings are not configured properly");
            }
            
            policy.WithOrigins(allowedOrigins)
                .WithMethods(allowedMethods)
                .WithHeaders(allowedHeaders);
        });
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.LoadSeedData();
}


// Middleware to add the CorrelationId to the Serilog LogContext
app.UseMiddleware<RequestLogContextMiddleware>();
app.UseSerilogRequestLogging();

//app.UseHttpsRedirection();

app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();

// Middleware to add user information to the HttpContext
// must be after UseAuthentication and UseAuthorization
app.UseMiddleware<ClaimsToUserMiddleware>();

app.MapControllers();

app.Run();


//  Create a public partial class Program to enable testing
public partial class Program {}