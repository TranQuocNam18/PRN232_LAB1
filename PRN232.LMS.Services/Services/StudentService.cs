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
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repo;
        public StudentService(IStudentRepository repo) => _repo = repo;

        private static StudentResponse Map(Student s, bool includeEnrollments = false, string? fields = null) => 
            FieldFilterHelper.ApplyFieldFilter(new StudentResponse()
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth,
                StudentCode = s.StudentCode,
                Enrollments = includeEnrollments
                    ? s.Enrollments.Select(e => new EnrollmentResponse
                    {
                        EnrollmentId = e.EnrollmentId,
                        StudentId = e.StudentId,
                        CourseId = e.CourseId,
                        EnrollDate = e.EnrollDate,
                        Status = e.Status,
                        Course = e.Course != null ? new CourseResponse { CourseId = e.Course.CourseId, CourseName = e.Course.CourseName, SemesterId = e.Course.SemesterId } : null
                    }).ToList()
                    : null
            }, fields)!;

        public async Task<PagedResult<StudentResponse>> GetAllAsync(QueryParams q)
        {
            var query = _repo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(s => s.FullName.Contains(q.Search) || s.Email.Contains(q.Search));

            bool expandEnrollments = q.Expand?.Contains("enrollments", StringComparison.OrdinalIgnoreCase) == true;
            if (expandEnrollments)
                query = query.Include(s => s.Enrollments).ThenInclude(e => e.Course);

            query = q.Sort switch
            {
                "fullName" => query.OrderBy(s => s.FullName),
                "-fullName" => query.OrderByDescending(s => s.FullName),
                "dateOfBirth" => query.OrderBy(s => s.DateOfBirth),
                "-dateOfBirth" => query.OrderByDescending(s => s.DateOfBirth),
                "email" => query.OrderBy(s => s.Email),
                "-email" => query.OrderByDescending(s => s.Email),
                _ => query.OrderBy(s => s.StudentId)
            };

            int total = await query.CountAsync();
            var items = await query.Skip((q.Page - 1) * q.Size).Take(q.Size).ToListAsync();

            return new PagedResult<StudentResponse>
            {
                Items = items.Select(s => Map(s, expandEnrollments, q.Fields)),
                Pagination = new PaginationMeta
                {
                    Page = q.Page,
                    PageSize = q.Size,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling(total / (double)q.Size)
                }
            };
        }

        public async Task<StudentResponse?> GetByIdAsync(int id)
        {
            var s = await _repo.GetByIdAsync(id);
            return s == null ? null : Map(s, includeEnrollments: true);
        }

        public async Task<StudentResponse> CreateAsync(StudentRequest req)
        {
            var entity = new Student 
            { 
                FullName = req.FullName, 
                Email = req.Email, 
                DateOfBirth = req.DateOfBirth,
                StudentCode = req.StudentCode
            };
            return Map(await _repo.CreateAsync(entity));
        }

        public async Task<StudentResponse?> UpdateAsync(int id, StudentRequest req)
        {
            var updated = await _repo.UpdateAsync(id, new Student 
            { 
                FullName = req.FullName, 
                Email = req.Email, 
                DateOfBirth = req.DateOfBirth,
                StudentCode = req.StudentCode
            });
            return updated == null ? null : Map(updated);
        }

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

        public async Task<PagedResult<EnrollmentResponse>> GetEnrollmentsByStudentIdAsync(int studentId, QueryParams q)
        {
            var student = await _repo.GetByIdAsync(studentId);
            if (student == null)
                throw new KeyNotFoundException($"Student with ID {studentId} not found");

            var query = student.Enrollments.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(e => e.Status.Contains(q.Search));

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

        public async Task<PagedResult<CourseResponse>> GetCoursesByStudentIdAsync(int studentId, QueryParams q)
        {
            var student = await _repo.GetByIdAsync(studentId);
            if (student == null)
                throw new KeyNotFoundException($"Student with ID {studentId} not found");

            var query = student.Enrollments
                .Select(e => e.Course)
                .Distinct()
                .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(c => c.CourseName.Contains(q.Search));

            bool expandSemester = q.Expand?.Contains("semester", StringComparison.OrdinalIgnoreCase) == true;
            bool expandSubject = q.Expand?.Contains("subject", StringComparison.OrdinalIgnoreCase) == true;

            // Sort
            query = q.Sort switch
            {
                "courseName" => query.OrderBy(c => c.CourseName),
                "-courseName" => query.OrderByDescending(c => c.CourseName),
                "courseId" => query.OrderBy(c => c.CourseId),
                "-courseId" => query.OrderByDescending(c => c.CourseId),
                _ => query.OrderBy(c => c.CourseId)
            };

            int total = query.Count();
            var items = query.Skip((q.Page - 1) * q.Size).Take(q.Size).ToList();

            return new PagedResult<CourseResponse>
            {
                Items = items.Select(c => new CourseResponse 
                { 
                    CourseId = c.CourseId, 
                    CourseName = c.CourseName, 
                    SemesterId = c.SemesterId,
                    SubjectId = c.SubjectId,
                    Semester = expandSemester && c.Semester != null
                        ? new SemesterResponse { SemesterId = c.Semester.SemesterId, SemesterName = c.Semester.SemesterName, StartDate = c.Semester.StartDate, EndDate = c.Semester.EndDate }
                        : null,
                    Subject = expandSubject && c.Subject != null
                        ? new SubjectResponse { SubjectId = c.Subject.SubjectId, SubjectCode = c.Subject.SubjectCode, SubjectName = c.Subject.SubjectName, Credit = c.Subject.Credit }
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
    }

}
