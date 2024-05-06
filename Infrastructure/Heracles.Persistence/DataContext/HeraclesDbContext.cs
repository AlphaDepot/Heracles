using Heracles.Domain.EquipmentGroups.Models;
using Heracles.Domain.Equipments.Models;
using Heracles.Domain.ExerciseMuscleGroups.Models;
using Heracles.Domain.ExercisesType.Models;
using Heracles.Domain.MuscleFunctions.Models;
using Heracles.Domain.MuscleGroups.Models;
using Heracles.Domain.UserExerciseHistories.Models;
using Heracles.Domain.UserExercises.Models;
using Heracles.Domain.Users.Models;
using Heracles.Domain.WorkoutSessions.Models;
using Microsoft.EntityFrameworkCore;

namespace Heracles.Persistence.DataContext;

public class HeraclesDbContext(DbContextOptions<HeraclesDbContext> options) : DbContext(options)
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

        modelBuilder.Entity<Equipment>()
            .HasIndex(e => e.Type)
            .IsUnique();
        
        modelBuilder.Entity<WorkoutSession>()
            .HasIndex(ws => ws.Name)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.UserId)
            .IsUnique();

    }
}