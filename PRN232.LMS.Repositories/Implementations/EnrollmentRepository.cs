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
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly LmsDbContext _ctx;
        public EnrollmentRepository(LmsDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Enrollment>> GetAllAsync() =>
            await _ctx.Enrollments.Include(e => e.Student)
                                   .Include(e => e.Course)
                                   .ToListAsync();

        public async Task<Enrollment?> GetByIdAsync(int id) =>
            await _ctx.Enrollments.AsSplitQuery()
                                   .Include(e => e.Student)
                                   .Include(e => e.Course)
                                   .ThenInclude(c => c.Semester)
                                   .FirstOrDefaultAsync(e => e.EnrollmentId == id);

        public async Task<Enrollment> CreateAsync(Enrollment entity)
        {
            _ctx.Enrollments.Add(entity);
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<Enrollment?> UpdateAsync(int id, Enrollment entity)
        {
            var existing = await _ctx.Enrollments.FindAsync(id);
            if (existing == null) return null;
            existing.StudentId = entity.StudentId;
            existing.CourseId = entity.CourseId;
            existing.EnrollDate = entity.EnrollDate;
            existing.Status = entity.Status;
            await _ctx.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _ctx.Enrollments.FindAsync(id);
            if (existing == null) return false;
            _ctx.Enrollments.Remove(existing);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public IQueryable<Enrollment> GetQueryable() =>
            _ctx.Enrollments.Include(e => e.Student)
                            .Include(e => e.Course)
                            .ThenInclude(c => c.Semester)
                            .AsQueryable();
    }

}
