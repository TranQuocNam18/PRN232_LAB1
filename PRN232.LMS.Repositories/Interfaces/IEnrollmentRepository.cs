using PRN232.LMS.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<IEnumerable<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int id);
        Task<Enrollment> CreateAsync(Enrollment entity);
        Task<Enrollment?> UpdateAsync(int id, Enrollment entity);
        Task<bool> DeleteAsync(int id);
        IQueryable<Enrollment> GetQueryable();
    }

}
