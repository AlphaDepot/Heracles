using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.EquipmentGroups.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;

public abstract class MockEquipmentGroupRepository
{
    
    public static Mock<IEquipmentGroupRepository> Get()
    {
        var equipmentGroups = EquipmentGroupFixture.Get();
        
        var mockRepo = new Mock<IEquipmentGroupRepository>();
        
        mockRepo.Setup(r => r.GetAsync(It.IsAny<QuariableDto<EquipmentGroup>>()))
            .ReturnsAsync((QuariableDto<EquipmentGroup> queryableDto) =>
            {
                var queryable = equipmentGroups.AsQueryable();
                if (queryableDto.Filter != null)
                {
                    queryable = queryable.Where(queryableDto.Filter);
                }
                if (queryableDto.Sorter != null)
                {
                    queryable = queryableDto.Sorter(queryable);
                }
                var result =  queryable.Skip((queryableDto.PageNumber - 1) * queryableDto.PageSize).Take(queryableDto.PageSize).ToList();
                
                return new QueryResponse<EquipmentGroup>()
                {
                    Data =  result,
                    TotalPages = result.Count(),
                    PageSize = queryableDto.PageSize,
                    PageNumber = queryableDto.PageNumber
                };
                
            });

        
        mockRepo.Setup(r => r.GetByIdAsync(It.IsAny<int>()))!
            .ReturnsAsync((int id) => equipmentGroups.FirstOrDefault(q => q.Id == id ));
        
        mockRepo.Setup(r => r.CreateAsync(It.IsAny<EquipmentGroup>()))
            .ReturnsAsync((EquipmentGroup equipmentGroup) =>
            {
                equipmentGroup.Id = equipmentGroups.Count + 1;
                equipmentGroups.Add(equipmentGroup);
                return equipmentGroup.Id;
            });
        
        mockRepo.Setup(r => r.ItExist(It.IsAny<int>()))
            .ReturnsAsync((int id) => equipmentGroups.Any(q => q.Id == id));
        
        mockRepo.Setup(r => r.UpdateAsync(It.IsAny<EquipmentGroup>()))
            .ReturnsAsync((EquipmentGroup equipmentGroup) =>
            {
                var index = equipmentGroups.FindIndex(q => q.Id == equipmentGroup.Id);
                equipmentGroups[index] = equipmentGroup;
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.DeleteAsync(It.IsAny<int>()))
            .ReturnsAsync((int id) =>
            {
                var index = equipmentGroups.FindIndex(q => q.Id == id);
                equipmentGroups.RemoveAt(index);
                return 1; // number of rows affected
            });
        
        mockRepo.Setup(r => r.IsUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => !equipmentGroups.Any(q => q.Name == name));
        
        return mockRepo;
        
    }
}