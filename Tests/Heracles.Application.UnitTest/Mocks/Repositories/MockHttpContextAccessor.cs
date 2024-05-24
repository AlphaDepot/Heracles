using System.Security.Claims;
using Heracles.TestUtilities.TestData;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockHttpContextAccessor
{
    public static Mock<IHttpContextAccessor> GetAdmin()
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        
        
        var adminUser = new Mock<ClaimsPrincipal>();
        var userId = UserExerciseSeedData.Users().First()!.UserId; // admin user
        
        adminUser.Setup(u 
                => u.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(
                ClaimTypes.NameIdentifier, 
                userId
            ));

        // set the id on httpContextAccessor.items["UserId"]
        mockHttpContextAccessor.Setup(h => h.HttpContext!.Items["UserId"]).Returns(userId);
        
        // set up the user to be an admin
        adminUser.Setup(u => u.IsInRole("Admin")).Returns(true);
        mockHttpContextAccessor.Setup(h => h.HttpContext!.User).Returns(adminUser.Object);
        
        
        return mockHttpContextAccessor;
    }
    
    public static Mock<IHttpContextAccessor> Get()
    {
        var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        var userId = UserExerciseSeedData.Users().Last()!.UserId; //  non admin user
        
        var notAdminUser = new Mock<ClaimsPrincipal>();
        notAdminUser.Setup(u 
            => u.FindFirst(ClaimTypes.NameIdentifier))
            .Returns(new Claim(
                ClaimTypes.NameIdentifier, 
                userId
                ));

        mockHttpContextAccessor.Setup(h => h.HttpContext!.Items["UserId"]).Returns(userId);
        
        // set up the user to be an admin
        notAdminUser.Setup(u => u.IsInRole("Admin")).Returns(false);
        
        mockHttpContextAccessor.Setup(h => h.HttpContext!.User).Returns(notAdminUser.Object);
        
        
        return mockHttpContextAccessor;
    }


}