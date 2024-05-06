using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.UserExerciseHistories.Interfaces;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockUserExerciseHistoryRepository
{
    
    public static Mock<IUserExerciseHistoryRepository> Get()
    {
        var userExerciseHistories = UserExerciseHistoryFixture.Get();
        
        var mockRepo = new Mock<IUserExerciseHistoryRepository>();
        
        mockRepo.Setup(r => r.GetAsync(It.IsAny<QuariableDto<UserExerciseHistory>>()))
            .ReturnsAsync((QuariableDto<UserExerciseHistory> queryableDto) =>
            {
                var result = userExerciseHistories.AsQueryable();
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
        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))!
            .ReturnsAsync((int id) => userExerciseHistories.FirstOrDefault(q => q.Id == id ));
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<UserExerciseHistory>()))
            .ReturnsAsync((UserExerciseHistory userExerciseHistory) =>
            {
                userExerciseHistory.Id = userExerciseHistories.Count + 1;
                userExerciseHistories.Add(userExerciseHistory);
                return userExerciseHistory.Id;
            });
        
        mockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => userExerciseHistories.Any(q => q.Id == id));
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<UserExerciseHistory>()))
            .ReturnsAsync((UserExerciseHistory userExerciseHistory) =>
            {
                var index = userExerciseHistories.FindIndex(q => q.Id == userExerciseHistory.Id);
                userExerciseHistories[index] = userExerciseHistory;
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var index = userExerciseHistories.FindIndex(q => q.Id == id);
                userExerciseHistories.RemoveAt(index);
                return 1; // number of rows affected
            });
        
        return mockRepo;
        
    }
}