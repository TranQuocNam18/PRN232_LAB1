using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.StudentService.Data
{
    public class StudentDbContext : LmsDbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<User>();
            modelBuilder.Ignore<RefreshToken>();
            modelBuilder.Ignore<Course>();
            modelBuilder.Ignore<Enrollment>();
            modelBuilder.Ignore<Semester>();
            modelBuilder.Ignore<Subject>();
        }
    }
}
