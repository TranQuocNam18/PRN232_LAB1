using System.ComponentModel.DataAnnotations;

namespace PRN232.LMS.Services.Models.Requests
{
    public class SemesterRequest
    {
        [Required(ErrorMessage = "SemesterName is required")]
        [StringLength(100, ErrorMessage = "SemesterName cannot exceed 100 characters")]
        public string SemesterName { get; set; } = null!;

        [Required(ErrorMessage = "StartDate is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "EndDate is required")]
        public DateTime EndDate { get; set; }
    }
}
