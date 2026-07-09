using Microsoft.EntityFrameworkCore;
using PRN232.LMS.CourseService.Protos;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;
using PRN232.LMS.Repositories.Entities;

namespace PRN232.LMS.CourseService.Services
{
    public class EnrollmentServiceWithGrpc : IEnrollmentService
    {
        private readonly IEnrollmentRepository _repo;
        private readonly ICourseRepository _courseRepo;
        private readonly StudentGrpc.StudentGrpcClient _studentGrpcClient;

        public EnrollmentServiceWithGrpc(IEnrollmentRepository repo, ICourseRepository courseRepo, StudentGrpc.StudentGrpcClient studentGrpcClient)
        {
            _repo = repo;
            _courseRepo = courseRepo;
            _studentGrpcClient = studentGrpcClient;
        }

        private static EnrollmentResponse Map(Enrollment e, bool includeStudent = false, bool includeCourse = false, string? fields = null) => 
            FieldFilterHelper.ApplyFieldFilter(new EnrollmentResponse()
            {
                EnrollmentId = e.EnrollmentId,
                StudentId = e.StudentId,
                CourseId = e.CourseId,
                EnrollDate = e.EnrollDate,
                Status = e.Status,
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
                query = query.Where(e => e.Status.Contains(q.Search) || e.Course.CourseName.Contains(q.Search));

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

            var resultItems = items.Select(e => Map(e, expandStudent, expandCourse, q.Fields)).ToList();

            if (expandStudent && resultItems.Any())
            {
                var studentIds = resultItems.Select(r => r.StudentId).Distinct().ToList();
                try
                {
                    var grpcReq = new GetStudentsRequest();
                    grpcReq.StudentIds.AddRange(studentIds);
                    var grpcRes = await _studentGrpcClient.GetStudentsAsync(grpcReq);
                    var studentMap = grpcRes.Students.ToDictionary(s => s.StudentId);

                    foreach (var item in resultItems)
                    {
                        if (studentMap.TryGetValue(item.StudentId, out var s))
                        {
                            item.Student = new StudentResponse
                            {
                                StudentId = s.StudentId,
                                FullName = s.FullName,
                                Email = s.Email,
                                DateOfBirth = DateTime.Parse(s.DateOfBirth),
                                StudentCode = s.StudentCode
                            };
                        }
                    }
                }
                catch
                {
                    // Fail gracefully or log if StudentService is offline
                }
            }

            return new PagedResult<EnrollmentResponse>
            {
                Items = resultItems,
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
            if (e == null) return null;

            var response = Map(e, includeStudent: true, includeCourse: true);
            try
            {
                var studentRes = await _studentGrpcClient.GetStudentByIdAsync(new GetStudentRequest { StudentId = e.StudentId });
                response.Student = new StudentResponse
                {
                    StudentId = studentRes.StudentId,
                    FullName = studentRes.FullName,
                    Email = studentRes.Email,
                    DateOfBirth = DateTime.Parse(studentRes.DateOfBirth),
                    StudentCode = studentRes.StudentCode
                };
            }
            catch
            {
                // Leave student null if offline or not found
            }
            return response;
        }

        public async Task<EnrollmentResponse> CreateAsync(EnrollmentRequest req)
        {
            // Verify student exists over gRPC
            try
            {
                await _studentGrpcClient.GetStudentByIdAsync(new GetStudentRequest { StudentId = req.StudentId });
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Student with ID {req.StudentId} not found");
            }

            var course = await _courseRepo.GetByIdAsync(req.CourseId);
            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {req.CourseId} not found");
            }

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
            // Verify student exists over gRPC
            try
            {
                await _studentGrpcClient.GetStudentByIdAsync(new GetStudentRequest { StudentId = req.StudentId });
            }
            catch (Grpc.Core.RpcException ex) when (ex.StatusCode == Grpc.Core.StatusCode.NotFound)
            {
                throw new KeyNotFoundException($"Student with ID {req.StudentId} not found");
            }

            var course = await _courseRepo.GetByIdAsync(req.CourseId);
            if (course == null)
            {
                throw new KeyNotFoundException($"Course with ID {req.CourseId} not found");
            }

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
