using HorsesForCourses.Core;
using Microsoft.EntityFrameworkCore;

namespace HorsesForCourses.WebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Coach> Coaches { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Session> Sessions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coach>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Coach>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Coach>()
                .Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Coach>()
                .Ignore(c => c.Availability);

            modelBuilder.Entity<Course>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Course>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Course>()
                .Property(c => c.StartDate)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v));

            modelBuilder.Entity<Course>()
                .Property(c => c.EndDate)
                .HasConversion(
                    v => v.ToDateTime(TimeOnly.MinValue),
                    v => DateOnly.FromDateTime(v));

            modelBuilder.Entity<Course>()
                .Ignore(c => c.Timeslots)
                .Ignore(c => c.RequiredCompetences);

            modelBuilder.Entity<Session>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Session>()
                .OwnsOne(s => s.Timeslot, ts =>
                {
                    ts.Property(t => t.Day).IsRequired();
                    ts.Property(t => t.Start).IsRequired();
                    ts.Property(t => t.End).IsRequired();
                });

            modelBuilder.Owned<Timeslot>();
        }
    }
}