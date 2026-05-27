using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRN232.LMS.Services.Models.Responses
{
    public class CourseResponse
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = null!;
        public int SemesterId { get; set; }
        public int? SubjectId { get; set; }
        public SemesterResponse? Semester { get; set; }
        public SubjectResponse? Subject { get; set; }
    }

}
