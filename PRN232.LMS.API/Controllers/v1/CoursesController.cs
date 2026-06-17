using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helper;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.API.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/courses")]
    [Produces("application/json", "application/xml")]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _service;
        public CoursesController(ICourseService service) => _service = service;

        /// <summary>Lấy danh sách courses (search, sort, paging, expand=semester)</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseResponse>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParams q)
        {
            var result = await _service.GetAllAsync(q);
            return Ok(ApiResponse<PagedResult<CourseResponse>>.Ok(result));
        }

        /// <summary>Lấy course theo ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), 404)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(ApiResponse<CourseResponse>.Fail("Course not found"));
            return Ok(ApiResponse<CourseResponse>.Ok(result));
        }

        /// <summary>Lấy enrollments của course</summary>
        [HttpGet("{id:int}/enrollments")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentResponse>>), 200)]
        public async Task<IActionResult> GetEnrollmentsByCourseId([FromRoute] int id, [FromQuery] QueryParams q)
        {
            var result = await _service.GetEnrollmentsByCourseIdAsync(id, q);
            return Ok(ApiResponse<PagedResult<EnrollmentResponse>>.Ok(result));
        }

        /// <summary>Lấy subjects của course</summary>
        [HttpGet("{id:int}/subjects")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SubjectResponse>>), 200)]
        public async Task<IActionResult> GetSubjectsByCourseId([FromRoute] int id, [FromQuery] QueryParams q)
        {
            var result = await _service.GetSubjectsByCourseIdAsync(id, q);
            return Ok(ApiResponse<PagedResult<SubjectResponse>>.Ok(result));
        }

        /// <summary>Tạo course mới</summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), 201)]
        public async Task<IActionResult> Create([FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CourseResponse>.Fail("Invalid request", ModelState));
            var result = await _service.CreateAsync(request);
            return StatusCode(201, ApiResponse<CourseResponse>.Ok(result, "Course created"));
        }

        /// <summary>Cập nhật course</summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), 200)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CourseResponse>.Fail("Invalid request", ModelState));
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound(ApiResponse<CourseResponse>.Fail("Course not found"));
            return Ok(ApiResponse<CourseResponse>.Ok(result, "Course updated"));
        }

        /// <summary>Xoá course (Admin only)</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(ApiResponse<object>.Fail("Course not found"));
            return Ok(ApiResponse<object>.Ok(null!, "Course deleted"));
        }
    }
}
