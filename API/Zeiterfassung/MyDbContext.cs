using Microsoft.EntityFrameworkCore;
using Zeiterfassung.Models;

namespace Zeiterfassung.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<WorkSession> WorkSessions { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Regulation> Regulations { get; set; }
    }
}