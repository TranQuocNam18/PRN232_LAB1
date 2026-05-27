using PRN232.LMS.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Repositories.Interfaces
{
    public interface ISubjectRepository
    {
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(int id);
        Task<Subject> CreateAsync(Subject entity);
        Task<Subject?> UpdateAsync(int id, Subject entity);
        Task<bool> DeleteAsync(int id);
        IQueryable<Subject> GetQueryable();
    }

}
