using PRN232.LMS.StudentService.Protos;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.StudentService.Services
{
    public class StudentServiceWithGrpc : IStudentService
    {
        private readonly PRN232.LMS.Services.Services.StudentService _innerService;
        private readonly CourseGrpc.CourseGrpcClient _courseGrpcClient;
        private readonly IStudentRepository _repo;

        public StudentServiceWithGrpc(IStudentRepository repo, CourseGrpc.CourseGrpcClient courseGrpcClient)
        {
            _repo = repo;
            _courseGrpcClient = courseGrpcClient;
            _innerService = new PRN232.LMS.Services.Services.StudentService(repo);
        }

        public Task<PagedResult<StudentResponse>> GetAllAsync(QueryParams q) => _innerService.GetAllAsync(q);
        public Task<StudentResponse?> GetByIdAsync(int id) => _innerService.GetByIdAsync(id);
        public Task<StudentResponse> CreateAsync(StudentRequest req) => _innerService.CreateAsync(req);
        public Task<StudentResponse?> UpdateAsync(int id, StudentRequest req) => _innerService.UpdateAsync(id, req);
        public Task<bool> DeleteAsync(int id) => _innerService.DeleteAsync(id);

        public async Task<PagedResult<EnrollmentResponse>> GetEnrollmentsByStudentIdAsync(int studentId, QueryParams q)
        {
            var student = await _repo.GetByIdAsync(studentId);
            if (student == null)
                throw new KeyNotFoundException($"Student with ID {studentId} not found");

            var grpcReq = new GetEnrollmentsRequest
            {
                StudentId = studentId,
                Search = q.Search ?? "",
                Sort = q.Sort ?? "",
                Page = q.Page,
                Size = q.Size,
                Expand = q.Expand ?? ""
            };

            var response = await _courseGrpcClient.GetEnrollmentsByStudentIdAsync(grpcReq);

            return new PagedResult<EnrollmentResponse>
            {
                Items = response.Items.Select(e => new EnrollmentResponse
                {
                    EnrollmentId = e.EnrollmentId,
                    StudentId = e.StudentId,
                    CourseId = e.CourseId,
                    EnrollDate = DateTime.Parse(e.EnrollDate),
                    Status = e.Status,
                    Course = e.Course != null ? new CourseResponse
                    {
                        CourseId = e.Course.CourseId,
                        CourseName = e.Course.CourseName,
                        SemesterId = e.Course.SemesterId,
                        SubjectId = e.Course.SubjectId,
                        Semester = e.Course.Semester != null ? new SemesterResponse
                        {
                            SemesterId = e.Course.Semester.SemesterId,
                            SemesterName = e.Course.Semester.SemesterName,
                            StartDate = DateTime.Parse(e.Course.Semester.StartDate),
                            EndDate = DateTime.Parse(e.Course.Semester.EndDate)
                        } : null,
                        Subject = e.Course.Subject != null ? new SubjectResponse
                        {
                            SubjectId = e.Course.Subject.SubjectId,
                            SubjectCode = e.Course.Subject.SubjectCode,
                            SubjectName = e.Course.Subject.SubjectName,
                            Credit = e.Course.Subject.Credit
                        } : null
                    } : null
                }).ToList(),
                Pagination = new PaginationMeta
                {
                    Page = response.Page,
                    PageSize = response.Size,
                    TotalItems = response.TotalItems,
                    TotalPages = (int)Math.Ceiling(response.TotalItems / (double)response.Size)
                }
            };
        }

        public async Task<PagedResult<CourseResponse>> GetCoursesByStudentIdAsync(int studentId, QueryParams q)
        {
            var student = await _repo.GetByIdAsync(studentId);
            if (student == null)
                throw new KeyNotFoundException($"Student with ID {studentId} not found");

            var grpcReq = new GetCoursesRequest
            {
                StudentId = studentId,
                Search = q.Search ?? "",
                Sort = q.Sort ?? "",
                Page = q.Page,
                Size = q.Size,
                Expand = q.Expand ?? ""
            };

            var response = await _courseGrpcClient.GetCoursesByStudentIdAsync(grpcReq);

            return new PagedResult<CourseResponse>
            {
                Items = response.Items.Select(c => new CourseResponse
                {
                    CourseId = c.CourseId,
                    CourseName = c.CourseName,
                    SemesterId = c.SemesterId,
                    SubjectId = c.SubjectId,
                    Semester = c.Semester != null ? new SemesterResponse
                    {
                        SemesterId = c.Semester.SemesterId,
                        SemesterName = c.Semester.SemesterName,
                        StartDate = DateTime.Parse(c.Semester.StartDate),
                        EndDate = DateTime.Parse(c.Semester.EndDate)
                    } : null,
                    Subject = c.Subject != null ? new SubjectResponse
                    {
                        SubjectId = c.Subject.SubjectId,
                        SubjectCode = c.Subject.SubjectCode,
                        SubjectName = c.Subject.SubjectName,
                        Credit = c.Subject.Credit
                    } : null
                }).ToList(),
                Pagination = new PaginationMeta
                {
                    Page = response.Page,
                    PageSize = response.Size,
                    TotalItems = response.TotalItems,
                    TotalPages = (int)Math.Ceiling(response.TotalItems / (double)response.Size)
                }
            };
        }
    }
}
