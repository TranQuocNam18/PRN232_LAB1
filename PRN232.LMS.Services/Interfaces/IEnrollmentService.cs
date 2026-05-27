using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<PagedResult<EnrollmentResponse>> GetAllAsync(QueryParams q);
        Task<EnrollmentResponse?> GetByIdAsync(int id);
        Task<EnrollmentResponse> CreateAsync(EnrollmentRequest request);
        Task<EnrollmentResponse?> UpdateAsync(int id, EnrollmentRequest request);
        Task<bool> DeleteAsync(int id);
    }

}
