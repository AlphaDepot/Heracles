using Heracles.Domain.EquipmentGroups.Models;
using Heracles.Domain.Equipments.Models;
using Heracles.Persistence.DataContext;

namespace Heracles.Persistence.SeedData;

public abstract class EquipmentDataLoader
{
    
    public static void  Initialize(HeraclesDbContext context)
    {
        // !!! ORDER OF SEED DATA IS IMPORTANT!!! #1#
        // SeedData Equipment first since it has no dependencies
        SeedEquipments(context);
        // SeedData EquipmentGroup second since it depends on Equipment
        SeedEquipmentGroups(context);
    }
    
    /// <summary>
    /// SeedData the Equipment data into the database.
    /// </summary>
    /// <param name="context">The database context.</param>
    private static void SeedEquipments(HeraclesDbContext context)
    {
        // Ensure the database is created
        context.Database.EnsureCreated();

        // Check if the database is already seeded
        if (context.Equipments.Any()) return;

        // Seed data
        var equipments = Equipments();

        // Add the seed data to the database
        context.Equipments.AddRange(equipments);

        // Save the changes
        context.SaveChanges();
    }

    /// <summary>
    /// SeedData the EquipmentGroup data into the database.
    /// </summary>
    /// <param name="context">The database context.</param>
    private static void SeedEquipmentGroups(HeraclesDbContext context)
    {
        // Ensure the database is created
        context.Database.EnsureCreated();

        // Check if the database is already seeded
        if (context.EquipmentGroups.Any()) return;

        // Seed data
        var equipmentGroups = EquipmentGroups(context);

        // Add the seed data to the database
        context.EquipmentGroups.AddRange(equipmentGroups);

        // Save the changes
        context.SaveChanges();
    }

    
    /// <summary>
    ///  SeedData data for Equipment
    ///  It must be the first  seed data to be inserted in the equipment category
    /// </summary>
    /// <returns> List of Equipment</returns>
    private static List<Equipment> Equipments()
    {
        // set a date for the created and updated at fields in UTC
        var date = new DateTime(2022, 1, 1).ToUniversalTime();
        return
        [
            new Equipment {  Type = "Barbell", Weight = 45, CreatedAt =  date, UpdatedAt = date},
            new Equipment { Type = "Dumbbell", Weight = 30, CreatedAt =  date, UpdatedAt = date },
            new Equipment {  Type = "Cable", Resistance = 100, CreatedAt =  date, UpdatedAt = date },
            new Equipment {  Type = "Kettlebell", Weight = 35, CreatedAt =  date, UpdatedAt = date }
        ];
    }
   
    /// <summary>
    ///  SeedData data for EquipmentGroup
    ///  It must be the second seed data to be inserted in the equipment category
    ///  It depends on EquipmentSeedData.Equipments
    /// </summary>
    /// <param name="context">The database context.</param>
    /// <returns> List of EquipmentGroup</returns>
    private static List<EquipmentGroup> EquipmentGroups(HeraclesDbContext context)
    {
        // get all the equipments from the database context
        var equipments = context.Equipments.ToList();
        // set a date for the created and updated at fields in UTC
        var date = new DateTime(2022, 1, 1).ToUniversalTime();
        return
        [
            new EquipmentGroup {  Name = "Home Gym",  CreatedAt =  date, UpdatedAt = date,
                // get the first and third equipment into the group
                Equipments =   new List<Equipment>() { equipments.First(), equipments[2] }
            },
            new EquipmentGroup {  Name = "Gym",  CreatedAt =  date, UpdatedAt = date,
                // get the second, third, and fourth equipment into the group
                Equipments =  new List<Equipment>() { equipments[1], equipments[2], equipments[3] }
            },
            new EquipmentGroup {  Name = "Work",  CreatedAt =  date, UpdatedAt = date,
                // get all the equipments into the group
                Equipments =  new List<Equipment>() { equipments.First(), equipments[2], equipments[1], equipments[3] }
            }
        ];
    }

}