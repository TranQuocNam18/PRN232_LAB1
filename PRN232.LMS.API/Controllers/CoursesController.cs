using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helper;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
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

        /// <summary>Lấy course theo ID (kèm semester)</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), 404)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(ApiResponse<CourseResponse>.Fail("Course not found"));
            return Ok(ApiResponse<CourseResponse>.Ok(result));
        }

        /// <summary>Lấy danh sách enrollments của course (search, sort, paging)</summary>
        [HttpGet("{id}/enrollments")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentResponse>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentResponse>>), 404)]
        public async Task<IActionResult> GetEnrollmentsByCourseId(int id, [FromQuery] QueryParams q)
        {
            try
            {
                var result = await _service.GetEnrollmentsByCourseIdAsync(id, q);
                return Ok(ApiResponse<PagedResult<EnrollmentResponse>>.Ok(result));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<PagedResult<EnrollmentResponse>>.Fail("Course not found"));
            }
        }

        /// <summary>Lấy danh sách subjects của course (search, sort, paging)</summary>
        [HttpGet("{id}/subjects")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SubjectResponse>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SubjectResponse>>), 404)]
        public async Task<IActionResult> GetSubjectsByCourseId(int id, [FromQuery] QueryParams q)
        {
            try
            {
                var result = await _service.GetSubjectsByCourseIdAsync(id, q);
                return Ok(ApiResponse<PagedResult<SubjectResponse>>.Ok(result));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<PagedResult<SubjectResponse>>.Fail("Course not found"));
            }
        }

        /// <summary>Tạo course mới</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), 201)]
        public async Task<IActionResult> Create([FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CourseResponse>.Fail("Invalid request", ModelState));
            var result = await _service.CreateAsync(request);
            return StatusCode(201, ApiResponse<CourseResponse>.Ok(result, "Course created"));
        }

        /// <summary>Cập nhật course</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<CourseResponse>), 404)]
        public async Task<IActionResult> Update(int id, [FromBody] CourseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<CourseResponse>.Fail("Invalid request", ModelState));
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound(ApiResponse<CourseResponse>.Fail("Course not found"));
            return Ok(ApiResponse<CourseResponse>.Ok(result, "Course updated"));
        }

        /// <summary>Xoá course</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(ApiResponse<object>.Fail("Course not found"));
            return Ok(ApiResponse<object>.Ok(null!, "Course deleted"));
        }
    }
}
