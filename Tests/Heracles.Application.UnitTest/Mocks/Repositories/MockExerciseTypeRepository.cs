using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Equipments.Models;
using Heracles.Domain.ExercisesType.Interfaces;
using Heracles.Domain.ExercisesType.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockExerciseTypeRepository
{
    public static Mock<IExerciseTypeRepository> Get()
    {
        var exerciseTypes = ExerciseTypeFixture.Get();
        
        var mockRepo = new Mock<IExerciseTypeRepository>();
        
        
        mockRepo.Setup(r => r.GetAsync(It.IsAny<QuariableDto<ExerciseType>>()))
            .ReturnsAsync((QuariableDto<ExerciseType> queryableDto) =>
            {
                var result = exerciseTypes.AsQueryable();
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
            .ReturnsAsync((int id) => exerciseTypes.FirstOrDefault(q => q.Id == id ));
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<ExerciseType>()))
            .Returns((ExerciseType exerciseType) =>
            {
                exerciseType.Id = exerciseTypes.Count + 1;
                exerciseTypes.Add(exerciseType);
                return Task.FromResult(exerciseType.Id);
            });
        
        mockRepo.Setup(r => r.IsNameUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => { 
                return !exerciseTypes.Any(q => q.Name == name);
            });
        
        mockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => exerciseTypes.Any(q => q.Id == id));
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<ExerciseType>()))
            .ReturnsAsync((ExerciseType exerciseType) =>
            {
                var index = exerciseTypes.FindIndex(q => q.Id == exerciseType.Id);
                exerciseTypes[index] = exerciseType;
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var index = exerciseTypes.FindIndex(q => q.Id == id);
                exerciseTypes.RemoveAt(index);
                return 1; // number of rows affected
            });
        
        return mockRepo;
        
        
    }
}