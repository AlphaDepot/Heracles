using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.MuscleGroups.Interfaces;
using Heracles.Domain.MuscleGroups.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockMuscleGroupRepository
{
    public static Mock<IMuscleGroupRepository> Get()
    {
        var muscleGroups = MuscleGroupFixture.Get();
        
        var mockRepo = new Mock<IMuscleGroupRepository>();
        
        
        
        mockRepo.Setup(r => r.GetAsync(It.IsAny<QuariableDto<MuscleGroup>>()))
            .ReturnsAsync((QuariableDto<MuscleGroup> queryableDto) =>
            {
                var result = muscleGroups.AsQueryable();
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
            .ReturnsAsync((int id) => muscleGroups.FirstOrDefault(q => q.Id == id ));
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<MuscleGroup>()))
            .ReturnsAsync((MuscleGroup muscleGroup) =>
            {
                muscleGroup.Id = muscleGroups.Count + 1;
                muscleGroups.Add(muscleGroup);
                return muscleGroup.Id;
            });
        
        mockRepo.Setup(r => r.IsNameUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => { 
                return !muscleGroups.Any(q => q.Name == name);
            });
        

        mockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => muscleGroups.Any(q => q.Id == id));
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<MuscleGroup>()))
            .ReturnsAsync((MuscleGroup muscleGroup) =>
            {
                var index = muscleGroups.FindIndex(q => q.Id == muscleGroup.Id);
                muscleGroups[index] = muscleGroup;
                return 1; // number of rows affected
            });
        
        
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var index =  muscleGroups.FindIndex(q => q.Id == id);
                muscleGroups.RemoveAt(index);
                return 1; // number of rows affected
            });
        
        return mockRepo;
        
    }
}