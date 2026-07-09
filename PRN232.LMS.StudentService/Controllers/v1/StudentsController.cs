using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.StudentService.Helper;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.StudentService.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/students")]
    [Produces("application/json", "application/xml")]
    [Authorize]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentService _service;
        public StudentsController(IStudentService service) => _service = service;

        /// <summary>Lấy danh sách students (search, sort, paging, expand=enrollments)</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<StudentResponse>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParams q)
        {
            var result = await _service.GetAllAsync(q);
            if (result.Items != null)
            {
                foreach (var item in result.Items)
                {
                    item.StudentCode = null;
                }
            }
            return Ok(ApiResponse<PagedResult<StudentResponse>>.Ok(result));
        }

        /// <summary>Lấy student theo ID (kèm enrollments)</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), 404)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(ApiResponse<StudentResponse>.Fail("Student not found"));
            result.StudentCode = null;
            return Ok(ApiResponse<StudentResponse>.Ok(result));
        }

        /// <summary>Lấy danh sách enrollments của student (search, sort, paging)</summary>
        [HttpGet("{id:int}/enrollments")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentResponse>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentResponse>>), 404)]
        public async Task<IActionResult> GetEnrollmentsByStudentId([FromRoute] int id, [FromQuery] QueryParams q)
        {
            try
            {
                var result = await _service.GetEnrollmentsByStudentIdAsync(id, q);
                return Ok(ApiResponse<PagedResult<EnrollmentResponse>>.Ok(result));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<PagedResult<EnrollmentResponse>>.Fail("Student not found"));
            }
        }

        /// <summary>Lấy danh sách courses của student (search, sort, paging)</summary>
        [HttpGet("{id:int}/courses")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseResponse>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseResponse>>), 404)]
        public async Task<IActionResult> GetCoursesByStudentId([FromRoute] int id, [FromQuery] QueryParams q)
        {
            try
            {
                var result = await _service.GetCoursesByStudentIdAsync(id, q);
                return Ok(ApiResponse<PagedResult<CourseResponse>>.Ok(result));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<PagedResult<CourseResponse>>.Fail("Student not found"));
            }
        }

        /// <summary>Tạo student mới</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), 201)]
        public async Task<IActionResult> Create([FromBody] StudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<StudentResponse>.Fail("Invalid request", ModelState));
            var result = await _service.CreateAsync(request);
            result.StudentCode = null;
            return StatusCode(201, ApiResponse<StudentResponse>.Ok(result, "Student created"));
        }

        /// <summary>Cập nhật student</summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<StudentResponse>), 404)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] StudentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<StudentResponse>.Fail("Invalid request", ModelState));
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound(ApiResponse<StudentResponse>.Fail("Student not found"));
            result.StudentCode = null;
            return Ok(ApiResponse<StudentResponse>.Ok(result, "Student updated"));
        }

        /// <summary>Xoá student (Admin only)</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(ApiResponse<object>.Fail("Student not found"));
            return Ok(ApiResponse<object>.Ok(null!, "Student deleted"));
        }
    }
}
