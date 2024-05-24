using Heracles.Domain.Abstractions.Queries;
using Heracles.Domain.EquipmentGroups.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///     Fixture for EquipmentGroup entity for unit tests
/// </summary>
public abstract class EquipmentGroupFixture
{
    public static List<EquipmentGroup> Get() => EquipmentSeedData.EquipmentGroups();
    
    
    public static List<EquipmentGroup> Query(QueryRequest? query)
    {
        return Fixtures.Query(Get(), query);
    }
    

}