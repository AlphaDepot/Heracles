using Heracles.Domain.EquipmentGroups.Models;
using Heracles.Domain.Equipments.Models;

namespace Heracles.TestUtilities.TestData;

public static class EquipmentSeedData
{
    /// <summary>
    ///  SeedData data for Equipment
    ///  It must be the first  seed data to be inserted in the equipment category
    /// </summary>
    /// <returns> List of Equipment</returns>
    public static List<Equipment> Equipments()
    {
        var date = new DateTime(2022, 1, 1);
        return
        [
            new Equipment { Id = 1, Type = "Barbell", Weight = 45, CreatedAt = date, UpdatedAt = date, },
            new Equipment { Id = 2, Type = "Dumbbell", Weight = 30, CreatedAt = date, UpdatedAt = date, },
            new Equipment { Id = 3, Type = "Cable", Resistance = 100, CreatedAt = date, UpdatedAt = date, },
            new Equipment { Id = 4, Type = "Kettlebell", Weight = 35, CreatedAt = date, UpdatedAt = date, }
        ];
    }
    
    /// <summary>
    ///  SeedData data for EquipmentGroup
    ///  It must be the second seed data to be inserted in the equipment category
    ///  It depends on EquipmentSeedData.Equipments
    /// </summary>
    /// <returns> List of EquipmentGroup</returns>
    public static List<EquipmentGroup> EquipmentGroups()
    {
        var date = new DateTime(2022, 1, 1);
        return
        [
            new EquipmentGroup { Id = 1, Name = "Home Gym",  CreatedAt = date, UpdatedAt = date,
                // get the first and third equipment into the group
                Equipments =[Equipments().First(), Equipments()[2]]
            },
            new EquipmentGroup { Id = 2, Name = "Gym",  CreatedAt = date, UpdatedAt = date,
                // get the second, third and fourth equipment into the group
                Equipments =[  Equipments()[1], Equipments()[2],  Equipments()[3]] 
            },
            new EquipmentGroup { Id = 3, Name = "Work",  CreatedAt = date, UpdatedAt = date,
                // get all the equipments into the group
                Equipments = Equipments() }
        ];
    }
    
    
  
    
}