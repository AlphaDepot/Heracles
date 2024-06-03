using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.EquipmentGroups.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///     Fixture for EquipmentGroup entity for unit tests
/// </summary>
public abstract class EquipmentGroupFixture
{
    public static List<EquipmentGroup> Get() => EquipmentSeedData.EquipmentGroups();
    
    
    public static List<EquipmentGroup> Query(QueryRequestDto? query)
    {
        return Fixtures.Query(Get(), query);
    }
    

}