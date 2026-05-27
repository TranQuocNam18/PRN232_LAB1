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
    public class SubjectRepository : ISubjectRepository
    {
        private readonly LmsDbContext _ctx;
        public SubjectRepository(LmsDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Subject>> GetAllAsync() =>
            await _ctx.Subjects.ToListAsync();

        public async Task<Subject?> GetByIdAsync(int id) =>
            await _ctx.Subjects.Include(s => s.Courses)
                               .ThenInclude(c => c.Semester)
                               .FirstOrDefaultAsync(s => s.SubjectId == id);

        public async Task<Subject> CreateAsync(Subject entity)
        {
            _ctx.Subjects.Add(entity);
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<Subject?> UpdateAsync(int id, Subject entity)
        {
            var existing = await _ctx.Subjects.FindAsync(id);
            if (existing == null) return null;
            existing.SubjectCode = entity.SubjectCode;
            existing.SubjectName = entity.SubjectName;
            existing.Credit = entity.Credit;
            await _ctx.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _ctx.Subjects.FindAsync(id);
            if (existing == null) return false;
            _ctx.Subjects.Remove(existing);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public IQueryable<Subject> GetQueryable() => _ctx.Subjects.AsQueryable();
    }

}
