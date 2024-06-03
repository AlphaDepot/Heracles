using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.Users.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;


namespace Heracles.Application.UnitTest.Mocks.Repositories;
/// <summary>
///  MockUserRepository is a mock repository for the User entity
/// </summary>
public  class MockUserRepository :  MockBaseRepository<User, IUserRepository>
{
    
    /// <summary>
    ///  Create a new instance of the MockUserRepository
    /// </summary>
    /// <param name="entities"> The list of User entities to use for the mock repository </param>
    public MockUserRepository(List<User> entities) : base(entities)
    {
        GetUserByUserIdAsyncMock();
        UserIdExistsAsyncMock();
        UserIdWithIdExistsAsyncMock();
        UserIsAdminMock();
    }
    
    /// <summary>
    ///  Get a new instance of the MockUserRepository
    /// </summary>
    /// <returns> A new instance of the MockUserRepository </returns>
    public static Mock<IUserRepository> Get()
    {
        return new MockUserRepository(UserFixture.Get()).MockRepo;
    }
    
    /// <summary>
    ///  Set up the GetUserByUserIdAsync mock method for the UserRepository
    ///  This method is used to retrieve a user by their user ID.
    /// </summary>
    private void GetUserByUserIdAsyncMock()
    {
        MockRepo.Setup(r => r.GetUserByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => Entities.FirstOrDefault(q => q.UserId == userId));
    }
    
    /// <summary>
    ///  Set up the UserIdExistsAsync mock method for the UserRepository
    ///  This method is used to check if a user with the given user ID exists.
    /// </summary>
    private void UserIdExistsAsyncMock()
    {
        MockRepo.Setup(r => r.UserIdExistsAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => Entities.Any(q => q.UserId == userId));
    }
    
    /// <summary>
    ///  Set up the UserIdWithIdExistsAsync mock method for the UserRepository
    ///  This method is used to check if a user with the specified userId and entity Id combination  exists in the repository.
    /// </summary>
    private  void UserIdWithIdExistsAsyncMock()
    {
        MockRepo.Setup(r => r.UserIdWithIdExistsAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync((string userId, int id) => Entities.FirstOrDefault(q => q.UserId == userId && q.Id != id) == null);
    }
    
    /// <summary>
    ///  Set up the IsAdminUser mock method for the UserRepository
    ///  This method is used to check if a user is an admin based on their user ID.
    /// </summary>
    private void UserIsAdminMock()
    {
        MockRepo.Setup(r => r.IsAdminUser(It.IsAny<string>()))
            .ReturnsAsync((string userId) => Entities.Any(q => q?.UserId == userId &&  q.Roles != null && q.Roles.Contains("Admin")));
    }
}
