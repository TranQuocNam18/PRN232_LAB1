using PRN232.LMS.Services.Models.Requests.Auth;
using PRN232.LMS.Services.Models.Responses.Auth;

namespace PRN232.LMS.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse?> LoginAsync(LoginRequest request);
        Task<AuthResponse?> RefreshTokenAsync(RefreshTokenRequest request);
    }
}
