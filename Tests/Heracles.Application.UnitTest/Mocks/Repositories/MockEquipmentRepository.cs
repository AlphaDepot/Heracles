using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Equipments.Interfaces;
using Heracles.Domain.Equipments.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockEquipmentRepository
{
    
    public static Mock<IEquipmentRepository> Get()
    {
        var equipments = EquipmentFixture.Get();
        
        var mockRepo = new Mock<IEquipmentRepository>();
        
        mockRepo.Setup(r => r.GetAsync(It.IsAny<QuariableDto<Equipment>>()))
            .ReturnsAsync((QuariableDto<Equipment> queryableDto) =>
            {
                var result = equipments.AsQueryable();
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
            .ReturnsAsync((int id) => equipments.FirstOrDefault(q => q.Id == id ));
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<Equipment>()))
            .ReturnsAsync((Equipment equipment) =>
            {
                equipment.Id = equipments.Count + 1;
                equipments.Add(equipment);
                return equipment.Id;
            });
        
        mockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => equipments.Any(q => q.Id == id));
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<Equipment>()))
            .ReturnsAsync((Equipment equipment) =>
            {
                var index = equipments.FindIndex(q => q.Id == equipment.Id);
                equipments[index] = equipment;
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var index = equipments.FindIndex(q => q.Id == id);
                equipments.RemoveAt(index);
                return 1; // number of rows affected
            });
        
        
        mockRepo.Setup(r => r.IsTypeUnique(It.IsAny<string>()))
            .ReturnsAsync((string type) => !equipments.Any(q => q.Type == type));
        
        return mockRepo;
        
    }
    
}