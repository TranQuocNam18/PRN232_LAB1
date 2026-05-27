using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Data;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly LmsDbContext _ctx;
        public CourseRepository(LmsDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Course>> GetAllAsync() =>
            await _ctx.Courses.Include(c => c.Semester).ToListAsync();

        public async Task<Course?> GetByIdAsync(int id) =>
            await _ctx.Courses.Include(c => c.Semester)
                              .Include(c => c.Enrollments)
                              .ThenInclude(e => e.Student)
                              .Include(c => c.Subject)
                              .FirstOrDefaultAsync(c => c.CourseId == id);

        public async Task<Course> CreateAsync(Course entity)
        {
            _ctx.Courses.Add(entity);
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<Course?> UpdateAsync(int id, Course entity)
        {
            var existing = await _ctx.Courses.FindAsync(id);
            if (existing == null) return null;
            existing.CourseName = entity.CourseName;
            existing.SemesterId = entity.SemesterId;
            await _ctx.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _ctx.Courses.FindAsync(id);
            if (existing == null) return false;
            _ctx.Courses.Remove(existing);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public IQueryable<Course> GetQueryable() =>
            _ctx.Courses.Include(c => c.Semester).AsQueryable();
    }

}
