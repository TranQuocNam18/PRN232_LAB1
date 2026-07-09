using Grpc.Core;
using PRN232.LMS.Repositories.Interfaces;
using PRN232.LMS.StudentService.Protos;

namespace PRN232.LMS.StudentService.Services
{
    public class StudentGrpcService : StudentGrpc.StudentGrpcBase
    {
        private readonly IStudentRepository _repo;

        public StudentGrpcService(IStudentRepository repo)
        {
            _repo = repo;
        }

        public override async Task<StudentResponseGrpc> GetStudentById(GetStudentRequest request, ServerCallContext context)
        {
            var student = await _repo.GetByIdAsync(request.StudentId);
            if (student == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Student with ID {request.StudentId} not found"));
            }

            return new StudentResponseGrpc
            {
                StudentId = student.StudentId,
                FullName = student.FullName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth.ToString("o"),
                StudentCode = student.StudentCode ?? ""
            };
        }

        public override async Task<GetStudentsResponse> GetStudents(GetStudentsRequest request, ServerCallContext context)
        {
            var query = _repo.GetQueryable();
            var matchedStudents = query.Where(s => request.StudentIds.Contains(s.StudentId)).ToList();

            var response = new GetStudentsResponse();
            response.Students.AddRange(matchedStudents.Select(s => new StudentResponseGrpc
            {
                StudentId = s.StudentId,
                FullName = s.FullName,
                Email = s.Email,
                DateOfBirth = s.DateOfBirth.ToString("o"),
                StudentCode = s.StudentCode ?? ""
            }));

            return response;
        }
    }
}
