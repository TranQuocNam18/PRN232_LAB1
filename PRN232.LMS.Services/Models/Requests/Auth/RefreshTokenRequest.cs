using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests.Auth
{
    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "RefreshToken is required")]
        public string RefreshToken { get; set; } = null!;
    }
}
