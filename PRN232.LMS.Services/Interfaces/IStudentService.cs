using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IStudentService
    {
        Task<PagedResult<StudentResponse>> GetAllAsync(QueryParams q);
        Task<StudentResponse?> GetByIdAsync(int id);
        Task<StudentResponse> CreateAsync(StudentRequest request);
        Task<StudentResponse?> UpdateAsync(int id, StudentRequest request);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<EnrollmentResponse>> GetEnrollmentsByStudentIdAsync(int studentId, QueryParams q);
        Task<PagedResult<CourseResponse>> GetCoursesByStudentIdAsync(int studentId, QueryParams q);
    }

}
