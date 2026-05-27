using PRN232.LMS.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync();
        Task<Student?> GetByIdAsync(int id);
        Task<Student> CreateAsync(Student entity);
        Task<Student?> UpdateAsync(int id, Student entity);
        Task<bool> DeleteAsync(int id);
        IQueryable<Student> GetQueryable();
    }

}
