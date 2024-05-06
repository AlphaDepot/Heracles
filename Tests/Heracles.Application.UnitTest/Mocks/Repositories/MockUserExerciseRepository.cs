using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.UserExercises.Interfaces;
using Heracles.Domain.UserExercises.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockUserExerciseRepository
{
    
    public static Mock<IUserExerciseRepository> Get()
    {
        var userExercises = UserExerciseFixture.Get();
        
        var mockRepo = new Mock<IUserExerciseRepository>();
        
        
        mockRepo.Setup(r => r.GetAsync(It.IsAny<QuariableDto<UserExercise>>()))
            .ReturnsAsync((QuariableDto<UserExercise> queryableDto) =>
            {
                var result = userExercises.AsQueryable();
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
            .ReturnsAsync((int id) => userExercises.FirstOrDefault(q => q.Id == id ));
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<UserExercise>()))
            .ReturnsAsync((UserExercise userExercise) =>
            {
                userExercise.Id = userExercises.Count + 1;
                userExercises.Add(userExercise);
                return userExercise.Id;
            });
        
        mockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => userExercises.Any(q => q.Id == id));
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<UserExercise>()))
            .ReturnsAsync((UserExercise userExercise) =>
            {
                var index = userExercises.FindIndex(q => q.Id == userExercise.Id);
                userExercises[index] = userExercise;
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var index = userExercises.FindIndex(q => q.Id == id);
                userExercises.RemoveAt(index);
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.GetUserExerciseByExerciseTypeIdAsync(It.IsAny<int>())) 
            .ReturnsAsync((int id) => userExercises.FirstOrDefault(q => q.ExerciseTypeId == id));
        
        return mockRepo;
        
    }
    
}