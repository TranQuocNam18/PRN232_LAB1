using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests
{
    public class SubjectRequest
    {
        [Required(ErrorMessage = "SubjectCode is required")]
        [StringLength(20, ErrorMessage = "SubjectCode cannot exceed 20 characters")]
        public string SubjectCode { get; set; } = null!;

        [Required(ErrorMessage = "SubjectName is required")]
        [StringLength(100, ErrorMessage = "SubjectName cannot exceed 100 characters")]
        public string SubjectName { get; set; } = null!;

        [Required(ErrorMessage = "Credit is required")]
        [Range(1, 10, ErrorMessage = "Credit must be between 1 and 10")]
        public int Credit { get; set; }
    }
}
