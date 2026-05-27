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
    public class StudentRepository : IStudentRepository
    {
        private readonly LmsDbContext _ctx;
        public StudentRepository(LmsDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Student>> GetAllAsync() =>
            await _ctx.Students.ToListAsync();

        public async Task<Student?> GetByIdAsync(int id) =>
            await _ctx.Students.Include(s => s.Enrollments)
                                .ThenInclude(e => e.Course)
                                .ThenInclude(c => c.Semester)
                                .FirstOrDefaultAsync(s => s.StudentId == id);

        public async Task<Student> CreateAsync(Student entity)
        {
            _ctx.Students.Add(entity);
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<Student?> UpdateAsync(int id, Student entity)
        {
            var existing = await _ctx.Students.FindAsync(id);
            if (existing == null) return null;
            existing.FullName = entity.FullName;
            existing.Email = entity.Email;
            existing.DateOfBirth = entity.DateOfBirth;
            await _ctx.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _ctx.Students.FindAsync(id);
            if (existing == null) return false;
            _ctx.Students.Remove(existing);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public IQueryable<Student> GetQueryable() => _ctx.Students.AsQueryable();
    }

}
