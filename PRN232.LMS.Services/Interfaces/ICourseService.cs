using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface ICourseService
    {
        Task<PagedResult<CourseResponse>> GetAllAsync(QueryParams q);
        Task<CourseResponse?> GetByIdAsync(int id);
        Task<CourseResponse> CreateAsync(CourseRequest request);
        Task<CourseResponse?> UpdateAsync(int id, CourseRequest request);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<EnrollmentResponse>> GetEnrollmentsByCourseIdAsync(int courseId, QueryParams q);
        Task<PagedResult<SubjectResponse>> GetSubjectsByCourseIdAsync(int courseId, QueryParams q);
    }

}
