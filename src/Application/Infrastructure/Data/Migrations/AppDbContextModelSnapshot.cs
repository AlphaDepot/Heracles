﻿// <auto-generated />
using System;
using Application.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Application.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Application.Features.EquipmentGroups.EquipmentGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Concurrency")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("EquipmentGroups");
                });

            modelBuilder.Entity("Application.Features.Equipments.Equipment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Concurrency")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("EquipmentGroupId")
                        .HasColumnType("integer");

                    b.Property<double>("Resistance")
                        .HasColumnType("double precision");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("Weight")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("EquipmentGroupId");

                    b.HasIndex("Type")
                        .IsUnique();

                    b.ToTable("Equipments");
                });

            modelBuilder.Entity("Application.Features.ExerciseMuscleGroups.ExerciseMuscleGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Concurrency")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("ExerciseTypeId")
                        .HasColumnType("integer");

                    b.Property<int>("FunctionId")
                        .HasColumnType("integer");

                    b.Property<double>("FunctionPercentage")
                        .HasColumnType("double precision");

                    b.Property<int>("MuscleId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("ExerciseTypeId");

                    b.HasIndex("FunctionId");

                    b.HasIndex("MuscleId");

                    b.ToTable("ExerciseMuscleGroups");
                });

            modelBuilder.Entity("Application.Features.ExerciseTypes.ExerciseType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Concurrency")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .HasMaxLength(1000)
                        .HasColumnType("character varying(1000)");

                    b.Property<string>("ImageUrl")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("ExerciseTypes");
                });

            modelBuilder.Entity("Application.Features.MuscleFunctions.MuscleFunction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Concurrency")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("MuscleFunctions");
                });

            modelBuilder.Entity("Application.Features.MuscleGroups.MuscleGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Concurrency")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("MuscleGroups");
                });

            modelBuilder.Entity("Application.Features.UserExerciseHistories.UserExerciseHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Change")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Concurrency")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Repetition")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("UserExerciseId")
                        .HasColumnType("integer");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<double>("Weight")
                        .HasColumnType("double precision");

                    b.HasKey("Id");

                    b.HasIndex("UserExerciseId");

                    b.ToTable("UserExerciseHistories");
                });

            modelBuilder.Entity("Application.Features.UserExercises.UserExercise", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("BodyWeight")
                        .HasColumnType("boolean");

                    b.Property<string>("Concurrency")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double?>("CurrentWeight")
                        .HasColumnType("double precision");

                    b.Property<int>("DurationInSeconds")
                        .HasColumnType("integer");

                    b.Property<int?>("EquipmentGroupId")
                        .HasColumnType("integer");

                    b.Property<int>("ExerciseTypeId")
                        .HasColumnType("integer");

                    b.Property<double?>("PercentageResistance")
                        .HasColumnType("double precision");

                    b.Property<double?>("PersonalRecord")
                        .HasColumnType("double precision");

                    b.Property<int>("Repetitions")
                        .HasColumnType("integer");

                    b.Property<int>("Sets")
                        .HasColumnType("integer");

                    b.Property<int>("SortOrder")
                        .HasColumnType("integer");

                    b.Property<double?>("StaticResistance")
                        .HasColumnType("double precision");

                    b.Property<bool>("Timed")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<int>("Version")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("EquipmentGroupId");

                    b.HasIndex("ExerciseTypeId");

                    b.ToTable("UserExercises");
                });

            modelBuilder.Entity("Application.Features.Users.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Application.Features.WorkoutSessions.WorkoutSession", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Concurrency")
                        .HasMaxLength(36)
                        .HasColumnType("character varying(36)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<int?>("SortOrder")
                        .HasColumnType("integer");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.HasKey("Id");

                    b.ToTable("WorkoutSessions");
                });

            modelBuilder.Entity("UserExerciseWorkoutSession", b =>
                {
                    b.Property<int>("UserExercisesId")
                        .HasColumnType("integer");

                    b.Property<int>("WorkoutSessionsId")
                        .HasColumnType("integer");

                    b.HasKey("UserExercisesId", "WorkoutSessionsId");

                    b.HasIndex("WorkoutSessionsId");

                    b.ToTable("UserExerciseWorkoutSession");
                });

            modelBuilder.Entity("Application.Features.Equipments.Equipment", b =>
                {
                    b.HasOne("Application.Features.EquipmentGroups.EquipmentGroup", null)
                        .WithMany("Equipments")
                        .HasForeignKey("EquipmentGroupId");
                });

            modelBuilder.Entity("Application.Features.ExerciseMuscleGroups.ExerciseMuscleGroup", b =>
                {
                    b.HasOne("Application.Features.ExerciseTypes.ExerciseType", null)
                        .WithMany("MuscleGroups")
                        .HasForeignKey("ExerciseTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Application.Features.MuscleFunctions.MuscleFunction", "Function")
                        .WithMany()
                        .HasForeignKey("FunctionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Application.Features.MuscleGroups.MuscleGroup", "Muscle")
                        .WithMany()
                        .HasForeignKey("MuscleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Function");

                    b.Navigation("Muscle");
                });

            modelBuilder.Entity("Application.Features.UserExerciseHistories.UserExerciseHistory", b =>
                {
                    b.HasOne("Application.Features.UserExercises.UserExercise", null)
                        .WithMany("ExerciseHistories")
                        .HasForeignKey("UserExerciseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Application.Features.UserExercises.UserExercise", b =>
                {
                    b.HasOne("Application.Features.EquipmentGroups.EquipmentGroup", "EquipmentGroup")
                        .WithMany()
                        .HasForeignKey("EquipmentGroupId");

                    b.HasOne("Application.Features.ExerciseTypes.ExerciseType", "ExerciseType")
                        .WithMany()
                        .HasForeignKey("ExerciseTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("EquipmentGroup");

                    b.Navigation("ExerciseType");
                });

            modelBuilder.Entity("UserExerciseWorkoutSession", b =>
                {
                    b.HasOne("Application.Features.UserExercises.UserExercise", null)
                        .WithMany()
                        .HasForeignKey("UserExercisesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Application.Features.WorkoutSessions.WorkoutSession", null)
                        .WithMany()
                        .HasForeignKey("WorkoutSessionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Application.Features.EquipmentGroups.EquipmentGroup", b =>
                {
                    b.Navigation("Equipments");
                });

            modelBuilder.Entity("Application.Features.ExerciseTypes.ExerciseType", b =>
                {
                    b.Navigation("MuscleGroups");
                });

            modelBuilder.Entity("Application.Features.UserExercises.UserExercise", b =>
                {
                    b.Navigation("ExerciseHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
