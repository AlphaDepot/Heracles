using Heracles.Domain.EquipmentGroups.Interfaces;
using Heracles.Domain.EquipmentGroups.Models;
using Heracles.TestUtilities.Fixtures;
using Moq;

namespace Heracles.Application.UnitTest.Mocks.Repositories;
/// <summary>
///  This class is responsible for mocking the EquipmentGroupRepository.
/// </summary>
public  class MockEquipmentGroupRepository : MockBaseRepository<EquipmentGroup, IEquipmentGroupRepository>
{
    /// <summary>
    ///  Constructor for the MockEquipmentGroupRepository.
    /// </summary>
    /// <param name="entities"> The list of EquipmentGroup entities.</param>
    public MockEquipmentGroupRepository(List<EquipmentGroup> entities) : base(entities)
    {
        IsUniqueMock();
    }
    
    /// <summary>
    ///  Get the Mock for the EquipmentGroupRepository.
    /// </summary>
    /// <returns> The Mock for the EquipmentGroupRepository.</returns>
    public new static Mock<IEquipmentGroupRepository> Get()
    {
        return new MockEquipmentGroupRepository(EquipmentGroupFixture.Get()).MockRepo;
    }
    
    /// <summary>
    ///  Mock for the IsUnique method of the EquipmentGroupRepository.
    ///  This method is used to check if the EquipmentGroup is unique.
    /// </summary>
    private void IsUniqueMock()
    {
        // Setup EquipmentGroup specific mock methods here
        MockRepo.Setup(r => r.IsUnique(It.IsAny<string>()))
            .ReturnsAsync((string name) => Entities.All(q => q.Name != name));
    }
    
    
}