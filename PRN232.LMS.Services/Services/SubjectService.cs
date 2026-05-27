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
    public class SubjectService : ISubjectService
    {
        private readonly ISubjectRepository _repo;
        public SubjectService(ISubjectRepository repo) => _repo = repo;

        private static SubjectResponse Map(Subject s, string? fields = null) => 
            FieldFilterHelper.ApplyFieldFilter(new SubjectResponse()
            {
                SubjectId = s.SubjectId,
                SubjectCode = s.SubjectCode,
                SubjectName = s.SubjectName,
                Credit = s.Credit
            }, fields)!;

        public async Task<PagedResult<SubjectResponse>> GetAllAsync(QueryParams q)
        {
            var query = _repo.GetQueryable();

            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(s => s.SubjectName.Contains(q.Search) || s.SubjectCode.Contains(q.Search));

            query = q.Sort switch
            {
                "subjectName" => query.OrderBy(s => s.SubjectName),
                "-subjectName" => query.OrderByDescending(s => s.SubjectName),
                "credit" => query.OrderBy(s => s.Credit),
                "-credit" => query.OrderByDescending(s => s.Credit),
                _ => query.OrderBy(s => s.SubjectId)
            };

            int total = await query.CountAsync();
            var items = await query.Skip((q.Page - 1) * q.Size).Take(q.Size).ToListAsync();

            return new PagedResult<SubjectResponse>
            {
                Items = items.Select(s => Map(s, q.Fields)),
                Pagination = new PaginationMeta
                {
                    Page = q.Page,
                    PageSize = q.Size,
                    TotalItems = total,
                    TotalPages = (int)Math.Ceiling(total / (double)q.Size)
                }
            };
        }

        public async Task<SubjectResponse?> GetByIdAsync(int id)
        {
            var s = await _repo.GetByIdAsync(id);
            return s == null ? null : Map(s);
        }

        public async Task<SubjectResponse> CreateAsync(SubjectRequest req)
        {
            var entity = new Subject { SubjectCode = req.SubjectCode, SubjectName = req.SubjectName, Credit = req.Credit };
            return Map(await _repo.CreateAsync(entity));
        }

        public async Task<SubjectResponse?> UpdateAsync(int id, SubjectRequest req)
        {
            var updated = await _repo.UpdateAsync(id, new Subject { SubjectCode = req.SubjectCode, SubjectName = req.SubjectName, Credit = req.Credit });
            return updated == null ? null : Map(updated);
        }

        public Task<bool> DeleteAsync(int id) => _repo.DeleteAsync(id);

        public async Task<PagedResult<CourseResponse>> GetCoursesBySubjectIdAsync(int subjectId, QueryParams q)
        {
            var subject = await _repo.GetByIdAsync(subjectId);
            if (subject == null)
                throw new KeyNotFoundException($"Subject with ID {subjectId} not found");

            var query = subject.Courses.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(q.Search))
                query = query.Where(c => c.CourseName.Contains(q.Search));

            bool expandSemester = q.Expand?.Contains("semester", StringComparison.OrdinalIgnoreCase) == true;

            // Sort
            query = q.Sort switch
            {
                "courseName" => query.OrderBy(c => c.CourseName),
                "-courseName" => query.OrderByDescending(c => c.CourseName),
                "courseId" => query.OrderBy(c => c.CourseId),
                "-courseId" => query.OrderByDescending(c => c.CourseId),
                "semesterId" => query.OrderBy(c => c.SemesterId),
                "-semesterId" => query.OrderByDescending(c => c.SemesterId),
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
