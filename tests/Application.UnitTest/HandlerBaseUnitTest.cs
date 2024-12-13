using Application.Infrastructure.Data;
using Application.UnitTest.TestData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.UnitTest;

/// <summary>
///     Base class for all handler unit tests
/// </summary>
public class HandlerBaseUnitTest
{
	private DbContextOptions<AppDbContext> _dbOptions;

	protected AppDbContext DbContext;

	protected readonly IHttpContextAccessor HttpContextAccessor = new HttpContextAccessor();

	[SetUp]
	public void Setup()
	{
		_dbOptions = new DbContextOptionsBuilder<AppDbContext>()
			.UseInMemoryDatabase(Guid.NewGuid().ToString())
			.EnableSensitiveDataLogging() // Enable sensitive data logging
			.Options;

		DbContext = new AppDbContext(_dbOptions);
		DbContext.Database.EnsureCreated(); // Ensure the database is created for each test

		HttpContextAccessor.HttpContext = new DefaultHttpContext();
		HttpContextAccessor.HttpContext.User = UserData.Users().First().ToClaimsPrincipal();
	}


	[TearDown]
	public void TearDown()
	{
		DbContext.Dispose();
	}
}
