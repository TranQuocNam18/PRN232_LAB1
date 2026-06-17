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
    [Route("api/v{version:apiVersion}/enrollments")]
    [Produces("application/json", "application/xml")]
    public class EnrollmentsController : ControllerBase
    {
        private readonly IEnrollmentService _service;
        public EnrollmentsController(IEnrollmentService service) => _service = service;

        /// <summary>Lấy danh sách enrollments (search, sort, paging)</summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<EnrollmentResponse>>), 200)]
        public async Task<IActionResult> GetAll([FromQuery] QueryParams q)
        {
            var result = await _service.GetAllAsync(q);
            return Ok(ApiResponse<PagedResult<EnrollmentResponse>>.Ok(result));
        }

        /// <summary>Lấy enrollment theo ID</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), 404)]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null) return NotFound(ApiResponse<EnrollmentResponse>.Fail("Enrollment not found"));
            return Ok(ApiResponse<EnrollmentResponse>.Ok(result));
        }

        /// <summary>Tạo enrollment mới</summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), 201)]
        public async Task<IActionResult> Create([FromBody] EnrollmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<EnrollmentResponse>.Fail("Invalid request", ModelState));
            var result = await _service.CreateAsync(request);
            return StatusCode(201, ApiResponse<EnrollmentResponse>.Ok(result, "Enrollment created"));
        }

        /// <summary>Cập nhật enrollment</summary>
        [HttpPut("{id:int}")]
        [Authorize]
        [ProducesResponseType(typeof(ApiResponse<EnrollmentResponse>), 200)]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] EnrollmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ApiResponse<EnrollmentResponse>.Fail("Invalid request", ModelState));
            var result = await _service.UpdateAsync(id, request);
            if (result == null) return NotFound(ApiResponse<EnrollmentResponse>.Fail("Enrollment not found"));
            return Ok(ApiResponse<EnrollmentResponse>.Ok(result, "Enrollment updated"));
        }

        /// <summary>Xoá enrollment (Admin only)</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ApiResponse<object>), 200)]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var deleted = await _service.DeleteAsync(id);
            if (!deleted) return NotFound(ApiResponse<object>.Fail("Enrollment not found"));
            return Ok(ApiResponse<object>.Ok(null!, "Enrollment deleted"));
        }
    }
}
