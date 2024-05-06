using System.Linq.Expressions;
using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.MuscleFunctions.Interfaces;
using Heracles.Domain.MuscleFunctions.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockMuscleFunctionRepository
{
    public static Mock<IMuscleFunctionRepository> Get()
    {
        var muscleFunctions = MuscleFunctionFixture.Get();
        
        var mockRepo = new Mock<IMuscleFunctionRepository>();
        
        mockRepo.Setup(r => r.GetAsync(It.IsAny<QuariableDto<MuscleFunction>>()))
            .ReturnsAsync((QuariableDto<MuscleFunction> queryableDto) =>
            {
                var result = muscleFunctions.AsQueryable();
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
            .ReturnsAsync((int id) => muscleFunctions.FirstOrDefault(q => q.Id == id ));
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<MuscleFunction>()))
            .ReturnsAsync((MuscleFunction muscleFunction) =>
            {
                muscleFunction.Id = muscleFunctions.Count + 1;
                muscleFunctions.Add(muscleFunction);
                return muscleFunction.Id;
            });
        
        mockRepo.Setup(r => r.IsNameUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => { 
                return !muscleFunctions.Any(q => q.Name == name);
            });
        
        mockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => muscleFunctions.Any(q => q.Id == id));
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<MuscleFunction>()))
            .ReturnsAsync((MuscleFunction muscleFunction) =>
            {
                var index = muscleFunctions.FindIndex(q => q.Id == muscleFunction.Id);
                muscleFunctions[index] = muscleFunction;
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var index = muscleFunctions.FindIndex(q => q.Id == id);
                muscleFunctions.RemoveAt(index);
                return 1; // number of rows affected
            });
        
        return mockRepo;
        
    }
    
}