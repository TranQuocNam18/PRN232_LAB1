using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Data
{
    public class LmsDbContext : DbContext
    {
        public LmsDbContext(DbContextOptions<LmsDbContext> options) : base(options) { }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Semester>()
                .HasMany(s => s.Courses)
                .WithOne(c => c.Semester)
                .HasForeignKey(c => c.SemesterId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subject>()
                .HasMany(s => s.Courses)
                .WithOne(c => c.Subject)
                .HasForeignKey(c => c.SubjectId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Course>()
                .HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Student>()
                .HasMany(s => s.Enrollments)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<User>()
                .HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}