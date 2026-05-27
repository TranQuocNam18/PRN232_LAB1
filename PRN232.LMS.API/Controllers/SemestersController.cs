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
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _service;
        public SemestersController(ISemesterService service) => _service = service;

        /// <summary>Lấy danh sách semesters (search, sort, paging, expand)</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SemesterResponse>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParams q)
        {
            var result = await _service.GetAllAsync(q);
            return Ok(ApiResponse<PagedResult<SemesterResponse>>.Ok(result));
        }

        /// <summary>Lấy semester theo ID (kèm courses)</summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), 404)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(ApiResponse<SemesterResponse>.Fail("Semester not found"));
            return Ok(ApiResponse<SemesterResponse>.Ok(result));
        }

        /// <summary>Lấy danh sách courses của semester (search, sort, paging)</summary>
        [HttpGet("{id}/courses")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseResponse>>), 200)]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseResponse>>), 404)]
        public async Task<IActionResult> GetCoursesBySemesterId(int id, [FromQuery] QueryParams q)
        {
            try
            {
                var result = await _service.GetCoursesBySemesterIdAsync(id, q);
                return Ok(ApiResponse<PagedResult<CourseResponse>>.Ok(result));
            }
            catch (KeyNotFoundException)
            {
                return NotFound(ApiResponse<PagedResult<CourseResponse>>.Fail("Semester not found"));
            }
        }

        /// <summary>Tạo semester mới</summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), 201)]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), 400)]
        public async Task<IActionResult> Create([FromBody] SemesterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<SemesterResponse>.Fail("Invalid request", ModelState));
            var result = await _service.CreateAsync(request);
            return StatusCode(201, ApiResponse<SemesterResponse>.Ok(result, "Semester created"));
        }

        /// <summary>Cập nhật semester</summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), 404)]
        public async Task<IActionResult> Update(int id, [FromBody] SemesterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<SemesterResponse>.Fail("Invalid request", ModelState));
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound(ApiResponse<SemesterResponse>.Fail("Semester not found"));
            return Ok(ApiResponse<SemesterResponse>.Ok(result, "Semester updated"));
        }

        /// <summary>Xoá semester</summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 404)]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(ApiResponse<object>.Fail("Semester not found"));
            return Ok(ApiResponse<object>.Ok(null!, "Semester deleted"));
        }
    }
}
