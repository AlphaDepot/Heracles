using Heracles.Domain.Entities;
using Heracles.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
	public DbSet<ExerciseType> ExerciseTypes { get; set; }
	public DbSet<ExerciseMuscleGroup> ExerciseMuscleGroups { get; set; }
	public DbSet<MuscleGroup> MuscleGroups { get; set; }
	public DbSet<MuscleFunction> MuscleFunctions { get; set; }
	public DbSet<UserExerciseHistory> UserExerciseHistories { get; set; }
	public DbSet<UserExercise> UserExercises { get; set; }
	public DbSet<WorkoutSession> WorkoutSessions { get; set; }
	public DbSet<EquipmentGroup> EquipmentGroups { get; set; }
	public DbSet<Equipment> Equipments { get; set; }

	public DbSet<User> Users { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<ExerciseType>()
			.HasIndex(ed => ed.Name)
			.IsUnique();


		modelBuilder.Entity<MuscleGroup>()
			.HasIndex(mg => mg.Name)
			.IsUnique();

		modelBuilder.Entity<MuscleFunction>()
			.HasIndex(mf => mf.Name)
			.IsUnique();

		modelBuilder.Entity<EquipmentGroup>()
			.HasIndex(eg => eg.Name)
			.IsUnique();

		modelBuilder.Entity<EquipmentGroup>()
			.HasMany(x => x.Equipments)
			.WithMany();

		modelBuilder.Entity<Equipment>()
			.HasIndex(e => e.Type)
			.IsUnique();
	}

	public override int SaveChanges()
	{
		UpdateEntityMetadata();
		return base.SaveChanges();
	}

	public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		UpdateEntityMetadata();
		return base.SaveChangesAsync(cancellationToken);
	}

	private void UpdateEntityMetadata()
	{
		var entries = ChangeTracker
			.Entries()
			.Where(e => e.Entity is IEntity && (
				e.State == EntityState.Added
				|| e.State == EntityState.Modified));

		foreach (var entityEntry in entries)
		{
			var entity = (IEntity)entityEntry.Entity;
			entity.UpdatedAt = DateTime.UtcNow;
			entity.Concurrency = Guid.NewGuid().ToString();

			if (entityEntry.State == EntityState.Added)
			{
				entity.CreatedAt = DateTime.UtcNow;
			}
		}
	}
}
