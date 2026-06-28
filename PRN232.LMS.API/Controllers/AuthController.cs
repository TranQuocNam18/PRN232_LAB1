using Microsoft.AspNetCore.Mvc;
using PRN232.LMS.API.Helper;
using PRN232.LMS.Services.Interfaces;
using PRN232.LMS.Services.Models.Requests.Auth;
using PRN232.LMS.Services.Models.Responses.Auth;

namespace PRN232.LMS.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    [Route("api/v1/auth")]
    [Route("api/v{version:apiVersion}/auth")]
    [Produces("application/json", "application/xml")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid request data", ModelState));
            }

            var result = await _authService.LoginAsync(request);
            if (result == null)
            {
                return Unauthorized(ApiResponse<object>.Fail("Invalid username or password"));
            }

            return Ok(ApiResponse<AuthResponse>.Ok(result, "Login successful"));
        }

        [HttpPost("refresh-token")]
        [ProducesResponseType(typeof(ApiResponse<AuthResponse>), 200)]
        [ProducesResponseType(typeof(ApiResponse<object>), 400)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid request data", ModelState));
            }

            var result = await _authService.RefreshTokenAsync(request);
            if (result == null)
            {
                return BadRequest(ApiResponse<object>.Fail("Invalid or expired refresh token"));
            }

            return Ok(ApiResponse<AuthResponse>.Ok(result, "Token refreshed successfully"));
        }
    }
}
