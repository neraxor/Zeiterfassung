using Microsoft.EntityFrameworkCore;
using Zeiterfassung.Models;

namespace Zeiterfassung.Data
{
    public class MyDbContext : DbContext
    {
        public MyDbContext() : base()
        {
        }
        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WorkSession> WorkSessions { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Regulation> Regulations { get; set; }
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Location>()
                .HasOne(l => l.User)
                .WithMany(u => u.Locations)
                .HasForeignKey(l => l.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Project>()
                .HasOne(p => p.User)
                .WithMany(u => u.Projects)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}