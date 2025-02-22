using Microsoft.EntityFrameworkCore;
using FitAI.Models;

namespace FitAI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<WorkoutPlan> WorkoutPlans { get; set; }
        public DbSet<WorkoutExercise> WorkoutExercises { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasOne(u => u.UserProfile)
                .WithOne(up => up.User)
                .HasForeignKey<UserProfile>(up => up.UserID);

            modelBuilder.Entity<WorkoutPlan>()
                .HasOne(wp => wp.User)
                .WithMany()
                .HasForeignKey(wp => wp.UserID);

            modelBuilder.Entity<WorkoutExercise>()
                .HasOne(we => we.WorkoutPlan)
                .WithMany(wp => wp.Exercises)
                .HasForeignKey(we => we.WorkoutPlanId);
        }
    }
}