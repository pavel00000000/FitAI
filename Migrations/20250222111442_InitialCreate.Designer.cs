﻿// <auto-generated />
using System;
using FitAI.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FitAI.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250222111442_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("FitAI.Models.User", b =>
                {
                    b.Property<int>("UserID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("UserID");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FitAI.Models.UserProfile", b =>
                {
                    b.Property<int>("UserID")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Age")
                        .HasColumnType("INTEGER");

                    b.Property<string>("BodyType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int?>("Height")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("LevelOfPhysicalFitness")
                        .HasColumnType("INTEGER");

                    b.Property<string>("MainGoals")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Sex")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Weight")
                        .HasColumnType("INTEGER");

                    b.HasKey("UserID");

                    b.ToTable("UserProfiles");
                });

            modelBuilder.Entity("FitAI.Models.WorkoutExercise", b =>
                {
                    b.Property<int>("WorkoutExerciseId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Approaches")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ExerciseName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<string>("ExerciseType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Repetitions")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("Weight")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WorkoutPlanId")
                        .HasColumnType("INTEGER");

                    b.HasKey("WorkoutExerciseId");

                    b.HasIndex("WorkoutPlanId");

                    b.ToTable("WorkoutExercises");
                });

            modelBuilder.Entity("FitAI.Models.WorkoutPlan", b =>
                {
                    b.Property<int>("WorkoutPlanId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("LevelOfPhysicalFitness")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PlanName")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("TEXT");

                    b.Property<int>("UserID")
                        .HasColumnType("INTEGER");

                    b.HasKey("WorkoutPlanId");

                    b.HasIndex("UserID");

                    b.ToTable("WorkoutPlans");
                });

            modelBuilder.Entity("FitAI.Models.UserProfile", b =>
                {
                    b.HasOne("FitAI.Models.User", "User")
                        .WithOne("UserProfile")
                        .HasForeignKey("FitAI.Models.UserProfile", "UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FitAI.Models.WorkoutExercise", b =>
                {
                    b.HasOne("FitAI.Models.WorkoutPlan", "WorkoutPlan")
                        .WithMany("Exercises")
                        .HasForeignKey("WorkoutPlanId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("WorkoutPlan");
                });

            modelBuilder.Entity("FitAI.Models.WorkoutPlan", b =>
                {
                    b.HasOne("FitAI.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FitAI.Models.User", b =>
                {
                    b.Navigation("UserProfile")
                        .IsRequired();
                });

            modelBuilder.Entity("FitAI.Models.WorkoutPlan", b =>
                {
                    b.Navigation("Exercises");
                });
#pragma warning restore 612, 618
        }
    }
}
