using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.CourseService.Data
{
    public class CourseDbContext : LmsDbContext
    {
        public CourseDbContext(DbContextOptions<CourseDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<User>();
            modelBuilder.Ignore<RefreshToken>();
            modelBuilder.Ignore<Student>();
            modelBuilder.Entity<Enrollment>().Ignore(e => e.Student);
        }
    }
}
