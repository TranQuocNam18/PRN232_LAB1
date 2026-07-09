using Grpc.Core;
using Microsoft.EntityFrameworkCore;
using PRN232.LMS.CourseService.Protos;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Models.Responses;
using System.Linq;

namespace PRN232.LMS.CourseService.Services
{
    public class CourseGrpcService : CourseGrpc.CourseGrpcBase
    {
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly ICourseRepository _courseRepo;

        public CourseGrpcService(IEnrollmentRepository enrollmentRepo, ICourseRepository courseRepo)
        {
            _enrollmentRepo = enrollmentRepo;
            _courseRepo = courseRepo;
        }

        public override async Task<GetEnrollmentsResponse> GetEnrollmentsByStudentId(GetEnrollmentsRequest request, ServerCallContext context)
        {
            var query = _enrollmentRepo.GetQueryable()
                .Where(e => e.StudentId == request.StudentId);

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(e => e.Status.Contains(request.Search) || e.Course.CourseName.Contains(request.Search));
            }

            query = request.Sort switch
            {
                "enrollDate" => query.OrderBy(e => e.EnrollDate),
                "-enrollDate" => query.OrderByDescending(e => e.EnrollDate),
                "status" => query.OrderBy(e => e.Status),
                "-status" => query.OrderByDescending(e => e.Status),
                _ => query.OrderBy(e => e.EnrollmentId)
            };

            int total = await query.CountAsync();
            var items = await query.Skip((request.Page - 1) * request.Size).Take(request.Size).ToListAsync();

            var response = new GetEnrollmentsResponse
            {
                TotalItems = total,
                Page = request.Page,
                Size = request.Size
            };

            response.Items.AddRange(items.Select(e => new EnrollmentGrpcDto
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate.ToString("o"),
                Status = e.Status,
                Course = e.Course != null ? new CourseGrpcDto
                {
                    CourseId = e.Course.CourseId,
                    CourseName = e.Course.CourseName,
                    SemesterId = e.Course.SemesterId,
                    SubjectId = e.Course.SubjectId ?? 0,
                    Semester = e.Course.Semester != null ? new SemesterGrpcDto
                    {
                        SemesterId = e.Course.Semester.SemesterId,
                        SemesterName = e.Course.Semester.SemesterName,
                        StartDate = e.Course.Semester.StartDate.ToString("o"),
                        EndDate = e.Course.Semester.EndDate.ToString("o")
                    } : null,
                    Subject = e.Course.Subject != null ? new SubjectGrpcDto
                    {
                        SubjectId = e.Course.Subject.SubjectId,
                        SubjectCode = e.Course.Subject.SubjectCode,
                        SubjectName = e.Course.Subject.SubjectName,
                        Credit = e.Course.Subject.Credit
                    } : null
                } : null
            }));

            return response;
        }

        public override async Task<GetCoursesResponse> GetCoursesByStudentId(GetCoursesRequest request, ServerCallContext context)
        {
            var query = _enrollmentRepo.GetQueryable()
                .Where(e => e.StudentId == request.StudentId)
                .Select(e => e.Course)
                .Distinct();

            if (!string.IsNullOrWhiteSpace(request.Search))
            {
                query = query.Where(c => c.CourseName.Contains(request.Search));
            }

            query = request.Sort switch
            {
                "courseName" => query.OrderBy(c => c.CourseName),
                "-courseName" => query.OrderByDescending(c => c.CourseName),
                "courseId" => query.OrderBy(c => c.CourseId),
                "-courseId" => query.OrderByDescending(c => c.CourseId),
                _ => query.OrderBy(c => c.CourseId)
            };

            int total = await query.CountAsync();
            var items = await query.Skip((request.Page - 1) * request.Size).Take(request.Size).ToListAsync();

            var response = new GetCoursesResponse
            {
                TotalItems = total,
                Page = request.Page,
                Size = request.Size
            };

            response.Items.AddRange(items.Select(c => new CourseGrpcDto
            {
                CourseId = c.CourseId,
                CourseName = c.CourseName,
                SemesterId = c.SemesterId,
                SubjectId = c.SubjectId ?? 0,
                Semester = c.Semester != null ? new SemesterGrpcDto
                {
                    SemesterId = c.Semester.SemesterId,
                    SemesterName = c.Semester.SemesterName,
                    StartDate = c.Semester.StartDate.ToString("o"),
                    EndDate = c.Semester.EndDate.ToString("o")
                } : null,
                Subject = c.Subject != null ? new SubjectGrpcDto
                {
                    SubjectId = c.Subject.SubjectId,
                    SubjectCode = c.Subject.SubjectCode,
                    SubjectName = c.Subject.SubjectName,
                    Credit = c.Subject.Credit
                } : null
            }));

            return response;
        }
    }
}
