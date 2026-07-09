using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.IdentityService.Data
{
    public class IdentityDbContext : LmsDbContext
    {
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<Student>();
            modelBuilder.Ignore<Course>();
            modelBuilder.Ignore<Enrollment>();
            modelBuilder.Ignore<Semester>();
            modelBuilder.Ignore<Subject>();
        }
    }
}
