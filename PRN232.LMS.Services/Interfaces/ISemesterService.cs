using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface ISemesterService
    {
        Task<PagedResult<SemesterResponse>> GetAllAsync(QueryParams q);
        Task<SemesterResponse?> GetByIdAsync(int id);
        Task<SemesterResponse> CreateAsync(SemesterRequest request);
        Task<SemesterResponse?> UpdateAsync(int id, SemesterRequest request);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<CourseResponse>> GetCoursesBySemesterIdAsync(int semesterId, QueryParams q);
    }

}
