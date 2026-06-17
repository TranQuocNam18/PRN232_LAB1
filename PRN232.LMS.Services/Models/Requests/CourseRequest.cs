using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests
{
    public class CourseRequest
    {
        [Required(ErrorMessage = "CourseName is required")]
        [StringLength(100, ErrorMessage = "CourseName cannot exceed 100 characters")]
        public string CourseName { get; set; } = null!;

        [Required(ErrorMessage = "SemesterId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "SemesterId must be a positive number")]
        public int SemesterId { get; set; }

        public int? SubjectId { get; set; }
    }
}
