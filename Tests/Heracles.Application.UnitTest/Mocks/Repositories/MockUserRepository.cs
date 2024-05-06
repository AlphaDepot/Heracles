using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Users.Interfaces;
using Heracles.Domain.Users.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockUserRepository
{
    public static Mock<IUserRepository> Get()
    {
        
        var users = UserFixture.Get();
        
        var mockRepo = new Mock<IUserRepository>();
        
        
        mockRepo.Setup(r => r.GetAsync(It.IsAny<QuariableDto<User>>()))
            .ReturnsAsync((QuariableDto<User> queryableDto) =>
            {
                var result = users.AsQueryable();
                if (queryableDto.Filter != null)
                {
                    result = result.Where(queryableDto.Filter);
                }
                if (queryableDto.Sorter != null)
                {
                    result = queryableDto.Sorter(result);
                }
                return result.Skip((queryableDto.PageNumber - 1) * queryableDto.PageSize).Take(queryableDto.PageSize).ToList();
            });
        
        mockRepo.Setup(r => r.GetUserByUserIdAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => users.FirstOrDefault(q => q.UserId == userId));
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync((User user) =>
            {
                user.Id = users.Count + 1;
                users.Add(user);
                return user.Id;
            });
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync((User? user) =>
            {
                var index = users.FindIndex(q => q.Id == user.Id);
                users[index] = user;
                return 1; // number of rows affected
            });

        
        mockRepo.Setup(r => r.UserIdExistsAsync(It.IsAny<string>()))
            .ReturnsAsync((string userId) => users.Any(q => q.UserId == userId));
        
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var index = users.FindIndex(q => q.Id == id);
                users.RemoveAt(index);
                return 1; // number of rows affected
            });
        
   
        mockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => users.Any(q => q.Id == id));
        
        
        mockRepo.Setup(r => r.UserIdWithIdExistsAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync((string userId, int id) => users.FirstOrDefault(q => q.UserId == userId && q.Id != id) == null);
        
        
        mockRepo.Setup(r => r.UserIsAdmin(It.IsAny<string>()))
            .ReturnsAsync((string userId) => users.Any(q => q?.UserId == userId &&  q.Roles != null && q.Roles.Contains("Admin")));
        
        
        mockRepo.Setup(r => r.DeleteUserAsync(It.IsAny<int>(), It.IsAny<string>()))
            .ReturnsAsync((int id, string userId) =>
            {
                var index = users.FindIndex(q => q.Id == id);
                users.RemoveAt(index);
                return 1; // number of rows affected
            });
        
        
        return mockRepo;
        
    }
    
}