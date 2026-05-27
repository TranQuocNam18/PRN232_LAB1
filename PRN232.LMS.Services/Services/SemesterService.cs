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
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _repo;
        public SemesterService(ISemesterRepository repo) => _repo = repo;

        private static SemesterResponse Map(Semester s, bool includeCourses = false, string? fields = null) => 
            FieldFilterHelper.ApplyFieldFilter(new SemesterResponse()
            {
                SemesterId = s.SemesterId,
                SemesterName = s.SemesterName,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Courses = includeCourses
                    ? s.Courses.Select(c => new CourseResponse { CourseId = c.CourseId, CourseName = c.CourseName, SemesterId = c.SemesterId }).ToList()
                    : null
            }, fields)!;

        public async Task<PagedResult<SemesterResponse>> GetAllAsync(QueryParams q)
        {
            var query = _repo.GetQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(s => s.SemesterName.Contains(q.Search));

            // Expand
            bool expandCourses = q.Expand?.Contains("courses", StringComparison.OrdinalIgnoreCase) == true;
            if (expandCourses) query = query.Include(s => s.Courses);

            // Sort
            query = q.Sort switch
            {
                "semesterName" => query.OrderBy(s => s.SemesterName),
                "-semesterName" => query.OrderByDescending(s => s.SemesterName),
                "startDate" => query.OrderBy(s => s.StartDate),
                "-startDate" => query.OrderByDescending(s => s.StartDate),
                _ => query.OrderBy(s => s.SemesterId)
            };

            int total = await query.CountAsync();
            var items = await query.Skip((q.Page - 1) * q.Size).Take(q.Size).ToListAsync();

            return new PagedResult<SemesterResponse>
            {
                Items = items.Select(s => Map(s, expandCourses, q.Fields)),
                Pagination = new PaginationMeta
                {
                    Page = q.Page,
                    PageSize = q.Size,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling(total / (double)q.Size)
                }
            };
        }

        public async Task<SemesterResponse?> GetByIdAsync(int id)
        {
            var s = await _repo.GetByIdAsync(id);
            return s == null ? null : Map(s, includeCourses: true);
        }

        public async Task<SemesterResponse> CreateAsync(SemesterRequest req)
        {
            var entity = new Semester
            {
                SemesterName = req.SemesterName,
                StartDate = req.StartDate,
                EndDate = req.EndDate
            };
            var created = await _repo.CreateAsync(entity);
            return Map(created);
        }

        public async Task<SemesterResponse?> UpdateAsync(int id, SemesterRequest req)
        {
            var updated = await _repo.UpdateAsync(id, new Semester
            {
                SemesterName = req.SemesterName,
                StartDate = req.StartDate,
                EndDate = req.EndDate
            });
            return updated == null ? null : Map(updated);
        }

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

        public async Task<PagedResult<CourseResponse>> GetCoursesBySemesterIdAsync(int semesterId, QueryParams q)
        {
            var semester = await _repo.GetByIdAsync(semesterId);
            if (semester == null)
                throw new KeyNotFoundException($"Semester with ID {semesterId} not found");

            var query = semester.Courses.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(c => c.CourseName.Contains(q.Search));

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
                    Semester = new SemesterResponse 
                    { 
                        SemesterId = semester.SemesterId, 
                        SemesterName = semester.SemesterName, 
                        StartDate = semester.StartDate, 
                        EndDate = semester.EndDate 
                    },
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
