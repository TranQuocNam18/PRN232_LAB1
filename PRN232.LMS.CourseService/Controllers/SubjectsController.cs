using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.CourseService.Helper;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests;
using PRN232.LMS.Services.Models.Responses;

namespace PRN232.LMS.CourseService.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    [Produces("application/json", "application/xml")]
    [Authorize]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _service;
        public SubjectsController(ISubjectService service) => _service = service;

        /// <summary>Lấy danh sách subjects (search, sort, paging)</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SubjectResponse>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParams q)
        {
            var result = await _service.GetAllAsync(q);
            return Ok(ApiResponse<PagedResult<SubjectResponse>>.Ok(result));
        }

        /// <summary>Lấy subject theo ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), 404)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(ApiResponse<SubjectResponse>.Fail("Subject not found"));
            return Ok(ApiResponse<SubjectResponse>.Ok(result));
        }

        /// <summary>Lấy courses của subject</summary>
        [HttpGet("{id:int}/courses")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseResponse>>), 200)]
        public async Task<IActionResult> GetCoursesBySubjectId([FromRoute] int id, [FromQuery] QueryParams q)
        {
            var result = await _service.GetCoursesBySubjectIdAsync(id, q);
            return Ok(ApiResponse<PagedResult<CourseResponse>>.Ok(result));
        }

        /// <summary>Tạo subject mới</summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), 201)]
        public async Task<IActionResult> Create([FromBody] SubjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<SubjectResponse>.Fail("Invalid request", ModelState));
            var result = await _service.CreateAsync(request);
            return StatusCode(201, ApiResponse<SubjectResponse>.Ok(result, "Subject created"));
        }

        /// <summary>Cập nhật subject</summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), 404)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SubjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<SubjectResponse>.Fail("Invalid request", ModelState));
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound(ApiResponse<SubjectResponse>.Fail("Subject not found"));
            return Ok(ApiResponse<SubjectResponse>.Ok(result, "Subject updated"));
        }

        /// <summary>Xoá subject (Admin only)</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(ApiResponse<object>.Fail("Subject not found"));
            return Ok(ApiResponse<object>.Ok(null!, "Subject deleted"));
        }
    }
}
