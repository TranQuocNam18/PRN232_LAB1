namespace PRN232.LMS.Services.Models.Responses.Auth
{
    public class AuthResponse
    {
        public string Token { get; set; } = null!;
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
