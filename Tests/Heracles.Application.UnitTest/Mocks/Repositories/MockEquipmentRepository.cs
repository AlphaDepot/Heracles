using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.Equipments.Interfaces;
using Heracles.Domain.Equipments.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;
/// <summary>
///  A mock repository for the Equipment entity
/// </summary>
public  class MockEquipmentRepository : MockBaseRepository<Equipment, IEquipmentRepository>
{
    /// <summary>
    ///  Create a new instance of the MockEquipmentRepository
    /// </summary>
    /// <param name="entities"> The list of entities to use for the mock repository </param>
    public MockEquipmentRepository(List<Equipment> entities) : base(entities)
    {
        IsTypeUniqueMock();
    }
    
    /// <summary>
    ///  Get a new instance of the MockEquipmentRepository
    /// </summary>
    /// <returns> A new instance of the MockEquipmentRepository </returns>
    public new static Mock<IEquipmentRepository> Get()
    {
        return new MockEquipmentRepository(EquipmentFixture.Get()).MockRepo;
    }
    
    /// <summary>
    ///  Set up the IsTypeUnique mock method for the EquipmentRepository
    /// </summary>
    private void IsTypeUniqueMock()
    {
        // Setup Equipment specific mock methods here
        MockRepo.Setup(r => r.IsTypeUnique(It.IsAny<string>()))
            .ReturnsAsync((string type) => Entities.All(q => q.Type != type));
    }
    
    
}
