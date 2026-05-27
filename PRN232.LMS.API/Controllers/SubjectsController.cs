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
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), 404)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(ApiResponse<SubjectResponse>.Fail("Subject not found"));
            return Ok(ApiResponse<SubjectResponse>.Ok(result));
        }

        /// <summary>Lấy danh sách courses của subject (search, sort, paging)</summary>
        [HttpGet("{id}/courses")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseResponse>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseResponse>>), 404)]
        public async Task<IActionResult> GetCoursesBySubjectId(int id, [FromQuery] QueryParams q)
        {
            try
            {
                var result = await _service.GetCoursesBySubjectIdAsync(id, q);
                return Ok(ApiResponse<PagedResult<CourseResponse>>.Ok(result));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<PagedResult<CourseResponse>>.Fail("Subject not found"));
            }
        }

        /// <summary>Tạo subject mới</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), 201)]
        public async Task<IActionResult> Create([FromBody] SubjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<SubjectResponse>.Fail("Invalid request", ModelState));
            var result = await _service.CreateAsync(request);
            return StatusCode(201, ApiResponse<SubjectResponse>.Ok(result, "Subject created"));
        }

        /// <summary>Cập nhật subject</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<SubjectResponse>), 404)]
        public async Task<IActionResult> Update(int id, [FromBody] SubjectRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<SubjectResponse>.Fail("Invalid request", ModelState));
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound(ApiResponse<SubjectResponse>.Fail("Subject not found"));
            return Ok(ApiResponse<SubjectResponse>.Ok(result, "Subject updated"));
        }

        /// <summary>Xoá subject</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(ApiResponse<object>.Fail("Subject not found"));
            return Ok(ApiResponse<object>.Ok(null!, "Subject deleted"));
        }
    }
}
