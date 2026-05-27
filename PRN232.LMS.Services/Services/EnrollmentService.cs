using Microsoft.EntityFrameworkCore;
using PRN232.LMS.Repositories.Entities;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repo;
        public EnrollmentService(IEnrollmentRepository repo) => _repo = repo;

        private static EnrollmentResponse Map(Enrollment e, bool includeStudent = false, bool includeCourse = false, string? fields = null) => 
            FieldFilterHelper.ApplyFieldFilter(new EnrollmentResponse()
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status,
                Student = includeStudent && e.Student != null
                    ? new StudentResponse { StudentId = e.Student.StudentId, FullName = e.Student.FullName, Email = e.Student.Email, DateOfBirth = e.Student.DateOfBirth }
                    : null,
                Course = includeCourse && e.Course != null
                    ? new CourseResponse
                    {
                        CourseId = e.Course.CourseId,
                        CourseName = e.Course.CourseName,
                        SemesterId = e.Course.SemesterId,
                        Semester = e.Course.Semester != null
                            ? new SemesterResponse { SemesterId = e.Course.Semester.SemesterId, SemesterName = e.Course.Semester.SemesterName, StartDate = e.Course.Semester.StartDate, EndDate = e.Course.Semester.EndDate }
                            : null
                    }
                    : null
            }, fields)!;

        public async Task<PagedResult<EnrollmentResponse>> GetAllAsync(QueryParams q)
        {
            var query = _repo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(e => e.Status.Contains(q.Search)
                    || e.Student.FullName.Contains(q.Search)
                    || e.Course.CourseName.Contains(q.Search));

            bool expandStudent = q.Expand?.Contains("student", StringComparison.OrdinalIgnoreCase) == true;
            bool expandCourse = q.Expand?.Contains("course", StringComparison.OrdinalIgnoreCase) == true;

            query = q.Sort switch
            {
                "enrollDate" => query.OrderBy(e => e.EnrollDate),
                "-enrollDate" => query.OrderByDescending(e => e.EnrollDate),
                "status" => query.OrderBy(e => e.Status),
                "-status" => query.OrderByDescending(e => e.Status),
                _ => query.OrderBy(e => e.EnrollmentId)
            };

            int total = await query.CountAsync();
            var items = await query.Skip((q.Page - 1) * q.Size).Take(q.Size).ToListAsync();

            return new PagedResult<EnrollmentResponse>
            {
                Items = items.Select(e => Map(e, expandStudent, expandCourse, q.Fields)),
                Pagination = new PaginationMeta
                {
                    Page = q.Page,
                    PageSize = q.Size,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling(total / (double)q.Size)
                }
            };
        }

        public async Task<EnrollmentResponse?> GetByIdAsync(int id)
        {
            var e = await _repo.GetByIdAsync(id);
            return e == null ? null : Map(e, includeStudent: true, includeCourse: true);
        }

        public async Task<EnrollmentResponse> CreateAsync(EnrollmentRequest req)
        {
            var entity = new Enrollment
            {
                StudentId = req.StudentId,
                CourseId = req.CourseId,
                EnrollDate = req.EnrollDate,
                Status = req.Status
            };
            return Map(await _repo.CreateAsync(entity));
        }

        public async Task<EnrollmentResponse?> UpdateAsync(int id, EnrollmentRequest req)
        {
            var updated = await _repo.UpdateAsync(id, new Enrollment
            {
                StudentId = req.StudentId,
                CourseId = req.CourseId,
                EnrollDate = req.EnrollDate,
                Status = req.Status
            });
            return updated == null ? null : Map(updated);
        }

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);
    }

}
