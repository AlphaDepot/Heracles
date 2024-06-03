using Heracles.Domain.Abstractions.DTOs;
using Heracles.Domain.Equipments.Models;
using Heracles.TestUtilities.TestData;

namespace Heracles.TestUtilities.Fixtures;

/// <summary>
///     Fixture for Equipment entity for unit tests
/// </summary>
public abstract class EquipmentFixture
{
    public static List<Equipment> Get() => EquipmentSeedData.Equipments();


    public static List<Equipment> Query(QueryRequestDto? query)
    {
        return Fixtures.Query(Get(), query);
    }
}