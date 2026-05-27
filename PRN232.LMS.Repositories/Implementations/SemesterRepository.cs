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
    public class SemesterRepository : ISemesterRepository
    {
        private readonly LmsDbContext _ctx;
        public SemesterRepository(LmsDbContext ctx) => _ctx = ctx;

        public async Task<IEnumerable<Semester>> GetAllAsync() =>
            await _ctx.Semesters.ToListAsync();

        public async Task<Semester?> GetByIdAsync(int id) =>
            await _ctx.Semesters.Include(s => s.Courses)
                                .ThenInclude(c => c.Subject)
                                .FirstOrDefaultAsync(s => s.SemesterId == id);

        public async Task<Semester> CreateAsync(Semester entity)
        {
            _ctx.Semesters.Add(entity);
            await _ctx.SaveChangesAsync();
            return entity;
        }

        public async Task<Semester?> UpdateAsync(int id, Semester entity)
        {
            var existing = await _ctx.Semesters.FindAsync(id);
            if (existing == null) return null;
            existing.SemesterName = entity.SemesterName;
            existing.StartDate = entity.StartDate;
            existing.EndDate = entity.EndDate;
            await _ctx.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _ctx.Semesters.FindAsync(id);
            if (existing == null) return false;
            _ctx.Semesters.Remove(existing);
            await _ctx.SaveChangesAsync();
            return true;
        }

        public IQueryable<Semester> GetQueryable() => _ctx.Semesters.AsQueryable();
    }

}
