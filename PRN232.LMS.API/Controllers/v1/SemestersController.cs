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
    [Route("api/v{version:apiVersion}/semesters")]
    [Produces("application/json", "application/xml")]
    [Authorize]
    public class SemestersController : ControllerBase
    {
        private readonly ISemesterService _service;
        public SemestersController(ISemesterService service) => _service = service;

        /// <summary>Lấy danh sách semesters (search, sort, paging)</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<SemesterResponse>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParams q)
        {
            var result = await _service.GetAllAsync(q);
            return Ok(ApiResponse<PagedResult<SemesterResponse>>.Ok(result));
        }

        /// <summary>Lấy semester theo ID (kèm courses)</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), 404)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(ApiResponse<SemesterResponse>.Fail("Semester not found"));
            return Ok(ApiResponse<SemesterResponse>.Ok(result));
        }

        /// <summary>Lấy courses của semester</summary>
        [HttpGet("{id:int}/courses")]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<CourseResponse>>), 200)]
        public async Task<IActionResult> GetCoursesBySemesterId([FromRoute] int id, [FromQuery] QueryParams q)
        {
            var result = await _service.GetCoursesBySemesterIdAsync(id, q);
            return Ok(ApiResponse<PagedResult<CourseResponse>>.Ok(result));
        }

        /// <summary>Tạo semester mới</summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), 201)]
        public async Task<IActionResult> Create([FromBody] SemesterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<SemesterResponse>.Fail("Invalid request", ModelState));
            var result = await _service.CreateAsync(request);
            return StatusCode(201, ApiResponse<SemesterResponse>.Ok(result, "Semester created"));
        }

        /// <summary>Cập nhật semester</summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<SemesterResponse>), 200)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SemesterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<SemesterResponse>.Fail("Invalid request", ModelState));
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound(ApiResponse<SemesterResponse>.Fail("Semester not found"));
            return Ok(ApiResponse<SemesterResponse>.Ok(result, "Semester updated"));
        }

        /// <summary>Xoá semester (Admin only)</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(ApiResponse<object>.Fail("Semester not found"));
            return Ok(ApiResponse<object>.Ok(null!, "Semester deleted"));
        }
    }
}
