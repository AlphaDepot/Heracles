using Heracles.Domain.Entities;

namespace Heracles.Persistence.SeedData;

internal abstract class EquipmentDataLoader
{
	public static void Initialize(AppDbContext context)
	{
		SeedEquipments(context);
		SeedEquipmentGroups(context);
	}

	private static void SeedEquipments(AppDbContext context)
	{
		context.Database.EnsureCreated();

		if (context.Equipments.Any())
		{
			return;
		}

		context.Equipments.AddRange(Equipments());
		context.SaveChanges();
	}

	private static void SeedEquipmentGroups(AppDbContext context)
	{
		context.Database.EnsureCreated();

		if (context.EquipmentGroups.Any())
		{
			return;
		}

		var groups = EquipmentGroups(context);

		context.EquipmentGroups.AddRange(groups);
		context.SaveChanges();

		// IMPORTANT: build relationships AFTER save
		var equipments = context.Equipments.ToList();
		var savedGroups = context.EquipmentGroups.ToList();

		savedGroups[0].Equipments = new List<Equipment> { equipments[0], equipments[2] };
		savedGroups[1].Equipments = new List<Equipment> { equipments[1], equipments[2], equipments[3] };
		savedGroups[2].Equipments = new List<Equipment> { equipments[0], equipments[1], equipments[2], equipments[3] };

		context.SaveChanges();
	}

	private static List<Equipment> Equipments()
	{
		var date = new DateTime(2022, 1, 1).ToUniversalTime();

		return
		[
			new Equipment { Type = "Barbell", Weight = 45, CreatedAt = date, UpdatedAt = date },
			new Equipment { Type = "Dumbbell", Weight = 30, CreatedAt = date, UpdatedAt = date },
			new Equipment { Type = "Cable", Resistance = 100, CreatedAt = date, UpdatedAt = date },
			new Equipment { Type = "Kettlebell", Weight = 35, CreatedAt = date, UpdatedAt = date }
		];
	}

	private static List<EquipmentGroup> EquipmentGroups(AppDbContext context)
	{
		var date = new DateTime(2022, 1, 1).ToUniversalTime();

		return
		[
			new EquipmentGroup
				{ Name = "Home Gym", CreatedAt = date, UpdatedAt = date, Equipments = new List<Equipment>() },
			new EquipmentGroup { Name = "Gym", CreatedAt = date, UpdatedAt = date, Equipments = new List<Equipment>() },
			new EquipmentGroup { Name = "Work", CreatedAt = date, UpdatedAt = date, Equipments = new List<Equipment>() }
		];
	}
}
