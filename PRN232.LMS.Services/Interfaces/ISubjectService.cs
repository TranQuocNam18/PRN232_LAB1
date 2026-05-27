using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<PagedResult<SubjectResponse>> GetAllAsync(QueryParams q);
        Task<SubjectResponse?> GetByIdAsync(int id);
        Task<SubjectResponse> CreateAsync(SubjectRequest request);
        Task<SubjectResponse?> UpdateAsync(int id, SubjectRequest request);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<CourseResponse>> GetCoursesBySubjectIdAsync(int subjectId, QueryParams q);
    }

}
