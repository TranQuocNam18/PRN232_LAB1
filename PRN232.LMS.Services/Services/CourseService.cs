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
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _repo;
        public CourseService(ICourseRepository repo) => _repo = repo;

        private static CourseResponse Map(Course c, bool includeSemester = false, bool includeSubject = false, string? fields = null) => 
            FieldFilterHelper.ApplyFieldFilter(new CourseResponse()
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SemesterId = c.SemesterId,
                SubjectId = c.SubjectId,
                Semester = includeSemester && c.Semester != null
                    ? new SemesterResponse { SemesterId = c.Semester.SemesterId, SemesterName = c.Semester.SemesterName, StartDate = c.Semester.StartDate, EndDate = c.Semester.EndDate }
                    : null,
                Subject = includeSubject && c.Subject != null
                    ? new SubjectResponse { SubjectId = c.Subject.SubjectId, SubjectCode = c.Subject.SubjectCode, SubjectName = c.Subject.SubjectName, Credit = c.Subject.Credit }
                    : null
            }, fields)!;

        public async Task<PagedResult<CourseResponse>> GetAllAsync(QueryParams q)
        {
            var query = _repo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(c => c.CourseName.Contains(q.Search));

            bool expandSemester = q.Expand?.Contains("semester", StringComparison.OrdinalIgnoreCase) == true;
            bool expandSubject = q.Expand?.Contains("subject", StringComparison.OrdinalIgnoreCase) == true;
            if (expandSubject) query = query.Include(c => c.Subject);

            query = q.Sort switch
            {
                "courseName" => query.OrderBy(c => c.CourseName),
                "-courseName" => query.OrderByDescending(c => c.CourseName),
                "semesterId" => query.OrderBy(c => c.SemesterId),
                "-semesterId" => query.OrderByDescending(c => c.SemesterId),
                _ => query.OrderBy(c => c.CourseId)
            };

            int total = await query.CountAsync();
            var items = await query.Skip((q.Page - 1) * q.Size).Take(q.Size).ToListAsync();

            return new PagedResult<CourseResponse>
            {
                Items = items.Select(c => Map(c, expandSemester, expandSubject, q.Fields)),
                Pagination = new PaginationMeta
                {
                    Page = q.Page,
                    PageSize = q.Size,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling(total / (double)q.Size)
                }
            };
        }

        public async Task<CourseResponse?> GetByIdAsync(int id)
        {
            var c = await _repo.GetByIdAsync(id);
            if (c == null) return null;
            // Load subject if not already loaded
            if (c.Subject == null && c.SubjectId.HasValue)
            {
                // Subject should be loaded, but if not, we can handle it
            }
            return Map(c, includeSemester: true, includeSubject: true);
        }

        public async Task<CourseResponse> CreateAsync(CourseRequest req)
        {
            var entity = new Course { CourseName = req.CourseName, SemesterId = req.SemesterId, SubjectId = req.SubjectId };
            var created = await _repo.CreateAsync(entity);
            return Map(created);
        }

        public async Task<CourseResponse?> UpdateAsync(int id, CourseRequest req)
        {
            var updated = await _repo.UpdateAsync(id, new Course { CourseName = req.CourseName, SemesterId = req.SemesterId, SubjectId = req.SubjectId });
            return updated == null ? null : Map(updated);
        }

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

        public async Task<PagedResult<EnrollmentResponse>> GetEnrollmentsByCourseIdAsync(int courseId, QueryParams q)
        {
            var course = await _repo.GetByIdAsync(courseId);
            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found");

            var query = course.Enrollments.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(e => e.Status.Contains(q.Search));

            bool expandStudent = q.Expand?.Contains("student", StringComparison.OrdinalIgnoreCase) == true;
            bool expandCourse = q.Expand?.Contains("course", StringComparison.OrdinalIgnoreCase) == true;

            // Sort
            query = q.Sort switch
            {
                "enrollDate" => query.OrderBy(e => e.EnrollDate),
                "-enrollDate" => query.OrderByDescending(e => e.EnrollDate),
                "status" => query.OrderBy(e => e.Status),
                "-status" => query.OrderByDescending(e => e.Status),
                _ => query.OrderBy(e => e.EnrollmentId)
            };

            int total = query.Count();
            var items = query.Skip((q.Page - 1) * q.Size).Take(q.Size).ToList();

            return new PagedResult<EnrollmentResponse>
            {
                Items = items.Select(e => new EnrollmentResponse 
                { 
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = e.EnrollDate,
                    Status = e.Status,
                    Student = expandStudent && e.Student != null
                        ? new StudentResponse { StudentId = e.Student.StudentId, FullName = e.Student.FullName, Email = e.Student.Email, DateOfBirth = e.Student.DateOfBirth }
                        : null,
                    Course = expandCourse && e.Course != null
                        ? new CourseResponse 
                        { 
                            CourseId = e.Course.CourseId,
                            CourseName = e.Course.CourseName,
                            SemesterId = e.Course.SemesterId,
                            SubjectId = e.Course.SubjectId,
                            Semester = e.Course.Semester != null
                                ? new SemesterResponse { SemesterId = e.Course.Semester.SemesterId, SemesterName = e.Course.Semester.SemesterName, StartDate = e.Course.Semester.StartDate, EndDate = e.Course.Semester.EndDate }
                                : null,
                            Subject = e.Course.Subject != null
                                ? new SubjectResponse { SubjectId = e.Course.Subject.SubjectId, SubjectCode = e.Course.Subject.SubjectCode, SubjectName = e.Course.Subject.SubjectName, Credit = e.Course.Subject.Credit }
                                : null
                        }
                        : null
                }),
                Pagination = new PaginationMeta
                {
                    Page = q.Page,
                    PageSize = q.Size,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling(total / (double)q.Size)
                }
            };
        }

        public async Task<PagedResult<SubjectResponse>> GetSubjectsByCourseIdAsync(int courseId, QueryParams q)
        {
            var course = await _repo.GetByIdAsync(courseId);
            if (course == null)
                throw new KeyNotFoundException($"Course with ID {courseId} not found");

            var subjects = new List<Subject>();
            if (course.SubjectId.HasValue && course.Subject != null)
                subjects.Add(course.Subject);

            var query = subjects.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(s => s.SubjectName.Contains(q.Search) || s.SubjectCode.Contains(q.Search));

            // Sort
            query = q.Sort switch
            {
                "subjectName" => query.OrderBy(s => s.SubjectName),
                "-subjectName" => query.OrderByDescending(s => s.SubjectName),
                "subjectCode" => query.OrderBy(s => s.SubjectCode),
                "-subjectCode" => query.OrderByDescending(s => s.SubjectCode),
                "credit" => query.OrderBy(s => s.Credit),
                "-credit" => query.OrderByDescending(s => s.Credit),
                _ => query.OrderBy(s => s.SubjectId)
            };

            int total = query.Count();
            var items = query.Skip((q.Page - 1) * q.Size).Take(q.Size).ToList();

            return new PagedResult<SubjectResponse>
            {
                Items = items.Select(s => new SubjectResponse 
                { 
                    SubjectId = s.SubjectId,
                    SubjectCode = s.SubjectCode,
                    SubjectName = s.SubjectName,
                    Credit = s.Credit
                }),
                Pagination = new PaginationMeta
                {
                    Page = q.Page,
                    PageSize = q.Size,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling(total / (double)q.Size)
                }
            };
        }
    }

}
