using Microsoft.EntityFrameworkCore;
using FitAI.Models;
using System.Collections.Generic;

namespace FitAI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }

    }
}
