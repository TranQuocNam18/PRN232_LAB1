using PRN232.LMS.Services.Attributes;
using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests
{
    public class StudentRequest
    {
        [Required(ErrorMessage = "FullName is required")]
        [StringLength(100, ErrorMessage = "FullName cannot exceed 100 characters")]
        public string FullName { get; set; } = null!;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "DateOfBirth is required")]
        public DateTime DateOfBirth { get; set; }

        [FptuStudentId]
        public string? StudentCode { get; set; }
    }
}
