using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests
{
    public class EnrollmentRequest
    {
        [Required(ErrorMessage = "StudentId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "StudentId must be a positive number")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "CourseId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "CourseId must be a positive number")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "EnrollDate is required")]
        public DateTime EnrollDate { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [RegularExpression("^(Active|Inactive|Pending)$",
            ErrorMessage = "Status must be one of: Active, Inactive, Pending")]
        public string Status { get; set; } = null!;
    }
}
