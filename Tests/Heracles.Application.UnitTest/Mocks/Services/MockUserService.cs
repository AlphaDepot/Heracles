using Heracles.Domain.Users.Interfaces;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Services;

public abstract class MockUserService
{
    public static Mock<IUserService> Get()
    {
        var users = UserFixture.Get();
        
        var mockService = new Mock<IUserService>();
        
        mockService.Setup(s => s.IsUserAuthorized(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync( (string userId, string currentUserId) =>
            {
                var isSameUser = userId == currentUserId;
                var isUserAdmin = users.Any(q => q.Roles!.Contains("Admin"));
                return isSameUser || isUserAdmin;
            });

        mockService.Setup(s => s.DoesUserExist(It.IsAny<string>()))
            .ReturnsAsync((string userId) => users.Any(q => q.UserId == userId));
            
        
        
        
        return mockService;
    }
}