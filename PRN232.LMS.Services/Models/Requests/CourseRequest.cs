using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Models.Requests
{
    public class CourseRequest
    {
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
        public int? SubjectId { get; set; }
    }
}
