using PRN232.LMS.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface ISemesterRepository
    {
        Task<IEnumerable<Semester>> GetAllAsync();
        Task<Semester?> GetByIdAsync(int id);
        Task<Semester> CreateAsync(Semester entity);
        Task<Semester?> UpdateAsync(int id, Semester entity);
        Task<bool> DeleteAsync(int id);
        IQueryable<Semester> GetQueryable();
    }

}
