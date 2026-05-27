using PRN232.LMS.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int id);
        Task<Course> CreateAsync(Course entity);
        Task<Course?> UpdateAsync(int id, Course entity);
        Task<bool> DeleteAsync(int id);
        IQueryable<Course> GetQueryable();
    }

}
